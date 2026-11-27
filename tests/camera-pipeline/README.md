# Camera ingress to pre-PLC sandbox benchmark

This console program compiles **exact extracted production methods**, including both camera RECEIVED callbacks, real Task.Run/BackgroundWorker dispatch, main/sub classification, carton state transitions, result counters, persistence enqueue and Sub pre-PLC wrapper. It never references the application assembly or starts WinForms/devices/production DB initialization. It is not a dictionary-only reimplementation.

QA owns this folder. `tests/desktop/**` and `tests/integration/**` have separate owners. Generated source, fake PO, executables, JSON and CSV go to a fresh directory outside the repo. Build/test/run requires the shared CEO build slot, including extraction/preparation.

## Run after slot grant and Pipeline source freeze

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File D:\DuAn\MASANSolution\tests\camera-pipeline\run-benchmark.ps1 -Artifacts D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts\qa-camera-UNIQUE -Mode both
```

Requires installed .NET SDK (MSBuild/Roslyn), .NET Framework 4.8 targeting/runtime and Python. No NuGet package/third-party download required. The script refuses a missing QA slot, an existing artifact directory or output inside repo/production paths. It captures commands, logs, exit codes, baseline ref, environment and before/after SHA256 for the harness and relevant production sources. Extraction signature/field drift or compile mismatch fails closed; never fall back to copied validation. A 300-second subprocess watchdog terminates only its own benchmark executable if required. Exit 0 means current suite passed (baseline may record historical failures), exit 2 means current assertion failed, exit 3 means fatal harness error. Read per-variant `execution.json` and logs, since baseline failures must not be counted as current passes.

## Fixture and workload

Each variant creates and reloads a sandbox PO file with **exactly 200000 unique deterministic codes**, checks count/distinct/round-trip, and writes its SHA256 metadata. Main and Sub dictionaries contain two distinct DTO instances per code. Fixture generation, loading and warmup are excluded from camera timing.

Current includes a complete 200000-code Main callback-ingestion scenario. Both modes include identical bounded 5000-code burst, open-loop 1000 events/s offer workload (a stimulus, not an SLA), slow serialized fake UI, slow cross-PO, slow pre-PLC read, slow fake transport influencing following scans, both-camera overlap and concurrent duplicate activation. Classification covers null/empty, literal `FAIL` (actual no-read sentinel), unknown `NOREAD`, notfound, duplicate policy, text control normalization, Sub-before-Main, cross-PO duplicate versus lookup error, PLC failure/route and ACK-enabled pass. Busy Sub reject is an explicit terminal Error result and remains part of the production fail-safe contract.

Between drained baseline scenarios, the fixture replaces each sandbox queue with a new instance of the same production queue type. Baseline concurrent `Queue<T>` corruption can make even `Clear()` throw; replacement isolates later workloads without changing measured production scheduling or erasing prior failures. The archived failed run retains that exception as baseline evidence.

`events.csv` stores scheduled, callback ingress, worker start, processing start, pre-PLC, callback return and terminal monotonic timestamps with camera/outcome/route/count. `summary.json` records offered/terminal/prePLC counts, missing/repeated/exception counts, actual offered/throughput rates, p50/p95/p99/max overall/per-camera/per-outcome, scheduler lateness, dispatch wait, Main serialization wait, processing, callback wall time, drain, outstanding peak, sampled working set/managed memory and GC counts. Memory/GC observation includes fixture population; latency begins after population. Missing/duplicate results, duplicate acceptance, failed send recorded as success, lost counters/enqueues and worker exceptions fail correctness. Healthy worker-admitted Sub must pass; inline busy decisions must Error/reject0, and each Sub load scenario requires at least one successful worker. Latency has no asserted SLA. Main ordering is asserted for single-producer load once the Pipeline FIFO contract is present; baseline ordering inversions are reported. Parallel producer IDs do not define arrival order. Sub busy branch overtaking is reported separately, not treated as FIFO compliance.

## Exact source and instrumentation boundaries

`extraction.json` stores every original member text/hash, instrumented hash, input hashes, capabilities and generated-source hash. Entry probes observe Main processing (inside its lock when present), Sub processing and result classification; a record observer runs after actual production enqueue. A Sub request-constructor probe binds request object identity to input identity, so resetting scan IDs on a new PO cannot collide. The test-only Task shim calls real `System.Threading.Tasks.Task.Run/ContinueWith`, observes worker entry/finally completion and report-fails worker exceptions while preserving task faults. Baseline uses real `BackgroundWorker` with a measured DoWork wrapper. Correlation is event identity via AsyncLocal/request reference, never payload or a resettable scan ID.

Current-only PO regressions extract the actual `PPOInfo.TryEnterEditing` guard into a minimal class with no app constructor/controls. They exercise its real `HasPendingWork` predicate for admitted workers and each of seven queues, plus retry after drain. The whole extracted `Globals_Database` supplies actual carton activation/snapshot helpers and DTOs. Snapshot tests switch the synthetic live PO/config after enqueue and check retained owner/fields. A forced stale-worker scenario uses a test gate around real task dispatch: callbacks capture A, the actual guard refuses transition, then the fixture deliberately bypasses the guard and swaps to B to prove old workers reject/record A without changing B dictionaries/counters. This forced pointer swap is defense in depth, not an allowed production UI transition or a test of SaveProductionData/database consumer. Stale-owner paths intentionally emit no new-PO HMI result, so record and HMI observations are distinct.

For compile-only preflight add `-CompileOnly`; for correctness-only smoke add `-Smoke -Mode current`. Smoke still validates the 200000-code file fixture but omits load scenarios and full200000 callback ingestion. These modes do not produce final performance evidence while source is still changing.

Fake boundaries are deliberate:

- File PO is synthetic. Test-only `ProductionOrder`/`Globals` have no real constructor or static initializer, and every Main/Sub DTO/carton is sandbox data. Application SQL/SQLite path helpers are not compiled. CEO's separate integration suite measures actual cross-PO SQLite storage.
- Cross-PO returns a production `TResult`, with populated DataTable for a real duplicate versus null for lookup failure. Its latency is injected, so callback numbers cannot claim full production DB timing.
- Fake `InvokeIfRequired` serializes/delays the action. Console stores count only; UI rendering, actual WinForms message pump and display-list memory are excluded. Production deferred-console helper is extracted if present; its UI timer drain is outside this pre-PLC seam.
- PLC send stub records immediately on entry before injected fake send delay. Sub's production synchronous `ReadInt32` before send is faked separately and measured with delay. Post-send polling uses deterministic ACK data; optical camera/TCP framing, PLC driver/network and mechanical rejection are excluded.
- Production enqueue code/types are extracted. Persistence consumer/disk throughput and downstream cloud services are excluded; retained queue memory is included.
- `IsNullOrEmpty` and numeric `ToInt32` external extensions have explicit adapters for fixture inputs; malformed numeric configuration is outside coverage.

Numbers describe synthetic callback ingress to immediately-before-PLC transport, not a physical-camera-to-PLC hardware latency. A high saturation delay or busy rejection count is reported as observed capacity behavior, never converted into an invented line-rate SLA.
