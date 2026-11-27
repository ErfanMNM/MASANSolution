# Actual compiled SQLite storage integration checks

This standalone .NET Framework 4.8 x64 console project calls the actual compiled `ProductionOrder.GetDataPO.CheckCodeActivatedInOtherPO`, `GetAllActivatedCodesFromOtherPOs`, and `ProductionOrder.PostDB.Update_Active_Status` methods. It references a selected `MASAN-SERIALIZATION.exe`; it does not copy/reimplement production decisions or build the application. It never constructs `ProductionOrder`, starts the GUI, or accesses `Globals`. The independent `ProductionCodeData` data-transfer class is safe to construct.

Before any production storage method runs, reflection redirects `dataPath`, `poMesJsonPODataPath`, `poMesJsonCodesPath`, and `POLog_dbPath` to the fresh synthetic sandbox. Field/API drift fails preparation. Fixture PO JSON uses uppercase `GTIN` and `productionDate`. The production path resolver is checked against the expected sandbox `2026-11/8930000000001` directory.

PO initialization regressions additionally invoke actual compiled `Check_Database_File` on a `FormatterServices.GetUninitializedObject` instance with only its two required DAO backing fields supplied. Its production constructor is never run. A synthetic uninitialized `AppConfigs` with `cartonPack=24` is injected into SunnyUI's existing cache after checking that its type has no static initializer and its `Current` getter retains the inspected non-null-cache return path. The configuration loader/default writer is bypassed; cache/getter drift fails the harness. Both runtime current directory and application output directory are isolated outside the repository. Only the SunnyUI managed libraries are copied, not any application INI or production configuration.

## Run only after receiving the shared build/test slot

The repository uses one shared build/test slot. A `-BuildSlotGrant` string records the CEO's explicit grant; it is not itself a permission request or an automatic lock. Do not invoke the script while the coordination slot is HOLD or owned by another room. No NuGet restore is needed. Full-framework MSBuild and the .NET Framework 4.8 targeting pack must already be installed.

Baseline, using the archived original EXE and existing runtime dependencies:

```powershell
.\tests\integration\run-storage.ps1 `
  -Label baseline `
  -AppBinary 'D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts\baseline-binary\MASAN-SERIALIZATION.exe' `
  -SourceCheckpoint '267e4675f17da0564787bb5c8a71a21b98dd25d5 (archived baseline EXE)' `
  -BuildSlotGrant '<actual CEO grant>' -ExpectBaseline
```

Current, after the production Release build:

```powershell
.\tests\integration\run-storage.ps1 `
  -Label current `
  -AppBinary 'D:\DuAn\MASANSolution\MASAN-SERIALIZATION\bin\x64\Release\MASAN-SERIALIZATION.exe' `
  -SourceCheckpoint '<exact current build checkpoint and source digest>' `
  -BuildSlotGrant '<actual CEO grant>'
```

Optional arguments: `-Iterations 1..1000` (default 1000 per direct lookup scenario), `-BulkIterations 1..1000` (default 50 per full-set startup scenario), `-Warmup 0..20` (default 5), `-MsBuild '<full MSBuild.exe path>'`, `-AppDependencyDirectory '<selected runtime directory>'`, and `-ArtifactRoot '<outside-repository directory>'`. All `bin`/`obj`, databases, logs, JSON, and CSV go under `ArtifactRoot\storage-<label>`. A fresh label is required for every run. The native `x64\SQLite.Interop.dll` is copied to the harness output alongside the managed SQLite and Newtonsoft dependencies. No app configuration, credentials, network, camera, or PLC are used.

## Coverage and interpretation

- The template and every healthy PO database contain exactly **200000 rows and 200000 distinct synthetic codes**, enforced by a `UNIQUE Code` constraint and executable row-count assertions. `PO200000` is the current PO. Same-month/same-GTIN fixtures have 1 and 5 old PO databases, each independently tracking the same code population. Database setup and copy time are reported separately.
- Regression checks cover sibling activation, negative/nonzero status, inactive code, missing code, current-PO exclusion, exact bulk union/deduplication, first PO, missing metadata, and auxiliary database exclusions.
- A corrupt old database and a valid SQLite old database lacking `UniqueCodes` are tested separately alongside a healthy sibling. Both APIs must fail closed. `-ExpectBaseline` records the original fail-open observations as `expected_defect`, **not passed product checks**. A baseline run can exit 0 to indicate successful capture while `ProductConformant=false`; inspect `RunOutcome` and defect counts. Current runs omit this flag and return nonzero on fail-open behavior.
- A separate bounded contention probe changes a synthetic sibling to DELETE journal mode, holds `BEGIN EXCLUSIVE`, starts the actual direct lookup on a background task, and releases the lock in `finally` after a target 250 ms. It records actual lock hold/query duration and whether the call returned before release. A 5-second start/completion guard fails the harness on an unexpected hang. Other healthy fixtures use WAL, as production database creation does. This is a blocking observation, not a product timeout or an SLA assertion.
- A write regression calls actual compiled `PostDB.Update_Active_Status` to store a final carton and nonzero sub-camera timestamp, then applies an older main-camera snapshot containing `cartonCode="pending"` and an empty sub-camera timestamp. Direct SQLite readback must retain both final values. Baseline overwritten values are separately recorded as known `expected_defect` observations; current must preserve them. The initial final write is always a required precondition.
- Additional actual compiled writes must persist a rejected `Main_Camera_Status="-1"`, and must propagate `SQLiteException` when the synthetic target table is deliberately renamed. Baseline forced-active status and swallowed write errors are known `expected_defect` observations; current must meet both requirements.
- Small, separately isolated PO initialization fixtures use required quantity2 and MES pools of1 or3 codes. They cover missing-MES failure then populated retry, corrupt-old-DB failure then repaired retry, preservation of valid nonempty rows/activation data, and repeated insufficient-pool rejection. These are initialization regressions, separate from the200000-code performance fixtures. Baseline retry/false-ready behavior is recorded as known defects; current must fail closed and populate enough codes before success.
- Refill fixtures call the actual compiled `GetDataPO.Check_And_Add_Missing_Codes_To_Dictionary`, after the real initializer creates two synthetic seed rows. A sibling database activates code A. MES `[A,B,C]` must add B/C without A; a separate `[A,B,B]` case must add B exactly once without rolling back, and existing rows/activation/carton/sub-camera data must remain unchanged. An unreadable sibling must fail closed before any refill write. Only synthetic JSON/SQLite paths are used; no refill decision logic is copied into the harness. These new cases require a newly compiled application binary carrying the refill fix; historical baseline/current artifacts before this addition do not claim these cases ran.
- An actual compiled update aimed at a nonexistent row must throw `InvalidOperationException`, permitting the production queue to retain its head rather than acknowledge a silent no-op. Existing data must remain unchanged. The original schema-failure propagation case remains separate. This check covers the storage method contract; camera queue retention is checked by its owning suite.
- Sequential warm-cache timings call the actual methods, with separate activated duplicate, inactive, missing, and bulk-union scenarios for 1 and 5 siblings. Direct lookups use 1000 measured calls each by default; the bulk union is a startup operation and uses 50 full-scan calls each. Actual sample counts are recorded per metric, separately from 5 warmup calls. Each timed call is checked for correctness; setup, warmup, validation, and CSV writing are excluded from query duration. `timings.csv` includes nearest-rank p50/p95/p99/max/mean and query/whole-loop throughput; separate sample CSVs retain each duration. Whole-loop throughput also includes harness checks. Method return construction and managed allocation remain part of query duration. No filesystem enumeration order is promised; duplicate lookup may return early, while inactive/missing scan all sibling files.
- `results.json` records the actual loaded EXE SHA256, caller-declared source checkpoint, runtime and SQLite version, fixtures, assertions, metrics, and environment. `provenance.json` also records selected/dependency/harness hashes and the live source hash. The live working-tree hash is explicitly separate from provenance of an archived binary. Compare only matching iteration counts and hardware/cache conditions.
- These checks measure real local SQLite storage paths, not optical camera acquisition, TCP transport, UI behavior, complete camera ingress scheduling, or PLC hardware. There is no asserted latency SLA. The separate camera-pipeline harness owns those simulated ingress-to-pre-PLC measurements.

The sandbox is retained for review. Remove only a verified synthetic artifact directory manually when no longer needed; this runner does not recursively delete data.
