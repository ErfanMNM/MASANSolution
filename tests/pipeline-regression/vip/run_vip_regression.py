"""V12 safe exact-method regression. Execute only with an explicit CEO slot grant."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys

HERE = Path(__file__).resolve().parent
REPO = HERE.parent.parent.parent
MSBUILD = Path(r"C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe")


def digest(raw):
    return hashlib.sha256(raw).hexdigest().upper()


def write_json(path, value):
    path.write_text(json.dumps(value, indent=2, ensure_ascii=False) + "\n", encoding="utf-8")


def extract(raw, signature, following):
    text = raw.decode("utf-8-sig")
    declaration = r"^        " + re.escape(signature) + r"[ \t]*\r?$"
    if len(re.findall(declaration, text, re.MULTILINE)) != 1:
        raise ValueError("Missing or ambiguous method signature: " + signature)
    # Fixed reviewed indentation plus the following exact method declaration make
    # the method boundary explicit. SHA pinning rejects unreviewed source drift.
    pattern = declaration + r"\r?\n        \{.*?^        \}"
    matches = list(re.finditer(pattern, text, re.MULTILINE | re.DOTALL))
    if len(matches) != 1:
        raise ValueError("Missing or ambiguous method boundary: " + signature)
    match = matches[0]
    if not re.match(r"\s*" + re.escape(following), text[match.end():]):
        raise ValueError("Unexpected next method after: " + signature)
    return match.group(0), {"signature": signature, "start_line": text.count("\n", 0, match.start()) + 1,
                            "end_line": text.count("\n", 0, match.end()) + 1}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, default=REPO / "MASAN-SERIALIZATION/Views/Database/CheckVIP.cs")
    parser.add_argument("--expected-source-sha", required=True)
    parser.add_argument("--artifacts", type=Path, required=True)
    parser.add_argument("--msbuild", type=Path, default=MSBUILD)
    args = parser.parse_args()
    source = args.source.resolve()
    artifacts = args.artifacts.resolve()
    if artifacts == REPO or REPO in artifacts.parents:
        parser.error("All generated source/obj/bin/log/results must stay outside the checkout.")
    if not args.msbuild.is_file():
        parser.error("Verified MSBuild path required.")
    raw = source.read_bytes()
    if not re.fullmatch(r"[0-9A-Fa-f]{64}", args.expected_source_sha) or digest(raw) != args.expected_source_sha.upper():
        parser.error("Source SHA mismatch; actual " + digest(raw))
    process, span_process = extract(raw, "public void ProcessScannedCode(string Code)",
                                    "private void btnfind_Click(object sender, EventArgs e)")
    button, span_button = extract(raw, "private void btnfind_Click(object sender, EventArgs e)",
                                  "private void btnDeleteCarton_Click(object sender, EventArgs e)")
    artifacts.mkdir(parents=True, exist_ok=False)
    generated = artifacts / "CheckVipMethods.g.cs"
    generated.write_bytes(("using System;\nusing System.Data;\nusing System.Threading;\nusing System.Threading.Tasks;\n"
                           "namespace VipRegression\n{\n    internal sealed partial class CheckVIP\n    {\n"
                           + process + "\n" + button + "\n    }\n}\n").encode("utf-8"))
    fixtures = {name: digest((HERE / name).read_bytes())
                for name in ("Harness.cs", "VipRegression.csproj", "run_vip_regression.py")}
    metadata = {"source": str(source), "source_sha256": digest(raw), "fixtures": fixtures,
                "methods": [{**span_process, "sha256": digest(process.encode("utf-8"))},
                            {**span_button, "sha256": digest(button.encode("utf-8"))}],
                "generated_source_sha256": digest(generated.read_bytes()),
                "boundaries": "Only exact ProcessScannedCode/btnfind_Click; actual Task.Run, fake UI/DB/Globals; no app/database/network/device."}
    try:
        command = [str(args.msbuild), str(HERE / "VipRegression.csproj"), "/t:Build", "/nologo", "/nr:false", "/v:minimal",
                   "/p:Configuration=Release", "/p:Platform=x64", "/p:BuildProjectReferences=false",
                   "/p:FixtureArtifacts=" + str(artifacts), "/p:GeneratedSource=" + str(generated)]
        metadata["build_command"] = command
        write_json(artifacts / "metadata.json", metadata)
        with (artifacts / "build.log").open("w", encoding="utf-8") as log:
            result = subprocess.run(command, cwd=artifacts, stdout=log, stderr=subprocess.STDOUT, check=False)
        metadata["build_exit_code"] = result.returncode
        write_json(artifacts / "metadata.json", metadata)
        if result.returncode != 0:
            raise RuntimeError("Build failed: " + str(result.returncode))
        executable = artifacts / "bin/VipRegression.exe"
        metadata["executable_sha256"] = digest(executable.read_bytes())
        command = [str(executable), str(artifacts / "results.json")]
        metadata["run_command"] = command
        write_json(artifacts / "metadata.json", metadata)
        with (artifacts / "run.log").open("w", encoding="utf-8") as log:
            result = subprocess.run(command, cwd=artifacts, stdout=log, stderr=subprocess.STDOUT, check=False)
        metadata["run_exit_code"] = result.returncode
        metadata["source_sha256_after"] = digest(source.read_bytes())
        metadata["fixtures_after"] = {name: digest((HERE / name).read_bytes()) for name in fixtures}
        if metadata["source_sha256_after"] != metadata["source_sha256"] or metadata["fixtures_after"] != fixtures:
            raise RuntimeError("Source or fixture changed during execution.")
        if result.returncode != 0:
            raise RuntimeError("Regression failed: " + str(result.returncode))
        results = json.loads((artifacts / "results.json").read_text(encoding="utf-8-sig"))
        if results["case_count"] != 8 or results["passed"] != 8 or results["failed"] != 0:
            raise RuntimeError("Incomplete/failing fixture results.")
        metadata["passed"] = results["passed"]
    except Exception as error:
        metadata["failure"] = str(error)
        write_json(artifacts / "metadata.json", metadata)
        print(json.dumps(metadata, ensure_ascii=False))
        return 1
    write_json(artifacts / "metadata.json", metadata)
    print(json.dumps({"artifacts": str(artifacts), "source_sha256": metadata["source_sha256"],
                      "build_exit_code": metadata["build_exit_code"], "run_exit_code": metadata["run_exit_code"],
                      "passed": metadata["passed"]}, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    sys.exit(main())
