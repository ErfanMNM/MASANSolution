"""Granted-slot runner: exact RenderOrderInfo extraction, isolated net48 build/run.

Preparing this file does not execute extraction or any production code. Run only
after the CEO grants the shared build/test slot for this specific fixture.
"""

import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys


HERE = Path(__file__).resolve().parent
REPO = HERE.parent.parent
DEFAULT_MSBUILD = Path(r"C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe")


def sha256(data):
    return hashlib.sha256(data).hexdigest().upper()


def dump(path, value):
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def extract_method(source_bytes):
    """Keep the actual method text unchanged; fail closed on signature/boundary drift."""
    source = source_bytes.decode("utf-8-sig")
    matches = list(re.finditer(r"^[ \t]*public void RenderOrderInfo\(\)[ \t]*\r?$", source, re.MULTILINE))
    if len(matches) != 1:
        raise ValueError("Expected exactly one public void RenderOrderInfo() declaration.")
    start = matches[0].start()
    cursor = matches[0].end()
    while cursor < len(source) and source[cursor].isspace():
        cursor += 1
    if cursor == len(source) or source[cursor] != "{":
        raise ValueError("RenderOrderInfo body opening brace missing.")
    depth = 0
    state = "code"
    opening = cursor
    while cursor < len(source):
        char = source[cursor]
        pair = source[cursor:cursor + 2]
        if state == "line-comment":
            if char == "\n":
                state = "code"
        elif state == "block-comment":
            if pair == "*/":
                state = "code"
                cursor += 1
        elif state in ("string", "char"):
            if char == "\\":
                cursor += 1
            elif char == ('"' if state == "string" else "'"):
                state = "code"
        elif state == "verbatim-string":
            if pair == '""':
                cursor += 1
            elif char == '"':
                state = "code"
        else:
            if pair == "//":
                state = "line-comment"
                cursor += 1
            elif pair == "/*":
                state = "block-comment"
                cursor += 1
            elif source.startswith('$@"', cursor) or source.startswith('@$"', cursor):
                state = "verbatim-string"
                cursor += 2
            elif pair == '@"':
                state = "verbatim-string"
                cursor += 1
            elif char == '"':
                state = "string"
            elif char == "'":
                state = "char"
            elif char == "{":
                depth += 1
            elif char == "}":
                depth -= 1
                if depth == 0:
                    end = cursor + 1
                    if not re.match(r"\s*private void ValidateDatabaseFile\(DataTable PO\)", source[end:]):
                        raise ValueError("Unexpected method boundary after RenderOrderInfo.")
                    method = source[start:end]
                    return method, {
                        "start_line": source.count("\n", 0, start) + 1,
                        "end_line": source.count("\n", 0, end) + 1,
                        "body_offset": opening - start,
                    }
                if depth < 0:
                    raise ValueError("Unexpected closing brace in method.")
        cursor += 1
    raise ValueError("Unterminated method, string or comment; refusing extraction.")


def verify_sha(path, expected):
    data = path.read_bytes()
    actual = sha256(data)
    if not re.fullmatch(r"[0-9A-Fa-f]{64}", expected) or actual != expected.upper():
        raise ValueError("Source SHA mismatch for " + str(path) + ": actual " + actual)
    return data


def run_variant(label, source_path, expected_sha, artifact_root, msbuild):
    raw = verify_sha(source_path, expected_sha)
    method, span = extract_method(raw)
    destination = artifact_root / label
    destination.mkdir()
    generated = destination / "RenderOrderInfo.g.cs"
    wrapper = ("using System;\nusing System.Data;\nnamespace PipelineRenderRegression\n{\n"
               "    internal sealed partial class PPOInfo\n    {\n" + method + "\n    }\n}\n")
    generated.write_bytes(wrapper.encode("utf-8"))
    project = HERE / "RenderFlagRegression.csproj"
    metadata = {
        "kind": "expected-defect-control" if label == "baseline" else "current-product-regression",
        "source": str(source_path), "source_sha256": sha256(raw),
        "extracted_method_sha256": sha256(method.encode("utf-8")),
        "generated_source_sha256": sha256(generated.read_bytes()), "method_span": span,
        "fixtures": {name: sha256((HERE / name).read_bytes())
                     for name in ("Harness.cs", "RenderFlagRegression.csproj", "run_render_flag_regression.py")},
        "boundaries": ["MES, PO history, UI/Invoke, logger, validation, code count, UI update and counter scheduling are fakes",
                       "only RenderOrderInfo is extracted unchanged; no application assembly or live Globals is loaded"],
    }
    command = [str(msbuild), str(project), "/t:Build", "/nologo", "/v:minimal",
               "/p:Configuration=Release", "/p:Platform=x64", "/p:BuildProjectReferences=false",
               "/p:FixtureArtifacts=" + str(destination), "/p:GeneratedSource=" + str(generated)]
    metadata["build_command"] = command
    dump(destination / "metadata.json", metadata)
    with (destination / "build.log").open("w", encoding="utf-8") as log:
        result = subprocess.run(command, cwd=destination, stdout=log, stderr=subprocess.STDOUT, check=False)
    metadata["build_exit_code"] = result.returncode
    dump(destination / "metadata.json", metadata)
    if result.returncode != 0:
        raise RuntimeError(label + " build failed, exit " + str(result.returncode) + "; inspect " + str(destination / "build.log"))
    executable = destination / "bin" / "RenderFlagRegression.exe"
    metadata["executable_sha256"] = sha256(executable.read_bytes())
    run_command = [str(executable), str(destination / "results.json"), label]
    metadata["run_command"] = run_command
    dump(destination / "metadata.json", metadata)
    with (destination / "run.log").open("w", encoding="utf-8") as log:
        result = subprocess.run(run_command, cwd=destination, stdout=log, stderr=subprocess.STDOUT, check=False)
    metadata["run_exit_code"] = result.returncode
    metadata["source_sha256_after"] = sha256(source_path.read_bytes())
    metadata["fixture_sha256_after"] = {name: sha256((HERE / name).read_bytes()) for name in metadata["fixtures"]}
    dump(destination / "metadata.json", metadata)
    if metadata["source_sha256_after"] != metadata["source_sha256"]:
        raise RuntimeError("Source changed during regression: " + str(source_path))
    if metadata["fixture_sha256_after"] != metadata["fixtures"]:
        raise RuntimeError("Fixture changed during regression.")
    if result.returncode != 0:
        raise RuntimeError(label + " regression failed, exit " + str(result.returncode) + "; inspect " + str(destination / "run.log"))
    summary = json.loads((destination / "results.json").read_text(encoding="utf-8-sig"))
    if summary["case_count"] != 10 or summary["failed_count"] != 0:
        raise RuntimeError("Incomplete or failing regression output for " + label)
    if label == "baseline":
        if summary["product_pass_count"] != 0 or summary["expected_defect_count"] != 8 or not summary["control_assertions_satisfied"]:
            raise RuntimeError("Baseline did not reproduce all eight expected stale-flag defects.")
    elif summary["product_pass_count"] != 10 or summary["expected_defect_count"] != 0:
        raise RuntimeError("Current product regression did not pass all ten cases.")
    return {"kind": metadata["kind"], "artifact_directory": str(destination),
            "source_sha256": metadata["source_sha256"],
            "extracted_method_sha256": metadata["extracted_method_sha256"],
            "build_exit_code": metadata["build_exit_code"], "run_exit_code": metadata["run_exit_code"],
            "product_pass_count": summary["product_pass_count"],
            "expected_defect_count": summary["expected_defect_count"], "failed_count": summary["failed_count"]}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--source", type=Path, default=REPO / "MASAN-SERIALIZATION/Views/ProductionInfo/PPOInfo.cs")
    parser.add_argument("--expected-source-sha", required=True)
    parser.add_argument("--artifacts", type=Path, required=True, help="New directory outside the checkout.")
    parser.add_argument("--msbuild", type=Path, default=DEFAULT_MSBUILD)
    parser.add_argument("--baseline-source", type=Path)
    parser.add_argument("--expected-baseline-sha")
    args = parser.parse_args()
    artifact_root = args.artifacts.resolve()
    if artifact_root == REPO or REPO in artifact_root.parents:
        parser.error("All generated sources, build output and results must stay outside the checkout.")
    if bool(args.baseline_source) != bool(args.expected_baseline_sha):
        parser.error("Baseline source and exact expected baseline SHA must be supplied together.")
    if not args.msbuild.is_file():
        parser.error("MSBuild executable not found: " + str(args.msbuild))
    verify_sha(args.source.resolve(), args.expected_source_sha)
    if args.baseline_source:
        verify_sha(args.baseline_source.resolve(), args.expected_baseline_sha)
    artifact_root.mkdir(parents=True, exist_ok=False)
    summary = {"current": None, "baseline_control": None, "failure": None}
    try:
        summary["current"] = run_variant("current", args.source.resolve(), args.expected_source_sha, artifact_root, args.msbuild)
        if args.baseline_source:
            summary["baseline_control"] = run_variant("baseline", args.baseline_source.resolve(), args.expected_baseline_sha, artifact_root, args.msbuild)
    except Exception as error:
        summary["failure"] = str(error)
        dump(artifact_root / "summary.json", summary)
        print(json.dumps(summary, ensure_ascii=False))
        return 1
    dump(artifact_root / "summary.json", summary)
    print(json.dumps(summary, ensure_ascii=False))
    return 0


if __name__ == "__main__":
    sys.exit(main())
