using MASAN_SERIALIZATION;
using MASAN_SERIALIZATION.Production;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StorageIntegration
{
    internal static partial class Program
    {
        private const int CodeCount = 200000;
        private const string CurrentOrder = "PO200000";
        private const string Gtin = "8930000000001";
        private const string ProductionDate = "2026-11-01";
        private static readonly List<AssertionResult> Assertions = new List<AssertionResult>();
        private static readonly List<Metric> Metrics = new List<Metric>();
        private static readonly List<FixtureResult> Fixtures = new List<FixtureResult>();
        private static readonly List<ContentionResult> Contention = new List<ContentionResult>();
        private static bool expectBaseline;

        private static int Main(string[] args)
        {
            var started = DateTime.UtcNow;
            var options = Options.Parse(args);
            expectBaseline = options.ExpectBaseline;
            Directory.CreateDirectory(options.OutputDirectory);
            var sandbox = Path.Combine(options.OutputDirectory, "sandbox");
            if (Directory.Exists(sandbox)) throw new InvalidOperationException("Use a fresh output directory: sandbox already exists.");
            Directory.CreateDirectory(sandbox);
            var report = new RunReport
            {
                Label = options.Label, StartedUtc = started, ExpectBaseline = expectBaseline,
                DirectLookupIterationsPerScenario = options.Iterations, BulkIterationsPerScenario = options.BulkIterations, WarmupCalls = options.Warmup,
                RequiredUniqueCodesPerDatabase = CodeCount,
                SourceCheckpoint = options.SourceCheckpoint,
                ApplicationAssembly = typeof(ProductionOrder.GetDataPO).Assembly.Location,
                ApplicationSha256 = Sha256(typeof(ProductionOrder.GetDataPO).Assembly.Location),
                SQLiteAssembly = typeof(SQLiteConnection).Assembly.FullName,
                SQLiteVersion = SQLiteConnection.SQLiteVersion,
                Runtime = Environment.Version.ToString(), OperatingSystem = Environment.OSVersion.ToString(),
                Is64BitProcess = Environment.Is64BitProcess, ProcessorCount = Environment.ProcessorCount,
                StopwatchFrequency = Stopwatch.Frequency,
                Provenance = File.Exists(options.ProvenancePath) ? JObject.Parse(File.ReadAllText(options.ProvenancePath)) : null,
                Assertions = Assertions, Metrics = Metrics, Fixtures = Fixtures, Contention = Contention
            };
            try
            {
                Assert("environment.x64", Environment.Is64BitProcess, "64-bit process is required for the selected native SQLite library.");
                var setup = Stopwatch.StartNew();
                var template = Path.Combine(sandbox, "unique-codes-template.db");
                CreateTemplate(template);
                setup.Stop();
                report.TemplateSetupMilliseconds = setup.Elapsed.TotalMilliseconds;
                VerifyDatabase(template, "template");
                foreach (var siblings in new[] { 1, 5 })
                {
                    var fixture = CreateFixture(sandbox, template, "siblings-" + siblings, siblings);
                    RegressionChecks(fixture);
                    Benchmark(fixture, options);
                }
                var first = CreateFixture(sandbox, template, "first-order", 0);
                var firstReader = new ProductionOrder.GetDataPO();
                AssertCheck("first-order.inactive", firstReader.CheckCodeActivatedInOtherPO(Code(500), CurrentOrder), true, false);
                AssertBulk("first-order.bulk", firstReader.GetAllActivatedCodesFromOtherPOs(CurrentOrder), true, new HashSet<string>());
                foreach (var failure in new[] { "corrupt", "missing-schema" })
                {
                    var fixture = CreateFixture(sandbox, template, failure, 1);
                    var badPath = Path.Combine(fixture.BasePath, "OLD_BROKEN.db");
                    if (failure == "corrupt") File.WriteAllText(badPath, "THIS IS NOT A SQLITE DATABASE", Encoding.ASCII);
                    else using (var connection = Open(badPath)) Execute(connection, "CREATE TABLE WrongTable (ID INTEGER PRIMARY KEY);");
                    var reader = new ProductionOrder.GetDataPO();
                    AssertCheck(failure + ".check-fails-closed", reader.CheckCodeActivatedInOtherPO(Code(500), CurrentOrder), false, true);
                    var bulk = reader.GetAllActivatedCodesFromOtherPOs(CurrentOrder);
                    Record(failure + ".bulk-fails-closed", !bulk.success, bulk.success, bulk.message, true);
                }
                var missingMetadata = new ProductionOrder.GetDataPO();
                AssertCheck("missing-metadata.check", missingMetadata.CheckCodeActivatedInOtherPO(Code(500), "NO_METADATA"), false, false);
                var missingBulk = missingMetadata.GetAllActivatedCodesFromOtherPOs("NO_METADATA");
                Record("missing-metadata.bulk", !missingBulk.success, missingBulk.success, missingBulk.message, false);
                ContentionProbe(CreateFixture(sandbox, template, "exclusive-lock", 1));
                StaleSnapshotRegression(CreateFixture(sandbox, template, "stale-snapshot", 0));
                WriteFailureRegression(CreateFixture(sandbox, template, "write-failure", 0));
                POInitializationRegressions(sandbox);
                PORefillRegressions(sandbox);
            }
            catch (Exception ex)
            {
                report.FatalError = ex.ToString();
                Assert("harness.unhandled", false, ex.ToString());
            }
            finally
            {
                SQLiteConnection.ClearAllPools();
                report.CompletedUtc = DateTime.UtcNow;
                report.PassedChecks = Assertions.Count(x => x.Outcome == "pass");
                report.ExpectedBaselineDefects = Assertions.Count(x => x.Outcome == "expected_defect");
                report.UnexpectedFailures = Assertions.Count(x => x.Outcome == "fail") + Metrics.Sum(x => x.Failures + x.Exceptions);
                report.ProductConformant = report.UnexpectedFailures == 0 && report.ExpectedBaselineDefects == 0;
                report.RunOutcome = report.UnexpectedFailures > 0 ? "failed" : report.ExpectedBaselineDefects > 0 ? "baseline_defects_reproduced" : "passed";
                File.WriteAllText(Path.Combine(options.OutputDirectory, "results.json"), JsonConvert.SerializeObject(report, Formatting.Indented), new UTF8Encoding(false));
                WriteCsv(Path.Combine(options.OutputDirectory, "timings.csv"));
                File.WriteAllText(Path.Combine(options.OutputDirectory, "assertions.csv"), "case,outcome,required_behavior_passed,actual_success,detail\r\n" + string.Join("\r\n", Assertions.Select(x => string.Join(",", Csv(x.Name), Csv(x.Outcome), x.RequiredBehaviorPassed.ToString(), x.ActualSuccess.HasValue ? x.ActualSuccess.Value.ToString() : "", Csv(x.Detail)))), new UTF8Encoding(false));
                Console.WriteLine("Outcome={0}; productConformant={1}; passed={2}; expectedDefects={3}; failures={4}; output={5}", report.RunOutcome, report.ProductConformant, report.PassedChecks, report.ExpectedBaselineDefects, report.UnexpectedFailures, options.OutputDirectory);
            }
            return report.UnexpectedFailures == 0 ? 0 : 1;
        }

        private static Fixture CreateFixture(string sandbox, string template, string name, int siblings)
        {
            var clock = Stopwatch.StartNew();
            var root = Path.Combine(sandbox, name);
            var metadata = Path.Combine(root, "metadata");
            var data = Path.Combine(root, "po-data");
            Directory.CreateDirectory(metadata);
            Directory.CreateDirectory(data);
            SetSandboxPaths(root, metadata, data);
            WriteMetadata(metadata, CurrentOrder);
            var basePath = ProductionOrder.GetOrderBasePath(CurrentOrder);
            var expectedBase = Path.Combine(data, "2026-11", Gtin);
            if (!string.Equals(Path.GetFullPath(basePath), Path.GetFullPath(expectedBase), StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Production GetOrderBasePath escaped the expected fixture path: " + basePath);
            var fixture = new Fixture { Name = name, BasePath = basePath, Siblings = siblings, ExpectedActivated = new HashSet<string>(StringComparer.OrdinalIgnoreCase) };
            var current = Path.Combine(basePath, CurrentOrder + ".db");
            File.Copy(template, current);
            Activate(current, Code(CodeCount - 1), 1);
            VerifyDatabase(current, name + "/" + CurrentOrder);
            for (var index = 0; index < siblings; index++)
            {
                var order = "OLD" + (index + 1).ToString("D2", CultureInfo.InvariantCulture);
                WriteMetadata(metadata, order);
                var db = Path.Combine(basePath, order + ".db");
                File.Copy(template, db);
                Activate(db, Code(index), index % 2 == 0 ? 1 : -1);
                Activate(db, Code(100), 2);
                fixture.ExpectedActivated.Add(Code(index));
                fixture.ExpectedActivated.Add(Code(100));
                VerifyDatabase(db, name + "/" + order);
            }
            clock.Stop();
            Fixtures.Add(new FixtureResult { Name = name, SiblingCount = siblings, RowsPerDatabase = CodeCount, ExpectedActivatedUnionCount = fixture.ExpectedActivated.Count, SetupMilliseconds = clock.Elapsed.TotalMilliseconds, DatabaseOrder = Directory.GetFiles(basePath, "*.db").Select(Path.GetFileName).ToArray() });
            return fixture;
        }

        private static void SetSandboxPaths(string root, string metadata, string data)
        {
            var values = new Dictionary<string, string>
            {
                { "dataPath", data }, { "poMesJsonPODataPath", metadata },
                { "poMesJsonCodesPath", Path.Combine(root, "unused-codes") },
                { "POLog_dbPath", Path.Combine(root, "unused-po-log.db") }
            };
            foreach (var item in values)
            {
                var field = typeof(ProductionOrder).GetField(item.Key, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (field == null || field.FieldType != typeof(string)) throw new MissingFieldException("Production path field changed: " + item.Key);
                field.SetValue(null, item.Value);
                if (!Equals(field.GetValue(null), item.Value)) throw new InvalidOperationException("Failed to redirect " + item.Key);
            }
        }

        private static void RegressionChecks(Fixture fixture)
        {
            var reader = new ProductionOrder.GetDataPO();
            AssertCheck(fixture.Name + ".inactive", reader.CheckCodeActivatedInOtherPO(Code(500), CurrentOrder), true, false);
            AssertCheck(fixture.Name + ".absent", reader.CheckCodeActivatedInOtherPO("ABSENT_SYNTHETIC_CODE", CurrentOrder), true, false);
            AssertCheck(fixture.Name + ".activated-sibling", reader.CheckCodeActivatedInOtherPO(Code(0), CurrentOrder), false, false);
            AssertCheck(fixture.Name + ".current-excluded", reader.CheckCodeActivatedInOtherPO(Code(CodeCount - 1), CurrentOrder), true, false);
            if (fixture.Siblings > 1) AssertCheck(fixture.Name + ".negative-status-is-active", reader.CheckCodeActivatedInOtherPO(Code(1), CurrentOrder), false, false);
            AssertBulk(fixture.Name + ".exact-activated-union", reader.GetAllActivatedCodesFromOtherPOs(CurrentOrder), true, fixture.ExpectedActivated);
            foreach (var name in new[] { "Record_ignored.db", "carton_ignored.db", "Send_AWS_ignored.db", "Recive_AWS_ignored.db" })
                File.WriteAllText(Path.Combine(fixture.BasePath, name), "IGNORED AUXILIARY FILE", Encoding.ASCII);
            AssertCheck(fixture.Name + ".auxiliary-files-ignored", reader.CheckCodeActivatedInOtherPO(Code(500), CurrentOrder), true, false);
            AssertBulk(fixture.Name + ".auxiliary-files-bulk-ignored", reader.GetAllActivatedCodesFromOtherPOs(CurrentOrder), true, fixture.ExpectedActivated);
            foreach (var name in new[] { "Record_ignored.db", "carton_ignored.db", "Send_AWS_ignored.db", "Recive_AWS_ignored.db" })
                File.Delete(Path.Combine(fixture.BasePath, name));
        }

        private static void Benchmark(Fixture fixture, Options options)
        {
            var reader = new ProductionOrder.GetDataPO();
            Measure(fixture, "CheckCodeActivatedInOtherPO", "activated_duplicate", options,
                () => reader.CheckCodeActivatedInOtherPO(Code(0), CurrentOrder), r => !r.issuccess && r.data != null && r.data.Rows.Count > 0, r => { if (r.data != null) r.data.Dispose(); });
            Measure(fixture, "CheckCodeActivatedInOtherPO", "inactive", options,
                () => reader.CheckCodeActivatedInOtherPO(Code(500), CurrentOrder), r => r.issuccess, r => { if (r.data != null) r.data.Dispose(); });
            Measure(fixture, "CheckCodeActivatedInOtherPO", "missing", options,
                () => reader.CheckCodeActivatedInOtherPO("ABSENT_SYNTHETIC_CODE", CurrentOrder), r => r.issuccess, r => { if (r.data != null) r.data.Dispose(); });
            Measure(fixture, "GetAllActivatedCodesFromOtherPOs", "activated_union", options,
                () => reader.GetAllActivatedCodesFromOtherPOs(CurrentOrder), r => r.success && r.activatedCodes.SetEquals(fixture.ExpectedActivated), r => { }, options.BulkIterations);
        }

        private static void ContentionProbe(Fixture fixture)
        {
            const int targetHoldMilliseconds = 250;
            var oldDatabase = Path.Combine(fixture.BasePath, "OLD01.db");
            using (var blocker = Open(oldDatabase))
            using (var started = new ManualResetEventSlim(false))
            {
                // WAL permits concurrent readers; explicitly select DELETE journal mode for this contention probe.
                Execute(blocker, "PRAGMA journal_mode=DELETE;");
                Task<TResult> call = null;
                var clock = new Stopwatch();
                var holdClock = new Stopwatch();
                var completedBeforeRelease = false;
                var transactionOpen = false;
                try
                {
                    Execute(blocker, "BEGIN EXCLUSIVE;");
                    transactionOpen = true;
                    holdClock.Start();
                    call = Task.Run(() =>
                    {
                        var reader = new ProductionOrder.GetDataPO();
                        clock.Start();
                        started.Set();
                        try { return reader.CheckCodeActivatedInOtherPO(Code(500), CurrentOrder); }
                        finally { clock.Stop(); }
                    });
                    if (!started.Wait(5000)) throw new TimeoutException("Contention worker did not start within 5 seconds.");
                    Thread.Sleep(targetHoldMilliseconds);
                    completedBeforeRelease = call.IsCompleted;
                }
                finally
                {
                    // Always release the synthetic exclusive lock, including when scheduling or waiting fails.
                    if (transactionOpen) Execute(blocker, "ROLLBACK;");
                    holdClock.Stop();
                }
                if (call == null) throw new InvalidOperationException("Contention worker was not created.");
                if (!call.Wait(5000)) throw new TimeoutException("Actual storage call did not finish within 5 seconds after the lock was released.");
                var result = call.Result;
                Contention.Add(new ContentionResult
                {
                    Fixture = fixture.Name, Method = "CheckCodeActivatedInOtherPO", JournalMode = "DELETE",
                    TargetHoldMilliseconds = targetHoldMilliseconds, ActualHoldMilliseconds = holdClock.Elapsed.TotalMilliseconds,
                    QueryMilliseconds = clock.Elapsed.TotalMilliseconds, CompletedBeforeLockRelease = completedBeforeRelease,
                    ActualSuccess = result.issuccess, Message = result.message
                });
                AssertCheck("exclusive-lock.inactive-after-release", result, true, false);
                Console.WriteLine("exclusive-lock: hold={0:F3}ms query={1:F3}ms completedBeforeRelease={2}", holdClock.Elapsed.TotalMilliseconds, clock.Elapsed.TotalMilliseconds, completedBeforeRelease);
            }
        }

        private static void StaleSnapshotRegression(Fixture fixture)
        {
            const string finalCarton = "SYNTHETIC_FINAL_CARTON_0001";
            const string subTimestamp = "2026-11-01T01:03:04Z";
            const int targetIndex = 750;
            var writer = new ProductionOrder.PostDB();
            var data = new ProductionCodeData
            {
                orderNo = CurrentOrder, Code = Code(targetIndex), codeID = targetIndex + 1,
                cartonCode = finalCarton, Activate_User = "SYNTHETIC_TEST", Main_Camera_Status = "1",
                Sub_Camera_Status = "1", Sub_Camera_Activate_Datetime = subTimestamp,
                Activate_Datetime = "2026-11-01T01:02:03Z", Production_Datetime = ProductionDate
            };
            writer.Update_Active_Status(data, CurrentOrder);
            var before = ReadSnapshot(fixture, targetIndex);
            Assert("postdb.final-snapshot-precondition", before.Item1 == finalCarton && before.Item2 == subTimestamp,
                "Actual compiled write: carton=" + before.Item1 + "; subTimestamp=" + before.Item2);

            // Model an older queued main-camera snapshot arriving after carton/sub-camera finalization.
            data.cartonCode = "pending";
            data.Sub_Camera_Activate_Datetime = "";
            data.Sub_Camera_Status = "0";
            writer.Update_Active_Status(data, CurrentOrder);
            var after = ReadSnapshot(fixture, targetIndex);
            RecordKnownBaselineInvariant("postdb.stale-pending-preserves-final-carton", after.Item1 == finalCarton,
                "Expected=" + finalCarton + "; actual=" + after.Item1);
            RecordKnownBaselineInvariant("postdb.stale-empty-preserves-sub-timestamp", after.Item2 == subTimestamp,
                "Expected=" + subTimestamp + "; actual=" + after.Item2);

            data.codeID = targetIndex + 2;
            data.Code = Code(targetIndex + 1);
            data.Main_Camera_Status = "-1";
            data.cartonCode = "pending";
            data.Sub_Camera_Activate_Datetime = "0";
            writer.Update_Active_Status(data, CurrentOrder);
            using (var connection = Open(Path.Combine(fixture.BasePath, CurrentOrder + ".db")))
            using (var command = new SQLiteCommand("SELECT Status FROM UniqueCodes WHERE ID=@id", connection))
            {
                command.Parameters.AddWithValue("@id", data.codeID);
                var actualStatus = Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
                RecordKnownBaselineInvariant("postdb.rejected-status-minus-one-persists", actualStatus == -1,
                    "Expected=-1; actual=" + actualStatus);
            }
        }

        private static void WriteFailureRegression(Fixture fixture)
        {
            var missingRowThrew = false;
            var missingRowDetail = "Actual compiled write returned successfully after UPDATE matched no row.";
            var existing = ReadSnapshot(fixture, 0);
            try
            {
                new ProductionOrder.PostDB().Update_Active_Status(new ProductionCodeData
                {
                    orderNo = CurrentOrder, Code = "SYNTHETIC_MISSING_UPDATE_ROW", codeID = CodeCount + 100,
                    cartonCode = "pending", Activate_User = "SYNTHETIC_TEST", Main_Camera_Status = "1",
                    Sub_Camera_Status = "0", Sub_Camera_Activate_Datetime = "0",
                    Activate_Datetime = "2026-11-01T01:02:03Z", Production_Datetime = ProductionDate
                }, CurrentOrder);
            }
            catch (InvalidOperationException ex)
            {
                missingRowThrew = true;
                missingRowDetail = ex.GetType().FullName + ": " + ex.Message;
            }
            RecordKnownBaselineInvariant("postdb.missing-row-write-fails", missingRowThrew, missingRowDetail);
            var existingAfter = ReadSnapshot(fixture, 0);
            Assert("postdb.missing-row-preserves-existing-data", existing.Item1 == existingAfter.Item1 && existing.Item2 == existingAfter.Item2,
                "A missing target row must leave existing carton/sub-camera data unchanged.");

            using (var connection = Open(Path.Combine(fixture.BasePath, CurrentOrder + ".db")))
                Execute(connection, "ALTER TABLE UniqueCodes RENAME TO DeliberatelyUnavailableCodes;");
            var threw = false;
            var detail = "Actual compiled write returned without propagating the SQLite schema failure.";
            try
            {
                new ProductionOrder.PostDB().Update_Active_Status(new ProductionCodeData
                {
                    orderNo = CurrentOrder, Code = Code(0), codeID = 1, cartonCode = "pending",
                    Activate_User = "SYNTHETIC_TEST", Main_Camera_Status = "1", Sub_Camera_Status = "0",
                    Sub_Camera_Activate_Datetime = "0", Activate_Datetime = "2026-11-01T01:02:03Z", Production_Datetime = ProductionDate
                }, CurrentOrder);
            }
            catch (SQLiteException ex) { threw = true; detail = ex.GetType().FullName + ": " + ex.Message; }
            RecordKnownBaselineInvariant("postdb.write-failure-propagates", threw, detail);
        }

        private static Tuple<string, string> ReadSnapshot(Fixture fixture, int index)
        {
            using (var connection = Open(Path.Combine(fixture.BasePath, CurrentOrder + ".db")))
            using (var command = new SQLiteCommand("SELECT cartonCode,SubCamera_ActivateDate FROM UniqueCodes WHERE Code=@code", connection))
            {
                command.Parameters.AddWithValue("@code", Code(index));
                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read()) throw new InvalidOperationException("Synthetic snapshot row is missing.");
                    return Tuple.Create(reader.GetString(0), reader.GetString(1));
                }
            }
        }

        private static void Measure<T>(Fixture fixture, string method, string scenario, Options options, Func<T> call, Func<T, bool> correct, Action<T> cleanup, int? iterations = null)
        {
            for (var i = 0; i < options.Warmup; i++)
            {
                var value = call();
                Assert(fixture.Name + ".warmup." + method + "." + scenario + "." + i, correct(value), "Warmup correctness.");
                cleanup(value);
            }
            var ticks = new long[iterations ?? options.Iterations];
            var failures = 0;
            var exceptions = 0;
            var memoryBefore = GC.GetTotalMemory(false);
            var total = Stopwatch.StartNew();
            for (var i = 0; i < ticks.Length; i++)
            {
                var before = Stopwatch.GetTimestamp();
                try
                {
                    var result = call();
                    ticks[i] = Stopwatch.GetTimestamp() - before;
                    if (!correct(result)) failures++;
                    cleanup(result);
                }
                catch (Exception ex)
                {
                    ticks[i] = Stopwatch.GetTimestamp() - before;
                    exceptions++;
                    if (exceptions == 1) Console.Error.WriteLine(fixture.Name + "/" + scenario + ": " + ex);
                }
            }
            total.Stop();
            var ordered = ticks.OrderBy(x => x).ToArray();
            var metric = new Metric
            {
                Fixture = fixture.Name, SiblingCount = fixture.Siblings, Method = method, Scenario = scenario,
                Calls = ticks.Length, Failures = failures, Exceptions = exceptions,
                P50Milliseconds = Percentile(ordered, 0.50), P95Milliseconds = Percentile(ordered, 0.95),
                P99Milliseconds = Percentile(ordered, 0.99), MaximumMilliseconds = ToMilliseconds(ordered[ordered.Length - 1]),
                MeanMilliseconds = ordered.Average(x => ToMilliseconds(x)),
                QueryCallsPerSecond = ticks.Length / (ticks.Sum(x => (double)x) / Stopwatch.Frequency),
                LoopCallsPerSecond = ticks.Length / total.Elapsed.TotalSeconds,
                QueryTotalMilliseconds = ticks.Sum(x => ToMilliseconds(x)), LoopWallMilliseconds = total.Elapsed.TotalMilliseconds,
                ManagedMemoryDeltaBytes = GC.GetTotalMemory(false) - memoryBefore
            };
            Metrics.Add(metric);
            File.WriteAllLines(Path.Combine(options.OutputDirectory, fixture.Name + "-" + method + "-" + scenario + "-samples.csv"), new[] { "call,elapsed_ms" }.Concat(ticks.Select((value, index) => (index + 1).ToString(CultureInfo.InvariantCulture) + "," + ToMilliseconds(value).ToString("R", CultureInfo.InvariantCulture))), new UTF8Encoding(false));
            Console.WriteLine("{0}/{1}/{2}: calls={3}, p50={4:F3}ms p95={5:F3}ms p99={6:F3}ms max={7:F3}ms failures={8}", fixture.Name, method, scenario, metric.Calls, metric.P50Milliseconds, metric.P95Milliseconds, metric.P99Milliseconds, metric.MaximumMilliseconds, failures + exceptions);
        }

        private static void CreateTemplate(string path)
        {
            using (var connection = Open(path))
            {
                Execute(connection, "PRAGMA journal_mode=WAL; PRAGMA synchronous=OFF; CREATE TABLE UniqueCodes (ID INTEGER NOT NULL UNIQUE PRIMARY KEY AUTOINCREMENT, Code TEXT NOT NULL UNIQUE, cartonCode TEXT NOT NULL DEFAULT 0, Status INTEGER NOT NULL DEFAULT 0, ActivateDate TEXT NOT NULL DEFAULT 0, ProductionDate TEXT NOT NULL DEFAULT 0, ActivateUser TEXT NOT NULL DEFAULT 0, SubCamera_ActivateDate TEXT NOT NULL DEFAULT 0, Send_Status TEXT NOT NULL DEFAULT Pending, Recive_Status TEXT NOT NULL DEFAULT Pending, Send_Recive_Logs JSON, Duplicate JSON);");
                using (var transaction = connection.BeginTransaction())
                using (var command = new SQLiteCommand("INSERT INTO UniqueCodes (Code,ProductionDate) VALUES (@code,@date)", connection, transaction))
                {
                    var code = command.Parameters.Add("@code", System.Data.DbType.String);
                    command.Parameters.AddWithValue("@date", ProductionDate);
                    command.Prepare();
                    for (var i = 0; i < CodeCount; i++) { code.Value = Code(i); command.ExecuteNonQuery(); }
                    transaction.Commit();
                }
                Execute(connection, "PRAGMA wal_checkpoint(TRUNCATE); PRAGMA synchronous=NORMAL;");
            }
            SQLiteConnection.ClearAllPools();
        }

        private static void VerifyDatabase(string path, string name)
        {
            using (var connection = Open(path))
            using (var command = new SQLiteCommand("SELECT COUNT(*),COUNT(DISTINCT Code) FROM UniqueCodes", connection))
            using (var reader = command.ExecuteReader())
            {
                reader.Read();
                Assert(name + ".exact-200000-unique", reader.GetInt64(0) == CodeCount && reader.GetInt64(1) == CodeCount, "COUNT(*)=" + reader.GetInt64(0) + "; COUNT(DISTINCT Code)=" + reader.GetInt64(1));
            }
        }

        private static void Activate(string path, string code, int status)
        {
            using (var connection = Open(path))
            using (var command = new SQLiteCommand("UPDATE UniqueCodes SET Status=@status,ActivateDate=@date,ActivateUser=@user WHERE Code=@code", connection))
            {
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@date", "2026-11-01T01:02:03Z");
                command.Parameters.AddWithValue("@user", "SYNTHETIC_TEST");
                command.Parameters.AddWithValue("@code", code);
                if (command.ExecuteNonQuery() != 1) throw new InvalidOperationException("Expected exactly one synthetic activation.");
            }
        }

        private static SQLiteConnection Open(string path) { var connection = new SQLiteConnection("Data Source=" + path + ";Version=3;Pooling=False;"); connection.Open(); return connection; }
        private static void Execute(SQLiteConnection connection, string sql) { using (var command = new SQLiteCommand(sql, connection)) command.ExecuteNonQuery(); }
        private static string Code(int index) { return "STORAGE" + index.ToString("D12", CultureInfo.InvariantCulture); }
        private static void WriteMetadata(string path, string order) { File.WriteAllText(Path.Combine(path, order + ".json"), JsonConvert.SerializeObject(new Dictionary<string, string> { { "orderNo", order }, { "productionDate", ProductionDate }, { "GTIN", Gtin } }), new UTF8Encoding(false)); }
        private static double ToMilliseconds(long ticks) { return ticks * 1000.0 / Stopwatch.Frequency; }
        private static double Percentile(long[] ordered, double percentile) { return ToMilliseconds(ordered[Math.Max(0, (int)Math.Ceiling(ordered.Length * percentile) - 1)]); }
        private static string Sha256(string path) { using (var stream = File.OpenRead(path)) using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", ""); }
        private static string Csv(string value) { return "\"" + (value ?? "").Replace("\"", "\"\"") + "\""; }
        private static void Assert(string name, bool passed, string detail) { Record(name, passed, null, detail, false); }
        private static void Record(string name, bool requiredPassed, bool? actualSuccess, string detail, bool failOpenRegression)
        {
            var expectedDefect = !requiredPassed && failOpenRegression && expectBaseline && actualSuccess == true;
            Assertions.Add(new AssertionResult { Name = name, Outcome = requiredPassed ? "pass" : expectedDefect ? "expected_defect" : "fail", RequiredBehaviorPassed = requiredPassed, ActualSuccess = actualSuccess, Detail = detail });
        }
        private static void AssertCheck(string name, TResult actual, bool expectedSuccess, bool failOpenRegression)
        {
            Record(name, actual.issuccess == expectedSuccess && (!failOpenRegression || actual.data == null), actual.issuccess,
                actual.message + "; dataIsNull=" + (actual.data == null), failOpenRegression);
            if (actual.data != null) actual.data.Dispose();
        }
        private static void RecordKnownBaselineInvariant(string name, bool passed, string detail)
        {
            Assertions.Add(new AssertionResult { Name = name, Outcome = passed ? "pass" : expectBaseline ? "expected_defect" : "fail", RequiredBehaviorPassed = passed, ActualSuccess = null, Detail = detail });
        }
        private static void AssertBulk(string name, (bool success, string message, HashSet<string> activatedCodes) actual, bool expectedSuccess, HashSet<string> expected)
        {
            Record(name, actual.success == expectedSuccess && actual.activatedCodes != null && actual.activatedCodes.SetEquals(expected), actual.success, actual.message + "; expectedSet=" + expected.Count + "; actualSet=" + (actual.activatedCodes == null ? -1 : actual.activatedCodes.Count), false);
        }
        private static void WriteCsv(string path)
        {
            var lines = new List<string> { "fixture,siblings,method,scenario,calls,p50_ms,p95_ms,p99_ms,max_ms,mean_ms,query_calls_per_sec,loop_calls_per_sec,failures,exceptions,query_total_ms,loop_wall_ms,managed_memory_delta_bytes" };
            foreach (var m in Metrics) lines.Add(string.Join(",", Csv(m.Fixture), m.SiblingCount, Csv(m.Method), Csv(m.Scenario), m.Calls, m.P50Milliseconds.ToString("R", CultureInfo.InvariantCulture), m.P95Milliseconds.ToString("R", CultureInfo.InvariantCulture), m.P99Milliseconds.ToString("R", CultureInfo.InvariantCulture), m.MaximumMilliseconds.ToString("R", CultureInfo.InvariantCulture), m.MeanMilliseconds.ToString("R", CultureInfo.InvariantCulture), m.QueryCallsPerSecond.ToString("R", CultureInfo.InvariantCulture), m.LoopCallsPerSecond.ToString("R", CultureInfo.InvariantCulture), m.Failures, m.Exceptions, m.QueryTotalMilliseconds.ToString("R", CultureInfo.InvariantCulture), m.LoopWallMilliseconds.ToString("R", CultureInfo.InvariantCulture), m.ManagedMemoryDeltaBytes));
            File.WriteAllLines(path, lines, new UTF8Encoding(false));
        }

        private sealed class Options
        {
            public string OutputDirectory, Label, SourceCheckpoint, ProvenancePath;
            public int Iterations = 1000, BulkIterations = 50, Warmup = 5;
            public bool ExpectBaseline;
            public static Options Parse(string[] args)
            {
                var options = new Options();
                for (var i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--expect-baseline") { options.ExpectBaseline = true; continue; }
                    if (i + 1 == args.Length) throw new ArgumentException("Missing option value: " + args[i]);
                    var key = args[i++]; var value = args[i];
                    switch (key)
                    {
                        case "--output": options.OutputDirectory = Path.GetFullPath(value); break;
                        case "--label": options.Label = value; break;
                        case "--source-checkpoint": options.SourceCheckpoint = value; break;
                        case "--provenance": options.ProvenancePath = value; break;
                        case "--iterations": options.Iterations = int.Parse(value, CultureInfo.InvariantCulture); break;
                        case "--bulk-iterations": options.BulkIterations = int.Parse(value, CultureInfo.InvariantCulture); break;
                        case "--warmup": options.Warmup = int.Parse(value, CultureInfo.InvariantCulture); break;
                        default: throw new ArgumentException("Unknown option: " + key);
                    }
                }
                if (string.IsNullOrWhiteSpace(options.OutputDirectory) || string.IsNullOrWhiteSpace(options.SourceCheckpoint) || string.IsNullOrWhiteSpace(options.Label)) throw new ArgumentException("--output, --label and --source-checkpoint are required.");
                if (options.Iterations < 1 || options.Iterations > 1000 || options.BulkIterations < 1 || options.BulkIterations > 1000 || options.Warmup < 0 || options.Warmup > 20) throw new ArgumentException("Direct/bulk iterations must be 1..1000 and warmup 0..20.");
                foreach (var token in new[] { "Record_", "carton_", "Send_AWS_", "Recive_AWS_" }) if (options.OutputDirectory.Contains(token)) throw new ArgumentException("Output path contains a production auxiliary-file exclusion token: " + token);
                return options;
            }
        }
        private sealed class Fixture { public string Name, BasePath; public int Siblings; public HashSet<string> ExpectedActivated; }
        private sealed class AssertionResult { public string Name, Outcome, Detail; public bool RequiredBehaviorPassed; public bool? ActualSuccess; }
        private sealed class FixtureResult { public string Name; public int SiblingCount, RowsPerDatabase, ExpectedActivatedUnionCount; public double SetupMilliseconds; public string[] DatabaseOrder; }
        private sealed class Metric
        {
            public string Fixture, Method, Scenario;
            public int SiblingCount, Calls, Failures, Exceptions;
            public double P50Milliseconds, P95Milliseconds, P99Milliseconds, MaximumMilliseconds, MeanMilliseconds, QueryCallsPerSecond, LoopCallsPerSecond, QueryTotalMilliseconds, LoopWallMilliseconds;
            public long ManagedMemoryDeltaBytes;
        }
        private sealed class ContentionResult
        {
            public string Fixture, Method, JournalMode, Message;
            public int TargetHoldMilliseconds;
            public double ActualHoldMilliseconds, QueryMilliseconds;
            public bool CompletedBeforeLockRelease, ActualSuccess;
        }
        private sealed class RunReport
        {
            public int SchemaVersion = 1;
            public string Label, SourceCheckpoint, ApplicationAssembly, ApplicationSha256, SQLiteAssembly, SQLiteVersion, Runtime, OperatingSystem, RunOutcome, FatalError;
            public DateTime StartedUtc, CompletedUtc;
            public bool ExpectBaseline, Is64BitProcess, ProductConformant;
            public int DirectLookupIterationsPerScenario, BulkIterationsPerScenario, WarmupCalls, RequiredUniqueCodesPerDatabase, ProcessorCount, PassedChecks, ExpectedBaselineDefects, UnexpectedFailures;
            public long StopwatchFrequency;
            public double TemplateSetupMilliseconds;
            public JObject Provenance;
            public List<AssertionResult> Assertions;
            public List<Metric> Metrics;
            public List<FixtureResult> Fixtures;
            public List<ContentionResult> Contention;
        }
    }
}
