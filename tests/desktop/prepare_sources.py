"""Build narrow fixtures from unchanged production method/row-loop bodies.

UI/transport wrappers are test doubles. Extracted statements are never edited.
"""
import hashlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = Path(__file__).resolve().parent / "obj" / "ExtractedFixtures.g.cs"
digests = {}


def body(relative, signature, marker=None):
    path = ROOT / relative
    source = path.read_text(encoding="utf-8-sig")
    digests[relative] = hashlib.sha256(path.read_bytes()).hexdigest()
    assert source.count(signature) == 1, signature
    start = source.index(signature)
    if marker:
        start = source.index(marker, start)
    opening = source.index("{", start)
    depth = 1
    end = opening + 1
    while depth:
        depth += (source[end] == "{") - (source[end] == "}")
        end += 1
    return source[start:end]


parts = ["using System; using System.Data; using System.Data.SQLite; using System.Collections.Generic; using System.IO; using System.Linq; using System.Text.RegularExpressions; using Newtonsoft.Json; namespace DesktopFixtures {"]
parts += ["""
internal class ProductData {
 public int ProductID, Active;
 public string TimeStampActive, TimeStampPrinted;
 public long TimeUnixActive, TimeUnixPrinted;
}
internal class FixtureUser { public string Username="fixture"; }
internal static class Globalvariable {
 public static FixtureUser CurrentUser = new FixtureUser();
 public static Dictionary<string, ProductData> Main_Content_Dictionary = new Dictionary<string, ProductData>();
 public static Dictionary<string, ProductData> C1_Content_Dictionary = new Dictionary<string, ProductData>();
 public static Dictionary<string, ProductData> C2_Content_Dictionary = new Dictionary<string, ProductData>();
}
internal class FakeSender {
 public readonly List<string> Sent = new List<string>();
 public void SEND_DYNAMIC_DATA_TM1_REMOTE(string data, ulong id) { Sent.Add(data); }
}
internal class PrinterBase {
 internal int INDEX, Old, BUFFER, SetBuffer = 30;
 internal readonly List<string> DATA = new List<string>();
 internal readonly FakeSender sendPrinter = new FakeSender();
 protected void UpdateCounter() { }
}
"""]
for suffix in ("", "_C1", "_C2"):
    loop = body("QR-MASAN-01/Views/F1Dashboard.cs", "public void Push_Data_To_Dic" + suffix + "()", "foreach (DataRow row in dataTable.Rows)")
    parts.append("internal static class Loader" + (suffix or "Main") + " { internal static void Load(DataTable dataTable) {\n" + loop + "\n} }")
for variant in ("UnoV7", "Unoplus"):
    method = body("PRINTER/" + variant + "/Norwix/uc_Nowix_vip.cs", "public void Send1Data()")
    parts.append("internal class Printer" + variant + " : PrinterBase {\n" + method + "\n}")

# ROUND2: unchanged authored statements, with narrow managed wrappers.
parts.extend(["using_placeholder"] if False else [])
for variant in ("UnoV7", "Unoplus"):
    method = body("PRINTER/" + variant + "/SQLite/SqlLite_Lib.cs", "public int ImportCsv(")
    parts.append("internal class Import" + variant + " { public string _DBFILEPATH;\n" + method + "\n}")
recipe_fields = """
internal class FakeText { public string Text; public object SelectedItem; public string SelectedText = ""; public List<object> Items = new List<object>(); }
internal class Entertext : IDisposable {
 public static string NextName = "Round2";
 public string TileText, TextValue;
 public event EventHandler EnterClicked;
 public void ShowDialog() { TextValue = NextName; EnterClicked?.Invoke(this, EventArgs.Empty); }
 public void Dispose() { }
}
internal class RecipeFixture {
 public string SelectRecipeName = "front.rplc", SelectRecipeName_CS = "rear.rplc";
 public string log_FilePath="PLC_RECIPEs/log.rlplc", log_FilePath_CS="PLC_RECIPEs_CS/log.rlplc";
 public string defaultFilePath_CS="PLC_RECIPEs_CS/Default.rplc";
 public PLC_Parameter defaultConfig_CS = new PLC_Parameter { DelayCamera="1",DelayReject="2",RejectStreng="3" };
 public PLC_Parameter PLC_Parameter_On_PC_CS = new PLC_Parameter();
 public FakeText ipDelayTriger_CS=new FakeText(),ipDelayReject_CS=new FakeText(),ipRejectStreng_CS=new FakeText(),ipRecipe_CS=new FakeText();
 public void InvokeIfRequired(Action action) { action(); }
 public void ShowErrorNotifier(string text) { throw new Exception(text); }
 public void ShowSuccessNotifier(string text) { }
 public void NewRecipe() { btnNewCS_Click(null,EventArgs.Empty); }
"""
parts.append(recipe_fields)
for signature in ["public class PLC_Parameter", "public void CreateLogTable(", "public void AddLogRecipe(", "public void AddLogRecipe_CS(", "public void CreateDefaultRecipeTable_CS()", "public void Check_Recipe_CS(", "public void Write_Recipe_To_File_CS(", "private void btnNewCS_Click("]:
    parts.append(body("QR-MASAN-01/Views/Settings/PLCSetting.cs", signature))
parts.append("}")
parts.append("""
internal enum e_Serial { Connected, Disconnected, Recive }
internal class Connection {
 public object SERIALPORT; public int Loads,Disconnects,Callbacks;
 public readonly List<string> Connects=new List<string>();
 private Action<e_Serial,string> callback;
 public event Action<e_Serial,string> EVENT { add { callback+=value; Callbacks++; } remove {callback-=value;Callbacks--;} }
 public void LOAD(){ if(SERIALPORT==null)throw new Exception("load without serial");Loads++; }
 public void CONNECT(string port){if(Loads==0)throw new Exception("connect before load");Connects.Add(port);}
 public void DISCONNECT(){Disconnects++;}
}
internal class Setting { public static Setting Current = new Setting(); public string HandScanCOM="COM7"; }
internal class ScanFixture {
 public Connection _ScanConection=new Connection(); public object serialPort1=new object();
 private bool scannerInitialized,scannerDisposed;
 private void _ScanConection_EVENT(e_Serial e,string data) { }
 public void Close(){DisposeScanner();}
""")
for signature in ["public void INIT()", "private void DisposeScanner()"]:
    parts.append(body("QR-MASAN-01/Views/ScanQR_V2.cs",signature))
parts.append("}")
parts.append("""
internal class DialogScannerFixture {
 public Connection _ScanConection=new Connection(); public object serialPort1=new object();
 private bool scannerInitialized,scannerDisposed; public string _Title="test";
 public FakeText ipCOM=new FakeText(),uiTitlePanel1=new FakeText();
 public void ScanCOMPorts(){ipCOM.SelectedItem="COM7";ipCOM_SelectedIndexChanged(null,EventArgs.Empty);}
 private void _ScanConection_EVENT(e_Serial e,string data) { }
 public void Load(){Scaner_Load(null,EventArgs.Empty);} public void Select(){ipCOM_SelectedIndexChanged(null,EventArgs.Empty);}
 public void Close(){DisposeScanner();}
""")
for signature in ["private void Scaner_Load(","private void ipCOM_SelectedIndexChanged(","private void DisposeScanner()"]:
    parts.append(body("QR-MASAN-01/Dialogs/Scaner.cs",signature))
parts.append("}")
parts.append("""
internal static class Application { public static string StartupPath; }
internal enum MessageBoxButtons {OK} internal enum MessageBoxIcon {Warning,Error}
internal static class MessageBox { public static void Show(params object[] args){throw new Exception(Convert.ToString(args[0]));} }
internal class FakeWebView { public Uri Source; }
internal class FixtureControl { protected virtual void Dispose(bool disposing){} }
internal class ViewerFixture:FixtureControl {
 public string _templateFile="bar-label-rotation.html",_tempHtmlFile;
 public readonly List<string> _generatedHtmlFiles=new List<string>(); public FakeWebView webView=new FakeWebView();
 public object Option; private object CreateChartOption(){return Option;}
 public string PathValue(){return GetTemplatePath();} public string Render(string html){return ProcessTemplate(html);}
 public void Close(){Dispose(true);}
""")
for signature in ["public void LoadChart()","private string GetTemplatePath()","private string ProcessTemplate(","protected override void Dispose(bool disposing)"]:
    parts.append(body("TeraCharts/Barcharts/UcBarChartViewer.cs",signature))
parts.append("}")
parts.append("""
internal class Cell {public object Value;} internal class Row { public List<Cell> Cells=Enumerable.Range(0,6).Select(i=>new Cell()).ToList();}
internal class Grid {public List<Row> Rows=Enumerable.Range(0,16).Select(i=>new Row()).ToList();}
internal class StatisticsFixture {
 public Grid uiDataGridView1=new Grid();
 private List<int> baseAddresses=new List<int>{0,10,20,30,40,50,60,70,80,90,100,110,120,130,140,150};
 public void Fill(int[] values) {
""")
parts.append(body("QR-MASAN-01/Views/FStatistics.cs","private void btnReloadPLC_Click(","for (int row = 0; row < baseAddresses.Count; row++)"))
parts.append("} }")

parts.append("}")
OUTPUT.parent.mkdir(parents=True, exist_ok=True)
parts.append("namespace TestApp { using System.ComponentModel; using System.Net; using System.Net.Sockets; using System.Text; using System.Threading; using System.Threading.Tasks;")
parts.append(body("TestApp/CameraSimulatorForm.cs", "public class CameraInstance"))
parts.append(body("TestApp/CameraSimulatorForm.cs", "internal static class CameraSimulatorCore"))
parts.append("}")
header = "// Extracted from production sources. SHA256:\n" + "".join("// " + p + " " + sha + "\n" for p, sha in digests.items())
OUTPUT.write_text(header + "\n".join(parts), encoding="utf-8")
print("Prepared extracted printer methods and all three legacy loader row loops")
