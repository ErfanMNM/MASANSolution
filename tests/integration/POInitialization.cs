using MASAN_SERIALIZATION.Configs;
using MASAN_SERIALIZATION.Production;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace StorageIntegration
{
    internal static partial class Program
    {
        private static void POInitializationRegressions(string sandbox)
        {
            var previousDirectory = Environment.CurrentDirectory;
            var runtimeDirectory = Path.Combine(sandbox, "isolated-runtime");
            Directory.CreateDirectory(runtimeDirectory);
            Environment.CurrentDirectory = runtimeDirectory;
            try
            {
                ConfigureSyntheticAppConfig();
                var productionOrder = UninitializedProductionOrder();

                var missing = CreateInitializationFixture(sandbox, "missing-mes", "8930000000101");
                var first = productionOrder.Check_Database_File(missing.Order, "2");
                Assert("po-init.missing-mes.first-not-ready", !first.issucess, first.message);
                WriteMesCodes(missing, 3);
                var retried = productionOrder.Check_Database_File(missing.Order, "2");
                var retryCount = RowCount(missing.Database);
                RecordKnownBaselineInvariant("po-init.missing-mes.retry-populates", retried.issucess && retryCount >= 2,
                    retried.message + "; UniqueCodes count=" + retryCount);

                var corrupt = CreateInitializationFixture(sandbox, "corrupt-old", "8930000000102");
                WriteMesCodes(corrupt, 3);
                var badSibling = Path.Combine(corrupt.BasePath, "OLD_BROKEN.db");
                File.WriteAllText(badSibling, "SYNTHETIC CORRUPT OLD DATABASE", Encoding.ASCII);
                var blocked = productionOrder.Check_Database_File(corrupt.Order, "2");
                RecordKnownBaselineInvariant("po-init.corrupt-old.first-not-ready", !blocked.issucess, blocked.message);
                File.Delete(badSibling);
                var repaired = productionOrder.Check_Database_File(corrupt.Order, "2");
                var repairedCount = RowCount(corrupt.Database);
                RecordKnownBaselineInvariant("po-init.corrupt-old.repaired-retry-populates", repaired.issucess && repairedCount >= 2,
                    repaired.message + "; UniqueCodes count=" + repairedCount);

                var existing = CreateInitializationFixture(sandbox, "existing-valid", "8930000000103");
                WriteMesCodes(existing, 3);
                var initial = productionOrder.Check_Database_File(existing.Order, "2");
                Assert("po-init.existing-valid.bootstrap", initial.issucess && RowCount(existing.Database) == 3, initial.message);
                using (var connection = Open(existing.Database))
                    Execute(connection, "UPDATE UniqueCodes SET Status=1,cartonCode='SYNTHETIC_PRESERVED_CARTON',SubCamera_ActivateDate='2026-11-01T02:03:04Z' WHERE ID=1;");
                var before = InitializationRows(existing.Database);
                File.Delete(existing.MesJson);
                var repeated = productionOrder.Check_Database_File(existing.Order, "2");
                var after = InitializationRows(existing.Database);
                Assert("po-init.existing-valid.rows-preserved", repeated.issucess && before.SequenceEqual(after),
                    repeated.message + "; row count=" + after.Length + "; existing activation/carton/subtime must remain unchanged.");

                var insufficient = CreateInitializationFixture(sandbox, "insufficient-pool", "8930000000104");
                WriteMesCodes(insufficient, 1);
                var insufficientFirst = productionOrder.Check_Database_File(insufficient.Order, "2");
                Assert("po-init.insufficient-pool.first-not-ready", !insufficientFirst.issucess, insufficientFirst.message);
                var insufficientRetry = productionOrder.Check_Database_File(insufficient.Order, "2");
                RecordKnownBaselineInvariant("po-init.insufficient-pool.retry-not-ready", !insufficientRetry.issucess,
                    insufficientRetry.message + "; UniqueCodes count=" + RowCount(insufficient.Database));
            }
            finally { Environment.CurrentDirectory = previousDirectory; }
        }

        private static void ConfigureSyntheticAppConfig()
        {
            // Inspect metadata only before setting the cache. Never invoke the configuration loader/default writer.
            var type = typeof(AppConfigs);
            FieldInfo cache = null;
            for (var ancestor = type; ancestor != null; ancestor = ancestor.BaseType)
            {
                if (ancestor.TypeInitializer != null) throw new InvalidOperationException("Config static initializer changed; refusing to initialize external configuration.");
                var candidate = ancestor.GetField("current", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (candidate != null && candidate.FieldType == type) { cache = candidate; break; }
            }
            if (cache == null || cache.IsInitOnly || cache.IsLiteral) throw new MissingFieldException("Writable AppConfigs cache was not found.");
            var getter = cache.DeclaringType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly).GetGetMethod();
            var il = getter.GetMethodBody().GetILAsByteArray();
            // SunnyUI3.8.6 non-null cache fast path: ldsfld; box; brfalse.s(+6); ldsfld; ret.
            if (il.Length < 18 || il[0] != 0x7e || il[5] != 0x8c || il[10] != 0x2c || il[11] != 6 || il[12] != 0x7e || il[17] != 0x2a ||
                !il.Skip(1).Take(4).SequenceEqual(il.Skip(13).Take(4)))
                throw new InvalidOperationException("Config Current getter changed; refusing to run its loader.");
            var config = (AppConfigs)FormatterServices.GetUninitializedObject(type);
            config.cartonPack = 24;
            cache.SetValue(null, config);
            if (!ReferenceEquals(cache.GetValue(null), config)) throw new InvalidOperationException("Synthetic config cache was not installed.");
            Assert("po-init.config-isolated", config.cartonPack == 24, "Synthetic uninitialized AppConfigs cache installed; configuration loader/default writer bypassed.");
        }

        private static ProductionOrder UninitializedProductionOrder()
        {
            var instance = (ProductionOrder)FormatterServices.GetUninitializedObject(typeof(ProductionOrder));
            var daos = new Dictionary<string, object>
            {
                { "<getDataPO>k__BackingField", new ProductionOrder.GetDataPO() },
                { "<getfromMES>k__BackingField", new ProductionOrder.GetfromMES() }
            };
            foreach (var pair in daos)
            {
                var field = typeof(ProductionOrder).GetField(pair.Key, BindingFlags.NonPublic | BindingFlags.Instance);
                if (field == null || !field.FieldType.IsInstanceOfType(pair.Value)) throw new MissingFieldException("DAO backing field changed: " + pair.Key);
                field.SetValue(instance, pair.Value);
            }
            return instance;
        }

        private static InitializationFixture CreateInitializationFixture(string sandbox, string name, string gtin)
        {
            var root = Path.Combine(sandbox, "po-init-" + name);
            var metadata = Path.Combine(root, "metadata");
            var data = Path.Combine(root, "po-data");
            var codes = Path.Combine(root, "unused-codes");
            Directory.CreateDirectory(metadata);
            Directory.CreateDirectory(data);
            Directory.CreateDirectory(codes);
            SetSandboxPaths(root, metadata, data);
            var order = "INIT_" + name.Replace('-', '_').ToUpperInvariant();
            File.WriteAllText(Path.Combine(metadata, order + ".json"), JsonConvert.SerializeObject(new Dictionary<string, string>
            { { "orderNo", order }, { "productionDate", ProductionDate }, { "GTIN", gtin }, { "orderQty", "2" } }), new UTF8Encoding(false));
            var basePath = ProductionOrder.GetOrderBasePath(order);
            var expected = Path.Combine(data, "2026-11", gtin);
            if (!string.Equals(Path.GetFullPath(basePath), Path.GetFullPath(expected), StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Initialization fixture escaped its sandbox.");
            return new InitializationFixture { Name = name, Order = order, BasePath = basePath, Database = Path.Combine(basePath, order + ".db"), MesJson = Path.Combine(codes, "GTIN_" + gtin + ".json") };
        }

        private static void WriteMesCodes(InitializationFixture fixture, int count)
        {
            var codes = new JArray(Enumerable.Range(1, count).Select(index => new JObject
            { ["code"] = fixture.Order + "_" + index.ToString("D4"), ["createdAt"] = "2026-11-01T00:00:00Z", ["blockNo"] = 0 }));
            var json = new JObject { ["blocks"] = new JObject { ["0"] = new JObject { ["codes"] = codes } } };
            File.WriteAllText(fixture.MesJson, json.ToString(Formatting.None), new UTF8Encoding(false));
        }

        private static long RowCount(string path)
        {
            if (!File.Exists(path)) return 0;
            using (var connection = Open(path))
            using (var command = new SQLiteCommand("SELECT COUNT(*) FROM UniqueCodes;", connection)) return Convert.ToInt64(command.ExecuteScalar());
        }

        private static string[] InitializationRows(string path)
        {
            var rows = new List<string>();
            using (var connection = Open(path))
            using (var command = new SQLiteCommand("SELECT ID,Code,Status,cartonCode,SubCamera_ActivateDate FROM UniqueCodes ORDER BY ID;", connection))
            using (var reader = command.ExecuteReader())
                while (reader.Read()) rows.Add(string.Join("|", Enumerable.Range(0, 5).Select(index => reader.GetValue(index).ToString())));
            return rows.ToArray();
        }

        private sealed class InitializationFixture { public string Name, Order, BasePath, Database, MesJson; }
    }
}
