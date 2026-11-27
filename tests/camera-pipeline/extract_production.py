"""Extract real callback/decision bodies; never imports the application or opens its DB.

Generated timing probes observe entry/results only. Task scheduling and worker admission
remain in the extracted source. The manifest records original and instrumented hashes.
Run only after acquiring the shared build/test slot.
"""
import argparse
import hashlib
import json
import pathlib
import re
import subprocess


def sha(text):
    return hashlib.sha256(text.encode("utf-8")).hexdigest()


def masked(text):
    # Preserve offsets/newlines while ignoring comments, quoted strings and chars.
    token = re.compile(r'//[^\n]*|/\*.*?\*/|@"(?:[^"]|"")*"|"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'', re.S)
    return token.sub(lambda m: re.sub(r'[^\n]', ' ', m.group()), text)


def block(text, pattern, label):
    clean = masked(text)
    matches = list(re.finditer(pattern, clean, re.M))
    if len(matches) != 1:
        raise ValueError(f"Extraction drift: {label}: expected 1 signature, found {len(matches)}")
    start = matches[0].start()
    opening = clean.find("{", matches[0].end())
    if opening < 0:
        raise ValueError(f"No block for {label}")
    depth = 0
    for pos in range(opening, len(clean)):
        if clean[pos] == "{":
            depth += 1
        elif clean[pos] == "}":
            depth -= 1
            if depth == 0:
                return text[start:pos + 1]
    raise ValueError(f"Unbalanced block: {label}")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--repo", required=True)
    parser.add_argument("--output", required=True)
    parser.add_argument("--ref", default="working-tree")
    args = parser.parse_args()
    repo, out = pathlib.Path(args.repo).resolve(), pathlib.Path(args.output).resolve()
    if out == repo or repo in out.parents:
        raise ValueError("Generated source must be outside the repository")
    if out.exists():
        raise ValueError("Extraction directory must be fresh; never overwrite a prior measurement")
    out.mkdir(parents=True)
    inputs, members = {}, []

    def read(path):
        if args.ref == "working-tree":
            value = (repo / path).read_text(encoding="utf-8-sig")
        else:
            value = subprocess.check_output(["git", "show", args.ref + ":" + path], cwd=repo).decode("utf-8-sig")
        inputs[path] = sha(value)
        return value

    dashboard = read("MASAN-SERIALIZATION/Views/Dashboards/FDashboard.cs")
    globals_source = read("MASAN-SERIALIZATION/Infrastructure/Globals.cs")
    production = read("MASAN-SERIALIZATION/Production/ProductionHelper.cs")
    states = read("MASAN-SERIALIZATION/Enums/eState.cs")
    po_info = read("MASAN-SERIALIZATION/Views/ProductionInfo/PPOInfo.cs")
    originals = {}

    def extract(text, pattern, label, probe=None):
        original = block(text, pattern, label)
        instrumented = original
        if probe:
            opening = masked(original).find("{")
            if label == "CameraMain_Process" and "lock (_cameraMainProcessingLock)" in masked(original):
                opening = masked(original).find("{", masked(original).find("lock (_cameraMainProcessingLock)"))
            instrumented = original[:opening + 1] + "\n" + probe + "\n" + original[opening + 1:]
        if label == "CameraSubRequest":
            if original.count("ScanId = scanId;") != 1:
                raise ValueError("Sub request correlation binding drift")
            instrumented = original.replace("ScanId = scanId;", "ScanId = scanId;\nCameraPipeline.QA.QaHooks.BindSub(this);")
        if label == "Enqueue_Product_To_Record":
            # Observe the exact record after actual production enqueue; ownership is
            # not inferred from HMI status (stale-owner records intentionally skip HMI).
            end = instrumented.rfind("}")
            if "_produtionCodeData" not in masked(instrumented):
                raise ValueError("Record observation variable drift")
            instrumented = instrumented[:end] + "\nCameraPipeline.QA.QaHooks.RecordEnqueued(_produtionCodeData);\n" + instrumented[end:]
        originals[label] = {"source": original, "originalSha256": sha(original), "instrumentedSha256": sha(instrumented)}
        return instrumented

    methods = ["CameraMain_ClientCallBack", "CameraSub_ClientCallBack", "CameraMain_Process", "CameraSub_Process",
               "MeasureCameraSubPrePlcLatency", "SendCameraSubToPLC", "Send_Result_Content_CMain", "Send_Result_Content_CSub",
               "Enqueue_Product_To_Record", "Enqueue_Product_To_SQLite", "subpr_DoWork"]
    probes = {
        "CameraMain_Process": "CameraPipeline.QA.QaHooks.ProcessMain();",
        "CameraSub_Process": "CameraPipeline.QA.QaHooks.ProcessSub(request);",
        "Send_Result_Content_CMain": "CameraPipeline.QA.QaHooks.Result(status.ToString());",
        "Send_Result_Content_CSub": "CameraPipeline.QA.QaHooks.Result(status.ToString());",
    }
    for name in methods:
        members.append(extract(dashboard, r'^\s*(?:private|public)\s+[\w?<>]+\s+' + name + r'\s*\([^;{}]*\)', name, probes.get(name)))
    for name in ["CameraSubRequest", "CameraSubHistoryEntry", "e_Dash_LogType"]:
        members.append(extract(dashboard, r'^\s*private\s+(?:sealed\s+)?(?:class|enum)\s+' + name + r'\b', name))

    # New production helpers/fields require an explicit allowlist update, never silently omit.
    field_names = ["_cameraSubHistory", "_cameraSubCounterLock", "_cameraMainProcessingLock", "_cameraSubBusy", "_cameraConsoleMessages", "_cameraMainDispatchLock", "_cameraMainTail"]
    for name in field_names:
        pattern = r'^\s*private[^\r\n;]*\b' + name + r'\s*(?:=[^;]*)?;'
        matches = list(re.finditer(pattern, masked(dashboard), re.M))
        if len(matches) > 1 or (not matches and name in field_names[:2]):
            raise ValueError("Field extraction drift: " + name)
        if matches:
            members.append(dashboard[matches[0].start():matches[0].end()])
    # Pipeline may add this log-only UI helper; it must remain production source.
    if re.search(r'\bPostCameraConsole\s*\(', masked(dashboard)):
        members.append(extract(dashboard, r'^\s*private\s+void\s+PostCameraConsole\s*\([^;{}]*\)', "PostCameraConsole"))

    dtos = []
    for name in ["Globals_Database", "ProductionCodeData", "ProductionCodeData_Record", "ProductionCartonData", "CameraMain_HMI", "CameraSub_HMI", "AWS_Recive_Data", "AWS_Send_Data"]:
        dtos.append(extract(globals_source, r'^\s*public\s+(?:static\s+)?class\s+' + name + r'\b', name))
    if re.search(r'\bclass\s+CartonActivationRequest\b', masked(globals_source)):
        dtos.append(extract(globals_source, r'^\s*public\s+sealed\s+class\s+CartonActivationRequest\b', "CartonActivationRequest"))
    counter = extract(production, r'^\s*public\s+class\s+Product_Counter\b', "Product_Counter")
    enums = []
    for name in ["e_Production_State", "e_Production_Status"]:
        enums.append(extract(production, r'^\s*public\s+enum\s+' + name + r'\b', name))
    enums.append(extract(production, r'^\s*public\s+class\s+TResult\b', "TResult"))
    prefix = "using System;\nusing System.Collections.Generic;\nusing System.Collections.Concurrent;\nusing System.ComponentModel;\nusing System.Diagnostics;\nusing System.Threading;\nusing System.Threading.Tasks;\nusing System.Data;\nusing MASAN_SERIALIZATION.Enums;\nusing MASAN_SERIALIZATION.Production;\nusing CameraPipeline.QA;\n"
    generated = prefix + "namespace MASAN_SERIALIZATION.Views.Dashboards { public partial class FDashboard {\n" + "\n".join(members) + "\n}}\n"
    generated += "namespace MASAN_SERIALIZATION {\n" + "\n".join(dtos) + "\n}\n"
    has_po_fence = bool(re.search(r'\bTryEnterEditing\s*\(', masked(po_info)))
    if has_po_fence:
        guard = extract(po_info, r'^\s*private\s+bool\s+TryEnterEditing\s*\([^;{}]*\)', "TryEnterEditing")
        generated += "namespace MASAN_SERIALIZATION.Views.ProductionInfo { public partial class PPOInfo {\n" + guard + "\n}}\n"
    states_namespace = block(states, r'^namespace\s+MASAN_SERIALIZATION\.Enums\b', "state namespace")
    generated += "namespace MASAN_SERIALIZATION.Production {\n" + "\n".join(enums) + "\npublic partial class ProductionOrder {\n" + counter + "\n}}\n" + states_namespace
    (out / "Production.Extracted.cs").write_text(generated, encoding="utf-8")
    (out / "extraction.json").write_text(json.dumps({"ref": args.ref, "inputSha256": inputs, "generatedSha256": sha(generated), "members": originals,
        "capabilities": {"poTransitionGuard": has_po_fence, "cartonActivationRequest": "CartonActivationRequest" in originals},
        "instrumentation": "Timing probes in Main/Sub process and result methods; record observer after actual enqueue. Main process probe inside production serialization lock if present. Sub request constructor binds request object identity (scan IDs can reset between POs) for real BackgroundWorker correlation. Globals_Database including ownership/snapshot/HasPendingWork/EnqueueCartonActivation helpers is extracted whole. Fake Task forwards real Task.Run/ContinueWith and observes workers. No decision body rewritten."}, ensure_ascii=False, indent=2), encoding="utf-8")
    print(json.dumps({"generated": str(out / "Production.Extracted.cs"), "ref": args.ref, "memberCount": len(originals), "generatedSha256": sha(generated)}))


if __name__ == "__main__":
    main()
