using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Path = System.IO.Path;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

// Metadata-only wrappers used solely by linked POService; no production settings/UI.
namespace Sunny.UI { public sealed class UIComboBox { public object DataSource; public string DisplayMember, ValueMember; } }
namespace QR_MASAN_01
{
    internal class FixtureUser { public string Username = "fixture"; }
    internal static class Globalvariable { public static FixtureUser CurrentUser = new FixtureUser(); }
    internal class Setting { public static Setting Current = new Setting(); public string PO_Data_path = "UNUSED"; }
}

internal static class Round2Tests
{
    private static string root;
    private static void Check(bool ok, string message) { if (!ok) throw new Exception(message); }
    internal static void Run(string directory, Action<string, Action> run)
    {
        root = directory;
        run("R2 named-pipe and TCP bridge lifetime", Round2Transport.Run);
        run("R2 native COM fake-adapter lifetime", Round2Native.Run);
        run("R2 camera simulator isolated lifecycle CSV", () => Round2Simulator.Run(root));
        run("R2 recipe CS namespace", Recipe);
        run("R2 PO paths history AWS and transaction", PO);
        run("R2 audit NULL shift and concurrent queues", Audit);
        run("R2 CSV roundtrip and atomic imports", CSV);
        run("R2 random capacity bounds", Random);
        run("R2 hourly chart grouping", Hours);
        run("R2 final PDF hash and report type", PDF);
        run("R2 memory COM subscriptions and prefix", Memory);
        run("R2 scanner initialization and selected port", Scanner);
        run("R2 PLC statistics mapping", Statistics);
        run("R2 chart deployed template and temp cleanup", Charts);
    }
    private static SQLiteConnection Open(string file)
    {
        var c = new SQLiteConnection(new SQLiteConnectionStringBuilder { DataSource = file, Version = 3 }.ConnectionString);
        c.Open(); return c;
    }
    private static void SQL(SQLiteConnection c, string text) { using (var cmd = new SQLiteCommand(text, c)) cmd.ExecuteNonQuery(); }
    private static object Scalar(string file, string text) { using (var c = Open(file)) using (var cmd = new SQLiteCommand(text, c)) return cmd.ExecuteScalar(); }
    private static void Recipe()
    {
        string cwd = Directory.GetCurrentDirectory(); string folder = Path.Combine(root, "recipes"); Directory.CreateDirectory(folder);
        try
        {
            Directory.SetCurrentDirectory(folder); Directory.CreateDirectory("PLC_RECIPEs"); Directory.CreateDirectory("PLC_RECIPEs_CS");
            var f = new DesktopFixtures.RecipeFixture(); f.CreateLogTable(f.log_FilePath); f.CreateLogTable(f.log_FilePath_CS);
            File.WriteAllText("PLC_RECIPEs/rear.rplc", "front sentinel");
            f.Write_Recipe_To_File_CS("rear data");
            Check(File.ReadAllText("PLC_RECIPEs/rear.rplc") == "front sentinel", "CS overwrote front recipe");
            Check(File.ReadAllText("PLC_RECIPEs_CS/rear.rplc") == "rear data", "CS wrong directory");
            Check(Convert.ToInt32(Scalar(f.log_FilePath, "SELECT COUNT(*) FROM Log")) == 0, "CS wrote front log");
            Check(Convert.ToString(Scalar(f.log_FilePath_CS, "SELECT RecipeName FROM Log")) == "rear.rplc", "CS wrong log name");
            f.SelectRecipeName_CS = "missing.rplc"; f.Check_Recipe_CS(f.SelectRecipeName_CS, f.defaultConfig_CS);
            Check(File.Exists("PLC_RECIPEs_CS/missing.rplc") && !File.Exists("PLC_RECIPEs/missing.rplc"), "CS missing fallback wrong directory");
            f.NewRecipe(); Check(f.SelectRecipeName_CS == "Round2.rplc" && Convert.ToString(f.ipRecipe_CS.SelectedItem) == "Round2.rplc", "CS new recipe selection wrong");
            Check(f.ipRecipe_CS.Items.Contains("Round2.rplc"), "CS item extension missing");
            f.CreateDefaultRecipeTable_CS(); Check(File.Exists(f.defaultFilePath_CS), "CS default absent");
        }
        finally { Directory.SetCurrentDirectory(cwd); }
    }
    private static void PO()
    {
        string mes = Path.Combine(root, "mes"); string codes = Path.Combine(mes, "codes"); string run = Path.Combine(root, "run");
        Directory.CreateDirectory(codes); string history = Path.Combine(root, "history", "PO.tlog");
        using (var c = Open(Path.Combine(codes, "PO1.db"))) { SQL(c, "CREATE TABLE UniqueCodes(Code TEXT); INSERT INTO UniqueCodes VALUES('ONE'),('TWO');"); }
        var service = new QR_MASAN_01.POService(mes, run, history); service.RunPO("PO1", "2026-11-27");
        Check(service.Get_CZRun_Count("PO1") == 2, "MES path joining/run import failed");
        service.Insert_Record_AWS("PO1", "m1", "ONE", "1", "activate", "date", "thing", "sent");
        service.Insert_Record_Recive_AWS("PO1", "m1", "ok", "", "received");
        Check(Convert.ToInt32(Scalar(Path.Combine(run, "Record_PO1_Send_AWS.db"), "SELECT COUNT(*) FROM Records")) == 1, "AWS send routed wrong DB");
        Check(Convert.ToInt32(Scalar(Path.Combine(run, "Record_PO1_Recive_AWS.db"), "SELECT COUNT(*) FROM Records")) == 1, "AWS receive routed wrong DB");
        string reason = "operator's \"reason\"\nline"; service.Delete_PO("PO1", reason);
        string json = Convert.ToString(Scalar(history, "SELECT Counter FROM PO WHERE Action='DELETE'"));
        Check(JsonConvert.DeserializeObject<Dictionary<string, string>>(json)["lydo"] == reason, "delete reason not parameterized JSON");
        using (var c = Open(Path.Combine(run, "PO1.db"))) SQL(c, "UPDATE UniqueCodes SET Status=1,Recive_Status='ok',Send_Status=CASE Code WHEN 'ONE' THEN 'Sent' ELSE 'sent' END");
        Check(service.Get_Unique_Codes_Run_Sent_Recive_OK("PO1").Rows.Count == 2, "Sent status query case mismatch");
        Check(service.get.Get_Unique_Codes_Run_Send_Count("PO1") == 2, "Sent status count case mismatch");
        using (var c = Open(Path.Combine(codes, "BAD.db"))) SQL(c, "CREATE TABLE UniqueCodes(Code TEXT); INSERT INTO UniqueCodes VALUES('DUP'),('DUP');");
        bool failed = false; try { service.Check_Run_File("BAD"); } catch (SQLiteException) { failed = true; }
        Check(failed && Convert.ToInt32(Scalar(Path.Combine(run, "BAD.db"), "SELECT COUNT(*) FROM UniqueCodes")) == 0, "PO import partially committed");
    }
    private static void Audit()
    {
        string system = Path.Combine(root, "system.db"), active = Path.Combine(root, "active.db"), aws = Path.Combine(root, "aws.db"), production = Path.Combine(root, "production.db");
        QR_MASAN_01.SystemLogs.InsertToSQLite(new QR_MASAN_01.SystemLogs("time", 1, QR_MASAN_01.SystemLogs.e_LogType.SYSTEM, "message"), system);
        QR_MASAN_01.ActiveLogs.ActiveInsertToSQLite(new QR_MASAN_01.ActiveLogs("time", 1, QR_MASAN_01.ActiveLogs.e_ActiveLogType.CONNECT, "message"), active);
        QR_MASAN_01.AWSLogs.AWSInsertToSQLite(new QR_MASAN_01.AWSLogs("time", 1, QR_MASAN_01.AWSLogs.e_AWSLogType.CONNECT, "message"), aws);
        foreach (var pair in new[] { Tuple.Create(system, "SystemLogs"), Tuple.Create(active, "ActiveLogs"), Tuple.Create(aws, "AWSLogs") })
            Check(Convert.ToInt32(Scalar(pair.Item1, "SELECT COUNT(*) FROM " + pair.Item2 + " WHERE User='' AND Details=''")) == 1, "optional NULL violates original schema");
        var log = new QR_MASAN_01.ProductionLogs("time", 1, "PO", "code", "factory", "site", "line", "date", 1, 2);
        QR_MASAN_01.ProductionLogs.InsertToSQLite(log, production); log.shift = "night"; QR_MASAN_01.ProductionLogs.InsertToSQLite(log, production);
        Check(Convert.ToInt32(Scalar(production, "SELECT COUNT(*) FROM ProductionLogs WHERE shift IN ('','night') AND User IS NULL AND Details IS NULL")) == 2, "production shift/default NULL semantics wrong");
        Parallel.For(0, 1000, i => QR_MASAN_01.SystemLogs.LogQueue.Enqueue(new QR_MASAN_01.SystemLogs("time", i, QR_MASAN_01.SystemLogs.e_LogType.SYSTEM, "message")));
        var seen = new HashSet<long>(); while (QR_MASAN_01.SystemLogs.LogQueue.TryDequeue(out var entry)) Check(seen.Add(entry.TimeUnix), "duplicate queue entry");
        Check(seen.Count == 1000, "concurrent queue loss");
    }
    private static void CSV()
    {
        var table = new DataTable(); table.Columns.Add("header,\"quote\"", typeof(string)); table.Columns.Add("second", typeof(string));
        table.Rows.Add("comma,quote\"\r\nUnicode tiếng Việt", DBNull.Value);
        string a = Path.Combine(root, "csv-a.csv"), b = Path.Combine(root, "csv-b.csv");
        Check(SpT.CsvHelper.ExportDataTableToCsv(table, a).IsSucces, "SpT CSV export failed");
        using (var exporter = new SPMS1.CSV_Extention()) exporter.ExportDataTableToCsv(table, root, "csv-b.csv");
        foreach (string file in new[] { a, b }) using (var parser = new TextFieldParser(file))
        {
            parser.SetDelimiters(","); parser.HasFieldsEnclosedInQuotes = true; parser.TrimWhiteSpace = false;
            Check(parser.ReadFields()[0] == table.Columns[0].ColumnName, "CSV header corrupted");
            var values = parser.ReadFields(); Check(values[0] == Convert.ToString(table.Rows[0][0]) && values[1] == "", "CSV data corrupted");
        }
        Check(SpT.CsvHelper.ExportDataTableToCsv(new DataTable(), Path.Combine(root, "empty.csv")).IsSucces, "zero column CSV failed");
        string input = Path.Combine(root, "import.csv"); File.WriteAllText(input, "NEW1\nEXISTING\nNEW2");
        foreach (string variant in new[] { "UnoV7", "Unoplus" })
        {
            string db = Path.Combine(root, "import-" + variant + ".db"); using (var c = Open(db)) SQL(c, "CREATE TABLE CaseQRContent(CaseQR TEXT UNIQUE); INSERT INTO CaseQRContent VALUES('EXISTING');");
            Func<string, int> import = variant == "UnoV7" ? (Func<string, int>)new DesktopFixtures.ImportUnoV7 { _DBFILEPATH = db }.ImportCsv : new DesktopFixtures.ImportUnoplus { _DBFILEPATH = db }.ImportCsv;
            bool failed = false; try { import(input); } catch (SQLiteException) { failed = true; }
            Check(failed && Convert.ToInt32(Scalar(db, "SELECT COUNT(*) FROM CaseQRContent")) == 1, "CSV import partially committed " + variant);
            File.WriteAllText(input, " NEW1 \n\nNEW2"); Check(import(input) == 2, "CSV import valid count wrong"); File.WriteAllText(input, "NEW1\nEXISTING\nNEW2");
        }
    }
    private static void Random()
    {
        using (var generator = new SPMS1.RandomStringGenerator())
        using (var done = new ManualResetEventSlim())
        {
            int errors = 0; DataTable result = null;
            generator.ErrorOccurred += (sender, ex) => Interlocked.Increment(ref errors);
            generator.Completed += (sender, data) => { result = data; done.Set(); };
            generator.GenerateRandomStrings(1, 2, "aa"); generator.GenerateRandomStrings(0, 2, "AB"); generator.GenerateRandomStrings(-1, 1); generator.GenerateRandomStrings(1, -1);
            Check(errors == 4, "impossible requests not rejected");
            generator.GenerateRandomStrings(2, 4, "AB"); Check(done.Wait(5000), "valid generation stalled");
            Check(result.Rows.Count == 4 && result.Rows.Cast<DataRow>().Select(row => (string)row[0]).Distinct().Count() == 4, "valid generator incorrect");
        }
    }
    private static void Hours()
    {
        string db = Path.Combine(root, "hours.db"); using (var c = Open(db)) SQL(c, "CREATE TABLE Records(ActivateDate TEXT); INSERT INTO Records VALUES('2026-11-27 03:20:00'),('2026-11-27 02:10:00'),('2026-11-27 03:59:59');");
        string html = new SpT.DataSCADA.ChartHelper(root).GenerateHtmlFromSQLite("hours");
        Check(html.Contains("data: [1,2]") && html.IndexOf("02:00:00") < html.IndexOf("03:00:00"), "hour grouping/sorting wrong");
    }
    private static void PDF()
    {
        var table = new DataTable(); table.Columns.Add("Fixture"); table.Rows.Add("ROUND2-PAYLOAD");
        string file = Path.Combine(root, "report.pdf"); var report = QR_MASAN_01.Report.ReportClass.ExportReportToPDF(table, file, "ROUND2-TYPE");
        using (var sha = SHA256.Create()) using (var stream = File.OpenRead(file))
            Check(report.HashCode.Equals(BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", ""), StringComparison.OrdinalIgnoreCase), "returned hash differs from final PDF");
        using (var reader = new PdfReader(file))
        {
            string text = string.Join("\n", Enumerable.Range(1, reader.NumberOfPages).Select(page => PdfTextExtractor.GetTextFromPage(reader, page)));
            Check(text.Contains("ROUND2-TYPE") && text.Contains("ROUND2-PAYLOAD") && !text.Contains("LOGIN") && !text.Contains("Tạm tính"), "PDF metadata/content overwritten");
        }
        using (File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
        var broken = new DataTable(); broken.Columns.Add("Fixture", typeof(object)); broken.Rows.Add(new BrokenPdfValue());
        string failed = Path.Combine(root, "failed-report.pdf"); bool threw = false;
        try { QR_MASAN_01.Report.ReportClass.ExportReportToPDF(broken, failed, "FAILURE"); } catch (Exception) { threw = true; }
        Check(threw, "PDF failure fixture did not throw");
        using (File.Open(failed, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
    }
    private sealed class BrokenPdfValue { public override string ToString() { throw new InvalidOperationException("fixture PDF value"); } }
    private static void Memory()
    {
        var a = new TestApp.VirtualCOMManager.VirtualCOMPortMemory("A"); var b = new TestApp.VirtualCOMManager.VirtualCOMPortMemory("B"); a.ConnectedPort = b; b.ConnectedPort = a;
        int notifications = 0;
        for (int i = 0; i < 10; i++) using (var bridge = new TestApp.MemoryCOMBridge(a, b, text => notifications++))
        { a.Write(new byte[] { 7 }); Check(b.Read().Single() == 7 && a.Read() == null, "memory bridge echoes/recurses"); }
        Check(notifications == 10, "memory bridge reconnect accumulates handlers"); a.Open(); b.Open(); a.Write(new byte[] { 8 }); Check(notifications == 10, "disposed binding still receives");
        Check(TestApp.VirtualCOMManager.IsVirtualPortEntry("VCP_COM1_guid", "COM1") && !TestApp.VirtualCOMManager.IsVirtualPortEntry("VCP_COM10_guid", "COM1") && !TestApp.VirtualCOMManager.IsVirtualPortEntry("VCP_COM1_guid", ""), "device prefix matches unrelated port");
    }
    private static void Scanner()
    {
        var f = new DesktopFixtures.ScanFixture(); f.INIT(); f.INIT(); Check(f._ScanConection.Loads == 1 && f._ScanConection.Callbacks == 1, "scanner init duplicates"); f.Close(); f.Close(); Check(f._ScanConection.Callbacks == 0 && f._ScanConection.Disconnects == 1, "scanner cleanup duplicates");
        var d = new DesktopFixtures.DialogScannerFixture(); d.Load(); Check(d._ScanConection.Connects.Single() == "COM7", "selected port or init order wrong"); d.ipCOM.SelectedItem = null; d.Select(); Check(d._ScanConection.Connects.Count == 1, "null selection connected"); d.Close();
    }
    private static void Statistics()
    {
        var f = new DesktopFixtures.StatisticsFixture(); f.Fill(Enumerable.Range(0, 160).Select(i => 2000 + 2 * i).ToArray());
        for (int row = 0; row < 16; row++) for (int col = 1; col <= 5; col++) Check(Convert.ToInt32(f.uiDataGridView1.Rows[row].Cells[col].Value) == 2000 + row * 10 + (col - 1) * 2, "D-word index mismatch");
    }
    private static void Charts()
    {
        DesktopFixtures.Application.StartupPath = root; string templates = Path.Combine(root, "ChartCS"); Directory.CreateDirectory(templates);
        var f = new DesktopFixtures.ViewerFixture { Option = new { title = "literal $& </script> tiếng Việt" } };
        File.WriteAllText(Path.Combine(templates, "bar-label-rotation.html"), "<head></head><script>var option={{ECHARTS_OPTION}};</script>");
        string html = f.Render(File.ReadAllText(f.PathValue())); Check(!html.Contains("{{ECHARTS_OPTION}}") && !html.Contains("$& </script>") && html.Contains("\\u003c"), "chart script escaping failed");
        string legacy = f.Render("<head></head><script>option = {}\n;</script>");
        var json = System.Text.RegularExpressions.Regex.Match(legacy, @"option\s*=\s*(\{[\s\S]*\})\s*;");
        Check(json.Success && (string)Newtonsoft.Json.Linq.JObject.Parse(json.Groups[1].Value)["title"] == "literal $& </script> tiếng Việt", "legacy newline terminator not replaced/literal changed");
        f.LoadChart(); string first = f._tempHtmlFile; f.LoadChart(); string second = f._tempHtmlFile; Check(first != second, "chart temp collision"); f.Close(); Check(!File.Exists(first) && !File.Exists(second), "chart leaked temp files");
    }
}
