# Offline V10/V14 view regressions

Run only after the actual Pipeline owner receives a CEO build/test slot. Outputs,
generated exact source, build intermediates, EXE, logs and JSON stay in a new
directory outside the checkout. No NuGet restore, application entry point, live
UI, database, network, credentials or device is used.

```powershell
python -X utf8 tests/pipeline-regression/views/run_views.py --artifacts 'D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts\pipeline-views-v10-v14' --grant '<specific CEO grant>'
```

The runner extracts the exact current production `AddReceiveNotification`, the
receive-trimming while-block inside `Bgw_process_DoWork`, `HourlyChartData` and
`GetLast6Hours`. Extraction fails on missing/ambiguous members or receive producer
coverage drift. It does not rewrite production decisions. A deterministic
DateTime value adapter supplies clock operations; fake SQLite types capture the
actual SQL/timestamp bindings and return declared result rows. SQL execution,
WinForms threading and database aggregation are not covered.

Nine groups check first/boundary/overflow/burst/backlog receive trimming, that the
periodic trim preserves the send list, six-hour day and midnight shapes with all
six populated buckets/out-of-range rows, and empty results. Passing requires
build exit 0, run exit 0, exactly nine groups, and unchanged input source hashes.
`extraction.json`, `provenance.json`, `results.json`, `build.log`, and `run.log`
record the exact source and execution evidence. Earlier source-only preparation
must not be reported as an executed pass.
