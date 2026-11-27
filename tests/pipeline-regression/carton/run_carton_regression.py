"""Run only under the shared CEO build/test slot. No app, DB, network or device startup."""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import sys
from datetime import datetime, timezone


def sha(path):
    return hashlib.sha256(Path(path).read_bytes()).hexdigest().upper()


def segment(text, start, end):
    if text.count(start) != 1 or text.count(end) != 1:
        raise RuntimeError("Missing or ambiguous production boundary: " + start)
    a, b = text.index(start), text.index(end)
    if b <= a:
        raise RuntimeError("Invalid production boundary order")
    return text[a:b].rstrip()


def generate(source, globals_source, destination, mode):
    text = source.read_text(encoding="utf-8-sig")
    methods = segment(text, "        private void HandScan02_Process(string s)", "        private void btnSend2_Click(object sender, EventArgs e)")
    if methods.count("private void HandScan01_Process(string s)") != 1:
        raise RuntimeError("Expected exactly one lane-01 method")
    helper = "private bool TryAssignCartonCode(string code, string capturedOrderNo, bool firstLane, out bool duplicate)"
    if mode == "current" and methods.count(helper) != 1:
        raise RuntimeError("Current atomic helper missing or ambiguous")
    if mode == "before" and helper in methods:
        raise RuntimeError("Before fixture unexpectedly already contains atomic fix")
    globals_text = globals_source.read_text(encoding="utf-8-sig")
    snapshot = segment(globals_text, "        public static ProductionCartonData SnapshotCarton(ProductionCartonData value)", "        public static void EnqueueCartonActivation(")
    dto = segment(globals_text, "    public class ProductionCartonData", "    #endregion\n\n    #region Các couter lưu tạm")
    generated = "using System;\nusing System.Linq;\nnamespace CartonRegression\n{\npartial class CartonHarness\n{\n" + methods + "\n}\nstatic partial class Globals_Database\n{\n" + snapshot + "\n}\n" + dto + "\n}\n"
    destination.write_text(generated, encoding="utf-8")
    return {"source": str(source), "sourceSHA256": sha(source), "globalsSource": str(globals_source), "globalsSHA256": sha(globals_source), "generatedSource": str(destination), "generatedSHA256": sha(destination), "methods": ["HandScan02_Process", "HandScan01_Process"] + (["TryAssignCartonCode"] if mode == "current" else []), "sharedExact": ["SnapshotCarton", "ProductionCartonData"], "boundaries": ["fake in-memory Globals/dictionary/queue", "fake log and synchronous UI dispatcher", "two-reader snapshot/barrier schedules an unprotected read race", "no application/DB/network/device"]}


def invoke(command, log, cwd):
    with log.open("w", encoding="utf-8") as handle:
        result = subprocess.run(command, cwd=str(cwd), stdout=handle, stderr=subprocess.STDOUT, timeout=90)
    return result.returncode


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--artifacts", required=True)
    parser.add_argument("--grant", required=True)
    parser.add_argument("--before-source", required=True)
    parser.add_argument("--msbuild", default=r"C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe")
    args = parser.parse_args()
    if not args.grant.strip():
        raise RuntimeError("An actual CEO slot grant description is required")
    here = Path(__file__).resolve().parent
    repo = here.parents[2]
    artifacts = Path(args.artifacts).resolve()
    if artifacts == repo or repo in artifacts.parents:
        raise RuntimeError("Artifacts must be outside the checkout")
    if artifacts.exists() and any(artifacts.iterdir()):
        raise RuntimeError("Choose a fresh artifact directory; previous results are not overwritten")
    artifacts.mkdir(parents=True, exist_ok=True)
    source = repo / "MASAN-SERIALIZATION/Views/Dashboards/PCartonDashboard.cs"
    globals_source = repo / "MASAN-SERIALIZATION/Infrastructure/Globals.cs"
    source_before = sha(source)
    result = {"startedUTC": datetime.now(timezone.utc).isoformat(), "grant": args.grant, "repo": str(repo), "project": str(here / "CartonAssignmentRegression.csproj"), "runs": []}
    failed = False
    for mode, selected in [("before", Path(args.before_source).resolve()), ("current", source)]:
        target = artifacts / mode
        target.mkdir()
        generated = target / "Production.Extracted.cs"
        provenance = generate(selected, globals_source, generated, mode)
        (target / "extraction.json").write_text(json.dumps(provenance, indent=2, ensure_ascii=False), encoding="utf-8")
        command = [args.msbuild, str(here / "CartonAssignmentRegression.csproj"), "/t:Build", "/p:Configuration=Release", "/p:Platform=x64", "/p:BuildProjectReferences=false", "/p:FixtureArtifacts=" + str(target), "/p:GeneratedSource=" + str(generated), "/m:1", "/nr:false", "/nologo", "/v:minimal"]
        build_exit = invoke(command, target / "build.log", repo)
        row = {"mode": mode, "provenance": provenance, "buildCommand": command, "buildExit": build_exit, "runExit": None}
        if build_exit == 0:
            executable = target / "bin/CartonAssignmentRegression.exe"
            run_command = [str(executable), mode, str(target / "results.json")]
            row["runCommand"] = run_command
            row["runExit"] = invoke(run_command, target / "run.log", repo)
            row["executableSHA256"] = sha(executable)
            if (target / "results.json").exists():
                row["results"] = json.loads((target / "results.json").read_text(encoding="utf-8-sig"))
        result["runs"].append(row)
        failed = failed or build_exit != 0 or row["runExit"] != 0
        if failed:
            break
    result["completedUTC"] = datetime.now(timezone.utc).isoformat()
    result["productionSourceBeforeSHA256"] = source_before
    result["productionSourceAfterSHA256"] = sha(source)
    result["sourceStable"] = source_before == sha(source)
    failed = failed or not result["sourceStable"]
    result["exitCode"] = int(failed)
    (artifacts / "summary.json").write_text(json.dumps(result, indent=2, ensure_ascii=False), encoding="utf-8")
    print(json.dumps({"artifacts": str(artifacts), "exitCode": int(failed), "sourceStable": result["sourceStable"], "runs": [{"mode": row["mode"], "buildExit": row["buildExit"], "runExit": row["runExit"], "results": row.get("results")} for row in result["runs"]]}, ensure_ascii=False))
    return int(failed)


if __name__ == "__main__":
    sys.exit(main())
