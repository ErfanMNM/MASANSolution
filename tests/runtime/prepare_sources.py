"""Extract production methods unchanged; replace only certificate file I/O.

All SDK implementations in this executable are in-memory fakes. No real MQTT,
PLC or Google assembly is referenced. Auth log/data adapters never touch a DB.
"""
import hashlib
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = Path(__file__).resolve().parent / "obj"
OUT.mkdir(exist_ok=True)
digests = {}


def source(relative):
    path = ROOT / relative
    digests[relative] = hashlib.sha256(path.read_bytes()).hexdigest()
    return path.read_text(encoding="utf-8-sig")


def method(text, signature):
    assert text.count(signature) == 1, signature
    start = text.index(signature)
    i = text.index("{", start)
    depth = 1
    end = i + 1
    while depth:
        depth += (text[end] == "{") - (text[end] == "}")
        end += 1
    return text[start:end]


aws = source("SpT/AWS/AwsMqttHelper.cs")
for statement in (
    "caCert = X509Certificate.CreateFromCertFile(rootCAPath);",
    "clientCert = new X509Certificate2(pfxPath, pfxPassword);",
):
    assert aws.count(statement) == 1, statement
    aws = aws.replace(statement, "// Fixture: certificate disk I/O omitted; fake MqttClient never uses certificates.")
(OUT / "AwsMqttHelper.g.cs").write_text(aws, encoding="utf-8")

auth = source("SpT/Auth/uc_UserSetting.cs")
wrapper = """
using System; using System.Threading.Tasks;
namespace RuntimeFixtures {
internal class AuthFixture {
 public UserData userData = new UserData();
 public string CurrentUserName, data_file_path;
 public bool IS2FAEnabled;
 public LogHelper<LoginAction> log;
 public TextControl ipUserName=new TextControl(),ipOldPassword=new TextControl(),
 ipNewPassword=new TextControl(),ipComfirmNewPassword=new TextControl(),
 opRole=new TextControl(),ipOTP=new TextControl(),btnSave=new TextControl();
 public event EventHandler<LoginActionEventArgs> OnUserAction;
 public void Save() { btnSave_Click(this,EventArgs.Empty); }
"""
wrapper += method(auth, "public void INIT()") + "\n"
wrapper += method(auth, "private void btnSave_Click(object sender, EventArgs e)")
wrapper += "\n}}\n"
(OUT / "AuthMethods.g.cs").write_text(wrapper, encoding="utf-8")
for path in ["SPMS1/GoogleStorage.cs", "SPMS1/GoogleStorage.Designer.cs",
             "SPMS1/Socket_Server_TCP.cs", "SPMS1/Socket_Server_TCP.Designer.cs",
             "SPMS1/OmronPLC_Hsl.cs", "SPMS1/OmronPLC_Hsl.Designer.cs",
             "SpT/PLC/OMRON/OmronPLC_Hsl.cs", "SpT/PLC/OMRON/OmronPLC_Hsl.Designer.cs"]:
    source(path)
(OUT / "source-sha256.json").write_text(json.dumps(digests, indent=2), encoding="utf-8")
print("Prepared unchanged runtime source/methods; certificate I/O only bypassed; SDKs fake")
