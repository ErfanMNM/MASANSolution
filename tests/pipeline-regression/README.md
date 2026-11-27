# RenderOrderInfo flag regression (V07)

This isolated .NET Framework 4.8 x64 fixture extracts the exact
`PPOInfo.RenderOrderInfo()` method from a SHA-pinned production source file.
It tests synchronous render-flag cleanup and the preserved adapter call order.
It does not load the main application or production `Globals`, construct a form,
access a database, open a network connection or touch camera/PLC hardware.

Only run after the CEO grants this fixture the shared build/test slot.
The Python standard-library runner performs extraction, invokes legacy MSBuild
with `/p:BuildProjectReferences=false`, runs the standalone fixture and captures
the build/run exit codes, logs, source/method/fixture/executable hashes and JSON
results. No NuGet restore or project-reference build occurs.

All generated source and `obj`, `bin`, logs and results are placed in a new
artifact directory outside the checkout. The runner refuses an artifact path
inside the repo or an existing output directory. Missing/ambiguous signatures,
unexpected method boundaries and source/fixture hash changes fail closed.

## Cases

The ten current-source cases cover normal completion, failed-MES early return,
malformed persisted production date (`FormatException`), and injected failures
at Invoke/UI, validation, code count, UI update, counter scheduling, MES and PO
history. Every adapter checks `IsRender == true` while called. After return or
exception each current-source case requires `IsRender == false`. Injected
exceptions must propagate as the exact same exception object. The malformed
date must propagate its original `FormatException`. Normal/early-return call
order, PO/user context, passed result identities and production date are checked.

Optional pre-fix baseline control runs in a separate output directory and reports
eight `expected_defect_reproduced` cases plus two normal control behaviors. These
are not product passes; baseline `product_pass_count` is always zero. A control
that fails to reproduce any expected defect fails the runner.

The fake methods explicitly replace MES, PO history, Invoke/UI, logging,
ValidateDatabaseFile, GetCodeCount, UpdateOrderInfoUI and LoadCountersAsync.
Their internal implementations, asynchronous counter work, actual UI threading
and concurrent reentrant RenderOrderInfo calls are outside this bounded fixture.

## Granted-slot command

Use the production source SHA read after the final authorized V07 edit. The
following command is prepared, not evidence of execution:

```powershell
python -X utf8 .\tests\pipeline-regression\run_render_flag_regression.py `
  --expected-source-sha 0C12CE44EA0B7D65F98FD37970C32A31048FAB40CF276010DB3E781560BFBE01 `
  --artifacts 'D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts\pipeline-v07-regression' `
  --baseline-source 'D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts\PPOInfo.before-v07.cs' `
  --expected-baseline-sha 7040726ABC65C735E5FB92AB5E10BA307F5BE30228927A0B6263E06409781D35
```

The default compiler path is
`C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe`.
Pass `--msbuild` for a verified alternative. After execution, inspect
`summary.json`, `current/metadata.json`, `current/results.json` and each variant's
`build.log` and `run.log`. An optional baseline has its own metadata and results.
