using MASAN_SERIALIZATION.Production;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace StorageIntegration
{
    internal static partial class Program
    {
        private const string RefillPreviouslyActivated = "SYNTHETIC_REFILL_ALREADY_ACTIVE_A";
        private const string RefillEligibleB = "SYNTHETIC_REFILL_ELIGIBLE_B";
        private const string RefillEligibleC = "SYNTHETIC_REFILL_ELIGIBLE_C";

        private static void PORefillRegressions(string sandbox)
        {
            // Use the real compiled initializer and refill methods. Paths/cache were
            // redirected by POInitializationRegressions; no application constructor runs.
            var exclusion = CreateRefillFixture(sandbox, "refill-exclusion", "8930000000201");
            if (exclusion != null)
            {
                WriteMesCodes(exclusion, new[] { RefillPreviouslyActivated, RefillEligibleB, RefillEligibleC });
                var before = InitializationRows(exclusion.Database);
                var result = Refill(exclusion);
                var after = InitializationRows(exclusion.Database);
                RecordKnownBaselineInvariant("po-refill.activated-sibling-not-reintroduced",
                    result.issuccess && !ContainsCode(exclusion.Database, RefillPreviouslyActivated), result.message);
                RecordKnownBaselineInvariant("po-refill.eligible-codes-added",
                    result.issuccess && result.count == 2 && ContainsCode(exclusion.Database, RefillEligibleB) &&
                    ContainsCode(exclusion.Database, RefillEligibleC) && after.Length == before.Length + 2,
                    result.message + "; count before=" + before.Length + "; count after=" + after.Length);
                Assert("po-refill.existing-activation-preserved", before.All(row => after.Contains(row)),
                    "Existing rows, activation, carton and sub-camera timestamp must not change during refill.");
            }

            var duplicates = CreateRefillFixture(sandbox, "refill-duplicate-mes", "8930000000202");
            if (duplicates != null)
            {
                // The raw MES count is three, so the actual two-row dictionary enters
                // the refill branch. Only B is eligible; repeated B must be inserted once.
                WriteMesCodes(duplicates, new[] { RefillPreviouslyActivated, RefillEligibleB, RefillEligibleB });
                var before = InitializationRows(duplicates.Database);
                var result = Refill(duplicates);
                var after = InitializationRows(duplicates.Database);
                RecordKnownBaselineInvariant("po-refill.duplicate-mes-inserts-once",
                    result.issuccess && result.count == 1 && CountCode(duplicates.Database, RefillEligibleB) == 1 &&
                    !ContainsCode(duplicates.Database, RefillPreviouslyActivated) && after.Length == before.Length + 1,
                    result.message + "; count before=" + before.Length + "; count after=" + after.Length);
                Assert("po-refill.duplicate-mes-existing-rows-preserved", before.All(row => after.Contains(row)),
                    "Duplicate MES entries must neither roll back the eligible insertion nor change existing activation data.");
            }

            var unreadable = CreateRefillFixture(sandbox, "refill-unreadable-sibling", "8930000000203");
            if (unreadable != null)
            {
                WriteMesCodes(unreadable, new[] { RefillPreviouslyActivated, RefillEligibleB, RefillEligibleC });
                File.WriteAllText(Path.Combine(unreadable.BasePath, "OLD_BROKEN.db"),
                    "SYNTHETIC UNREADABLE REFILL SIBLING", Encoding.ASCII);
                var before = InitializationRows(unreadable.Database);
                var result = Refill(unreadable);
                var after = InitializationRows(unreadable.Database);
                RecordKnownBaselineInvariant("po-refill.unreadable-sibling-fails-closed",
                    !result.issuccess && before.SequenceEqual(after),
                    result.message + "; unchanged rows=" + before.SequenceEqual(after));
            }
        }

        private static InitializationFixture CreateRefillFixture(string sandbox, string name, string gtin)
        {
            var fixture = CreateInitializationFixture(sandbox, name, gtin);
            WriteMesCodes(fixture, 2);
            var bootstrap = UninitializedProductionOrder().Check_Database_File(fixture.Order, "2");
            var ready = bootstrap.issucess && RowCount(fixture.Database) == 2;
            Assert("po-refill." + name + ".bootstrap", ready, bootstrap.message);
            if (!ready) return null;
            using (var current = Open(fixture.Database))
                Execute(current, "UPDATE UniqueCodes SET Status=1,cartonCode='SYNTHETIC_REFILL_PRESERVE',SubCamera_ActivateDate='2026-11-01T03:04:05Z' WHERE ID=1;");
            using (var sibling = Open(Path.Combine(fixture.BasePath, "OLD_ACTIVE.db")))
            {
                Execute(sibling, "CREATE TABLE UniqueCodes (Code TEXT NOT NULL UNIQUE, Status INTEGER NOT NULL);");
                using (var insert = new SQLiteCommand("INSERT INTO UniqueCodes (Code,Status) VALUES (@code,1);", sibling))
                {
                    insert.Parameters.AddWithValue("@code", RefillPreviouslyActivated);
                    insert.ExecuteNonQuery();
                }
            }
            return fixture;
        }

        private static TResult Refill(InitializationFixture fixture)
        {
            return new ProductionOrder.GetDataPO().Check_And_Add_Missing_Codes_To_Dictionary(
                fixture.Order, new ProductionOrder.GetfromMES());
        }

        private static bool ContainsCode(string path, string code) { return CountCode(path, code) > 0; }

        private static long CountCode(string path, string code)
        {
            using (var connection = Open(path))
            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM UniqueCodes WHERE Code=@code;", connection))
            {
                command.Parameters.AddWithValue("@code", code);
                return Convert.ToInt64(command.ExecuteScalar());
            }
        }

        private static void WriteMesCodes(InitializationFixture fixture, IEnumerable<string> source)
        {
            var codes = new JArray(source.Select(code => new JObject
            { ["code"] = code, ["createdAt"] = "2026-11-01T00:00:00Z", ["blockNo"] = 0 }));
            var json = new JObject { ["blocks"] = new JObject { ["0"] = new JObject { ["codes"] = codes } } };
            File.WriteAllText(fixture.MesJson, json.ToString(Formatting.None), new UTF8Encoding(false));
        }
    }
}
