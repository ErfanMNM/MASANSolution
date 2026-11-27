# Carton assignment regression

Run only after the CEO grants the single shared build/test slot. The runner extracts the exact two production `HandScan*_Process` methods and current `TryAssignCartonCode` helper. It also extracts the actual carton DTO and snapshot method from `Globals.cs`. It never constructs real application globals or forms.

```powershell
python -X utf8 D:\DuAn\MASANSolution\tests\pipeline-regression\carton\run_carton_regression.py --artifacts D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts\carton-atomic-regression-<fresh-label> --before-source D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts\PCartonDashboard.before-carton-atomic.cs --grant '<actual CEO grant>'
```

The two-lane fixture aligns admitted scans via the fake logger. Its in-memory dictionary can snapshot values then barrier two **unguarded** readers. This schedules the original legal check/mutate race without reimplementing production decisions. Atomic current readers serialize and see the first assignment. Before-source replay must reproduce two assignments/two records for one code (two expected defects, odd/even); it is explicitly not product-conformant. Current must admit exactly one assignment and one immutable owner-bound snapshot, notify the duplicate outside the lifecycle lock, preserve odd/even lane routing, block stale owner or a saving/editing/loading transition, and leave occupied/wrong-owner/missing targets unchanged.

Outputs and all generated/compiled files stay outside the checkout. There are no project references, NuGet restore, app entrypoints, database, serial/TCP, network or PLC operations. Source SHA256, exact extraction SHA256, commands, actual exit codes, build/run logs and results are captured. This verifies extracted methods with explicit fake state/UI/log boundaries, not the WinForms message pump, optical acquisition, hardware or full cartoner lifecycle. Parent owns the final application build and source manifest. No execution is claimed by merely authoring these fixture files.
