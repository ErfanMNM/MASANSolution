# V12 CheckVIP lookup admission regression

SHA-pinned exact extraction of production `ProcessScannedCode` and
`btnfind_Click`, compiled in a standalone .NET Framework 4.8 x64 legacy project.
The original `Task.Run` runs on the actual thread pool. Explicit fake UI, grid,
PO context and data lookups replace every external boundary. No application
assembly, real Globals initializer, DB, camera, PLC or network is accessed.

Eight cases cover: a deterministic callback held in its first UI action before
dispatch, with a second callback required to return while it is still held;
trimmed input and captured PO-A after a forced PO-B/input swap; first/second DB
lookup exceptions; grid UI and result UI exceptions; initial UI exception;
notfound messages and the manual button delegation. Every UI/DB boundary asserts
the lifecycle lock is not held and the atomic admission gate is held. Grid clear
must execute inside the fake UI invocation. Error cases require gate release and
a successful retry. The fixture does not simulate Task.Run scheduling failure;
the outer source catch is independently reviewable. Worker errors retain the
existing faulted-task behavior; this fixture validates gate cleanup/retry only.
It does not certify a real WinForms message pump or hardware transport.

Do not run until the CEO explicitly grants the shared build/test slot:

```powershell
python -X utf8 .\tests\pipeline-regression\vip\run_vip_regression.py `
  --expected-source-sha 1E40BA342ED8966F6E90F859CC9CE57988A85731D4E9BCE8B1ADAD636AC17596 `
  --artifacts 'D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts\pipeline-v12-vip-regression'
```

The new artifact directory must be outside the checkout. Exact signatures and
following-method boundaries must match; missing/ambiguous methods or SHA drift
fail closed. Generated source, obj/bin, build/run logs, JSON results, commands,
exit codes and source/method/fixture/executable hashes stay in that directory.
Compiler uses `/nr:false /p:BuildProjectReferences=false`; no NuGet restore or
project references. The project compiles exactly Harness.cs and generated
methods, so sibling render/carton/views fixtures are not included.

Default MSBuild is the verified VS18 Enterprise Current/Bin/MSBuild.exe path;
`--msbuild` accepts an explicit alternative. Read metadata.json, results.json,
build.log and run.log after the granted execution. Preparing these files is not
evidence of a passing build/test.
