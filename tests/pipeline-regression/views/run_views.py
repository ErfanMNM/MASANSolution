"""Exact-source offline V10/V14 regression. Execute only under CEO build slot."""
import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import sys
from datetime import datetime, timezone


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest().upper()


def block(source, signature):
    """Return one exact C# brace block, skipping comments and string literals."""
    if source.count(signature) != 1:
        raise ValueError(f"Expected one exact signature: {signature}")
    start = source.index(signature)
    opening = source.index("{", start + len(signature))
    depth, i, state = 0, opening, "code"
    while i < len(source):
        ch = source[i]
        nxt = source[i + 1] if i + 1 < len(source) else ""
        if state == "line":
            if ch == "\n": state = "code"
        elif state == "comment":
            if ch == "*" and nxt == "/": state = "code"; i += 1
        elif state == "verbatim":
            if ch == '"':
                if nxt == '"': i += 1
                else: state = "code"
        elif state in ("string", "char"):
            if ch == "\\": i += 1
            elif ch == ('"' if state == "string" else "'"): state = "code"
        else:
            if ch == "/" and nxt == "/": state = "line"; i += 1
            elif ch == "/" and nxt == "*": state = "comment"; i += 1
            elif ch == "@" and nxt == '"': state = "verbatim"; i += 1
            elif ch == '"': state = "string"
            elif ch == "'": state = "char"
            elif ch == "{": depth += 1
            elif ch == "}":
                depth -= 1
                if depth == 0: return source[start:i + 1]
        i += 1
    raise ValueError(f"Unclosed C# block: {signature}")


def main():
    parser = argparse.ArgumentParser(__doc__)
    parser.add_argument("--artifacts", required=True)
    parser.add_argument("--grant", required=True)
    parser.add_argument("--repo", default=str(Path(__file__).resolve().parents[3]))
    parser.add_argument("--msbuild", default=r"C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe")
    args = parser.parse_args()
    repo, out = Path(args.repo).resolve(), Path(args.artifacts).resolve()
    if out == repo or repo in out.parents:
        raise ValueError("Artifacts must remain outside the repository")
    out.mkdir(parents=True, exist_ok=False)
    aws_path = repo / "MASAN-SERIALIZATION/Views/AWS/PAwsIot.cs"
    stats_path = repo / "MASAN-SERIALIZATION/Views/SCADA/PStatictis.cs"
    source_hashes = {str(p): sha(p) for p in (aws_path, stats_path)}
    aws, stats = aws_path.read_text(encoding="utf-8-sig"), stats_path.read_text(encoding="utf-8-sig")
    receive = block(aws, "private void AddReceiveNotification(string message)")
    incoming = block(aws, "private void AWS_Status_OnReceive(object sender, AwsIotClientHelper.AWSStatusReceiveEventArgs e)")
    records = block(aws, "public void Insert_AWS_Receive_Record(") if "public void Insert_AWS_Receive_Record(" in aws else block(aws, "private void Insert_AWS_Receive_Record(")
    if aws.count("opRecive.Items.Insert(") != 1 or incoming.count("AddReceiveNotification(") != 1 or records.count("AddReceiveNotification(") != 2:
        raise ValueError("Receive producer coverage drift: inspect all receive-list writers")
    periodic = block(aws, "private void Bgw_process_DoWork(object sender, DoWorkEventArgs e)")
    periodic_trim = block(periodic, "while (opRecive.Items.Count > 50)")
    if "opNotiboardAndSend" in periodic_trim:
        raise ValueError("Periodic receive trim targets send list")
    hourly_class = block(stats, "public class HourlyChartData")
    hourly = block(stats, "public static HourlyChartData GetLast6Hours(string dbPath)")
    generated = ("using System;\nusing System.Linq;\nusing DateTime = ViewRegression.TestDateTime;\nnamespace ViewRegression {\n"
                 "public partial class ReceiveView {\n" + receive + "\npublic void PeriodicTrim() {\n" + periodic_trim + "\n}\n}\n"
                 "public partial class StatisticsView {\n" + hourly_class + "\n" + hourly + "\n}\n}\n")
    generated_path = out / "ProductionMethods.cs"
    generated_path.write_text(generated, encoding="utf-8")
    extraction = {"createdUtc": datetime.now(timezone.utc).isoformat(), "grant": args.grant, "sourceSHA256": source_hashes,
                  "generatedSHA256": sha(generated_path), "members": ["AddReceiveNotification", "Bgw_process_DoWork exact receive while-block", "HourlyChartData", "GetLast6Hours"],
                  "boundaries": "Fake receive/send lists; fake SQLite connection/command/reader captures SQL/bindings and returns configured rows; DateTime type alias is a deterministic clock/date value adapter. No production expression rewritten.",
                  "sourceGuards": "One receive Items.Insert writer; actual incoming calls helper once and receive-record success/failure twice; periodic block cannot name send list."}
    (out / "extraction.json").write_text(json.dumps(extraction, indent=2), encoding="utf-8")
    command = [args.msbuild, str(Path(__file__).with_name("ViewsRegression.csproj")), "/t:Build", "/p:Configuration=Release", "/p:Platform=x64",
               "/p:BuildProjectReferences=false", f"/p:FixtureArtifacts={out}", f"/p:GeneratedSource={generated_path}", "/m:1", "/nr:false", "/nologo", "/v:minimal"]
    build = subprocess.run(command, capture_output=True, text=True, errors="replace", cwd=repo)
    (out / "build.log").write_text(build.stdout + build.stderr, encoding="utf-8")
    run_exit = None
    if build.returncode == 0:
        run = subprocess.run([str(out / "bin/ViewsRegression.exe"), str(out / "results.json")], capture_output=True, text=True, errors="replace", cwd=out)
        (out / "run.log").write_text(run.stdout + run.stderr, encoding="utf-8")
        run_exit = run.returncode
    hashes_after = {str(p): sha(p) for p in (aws_path, stats_path)}
    provenance = {"command": command, "buildExitCode": build.returncode, "runExitCode": run_exit, "sourceUnchanged": source_hashes == hashes_after,
                  "sourceBefore": source_hashes, "sourceAfter": hashes_after, "completedUtc": datetime.now(timezone.utc).isoformat()}
    (out / "provenance.json").write_text(json.dumps(provenance, indent=2), encoding="utf-8")
    print(json.dumps(provenance))
    if build.returncode or run_exit != 0 or source_hashes != hashes_after: return 1
    result = json.loads((out / "results.json").read_text(encoding="utf-8-sig"))
    return 0 if result.get("passedGroups") == 9 and result.get("failedGroups") == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
