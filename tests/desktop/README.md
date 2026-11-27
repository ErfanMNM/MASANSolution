# Desktop regression fixtures

Run only with the coordinator's shared build/test slot. No application entrypoint is launched.

```
MSBuild.exe tests\desktop\DesktopRegression.csproj /t:Build /p:Configuration=Release /p:Platform=x64 /m:1 /nr:false
tests\desktop\bin\Release\DesktopRegression.exe
python -X utf8 tests\desktop\sql_regression.py
```

The C# executable source-links real TCP, audit-log, audit model and chart code. TCP binds ephemeral IPv4 loopback ports only; audit/chart fixtures use a unique temporary directory. It checks audit Code roundtrip and draining 1000 accepted entries on Dispose, relative paths, event timestamps, literal regex replacement, TCP connect idempotence, receive encoding, send, EOF reconnect after a throwing subscriber, explicit disconnect, explicit reconnect, and container disposal without an event subscriber. The TCP layer intentionally exposes raw chunks, not application frames.

Before compilation, `prepare_sources.py` extracts unchanged production row-loop bodies from all three legacy loaders and `Send1Data` from both printer variants. SHA256 values identify the source files in the generated fixture. Tests execute those exact statements with in-memory DataTables/dictionaries, a fake printer sender and a no-op UI counter. They cover millisecond timestamps above Int32 range, camera dictionary routing, resumed print offset and end bounds. They do not execute the WinForms loaders' database queries, printer network I/O or UI event callback. The DATA_PRINTING event offset is compile/source-reviewed only.

The Python suite executes the **actual SQL strings extracted from production methods** in SQLite `:memory:` databases. It checks all three legacy activation UPDATEs, failed-versus-successful status persistence, production-log count and quoted scan values. It does not execute the WinForms methods or their hardcoded DB paths, so it does not prove UI, queue lifetime, routing, PO switch or PLC behavior.

Neither fixture accesses a production database, real PLC, camera, serial port, AWS or printer. These are regression fixtures, not the 200000-code pipeline benchmark (owned separately by QA).


## ROUND2 final regression coverage

The latest full executable has **22 unique C# groups**, including 14 ROUND2 groups (not an addition to historical reruns), plus **4 Python SQL tests**. Build and run only while holding the coordinator's global slot. The final owner report and SHA256 manifests live outside the repository in the existing coordination folder.

ROUND2 links or extracts unchanged production decisions for recipe CS isolation; temp-directory PO paths/AWS DB routing/JSON history/status case/atomic import; nullable audit fields/shift/concurrent queues; CSV quote/CR/LF roundtrip and rollback in both printer imports; random-key capacity; hourly chart grouping; final PDF type/hash and forced-failure stream cleanup; managed COM event ownership/prefix checks; scanner init/selection/cleanup; PLC statistics address-to-index mapping; chart token/legacy JSON injection, app-relative paths and temp-file cleanup.

Native COM lifetime uses the real COMPortEmulator class with an internal fake connection factory. It checks per-open readers, delayed stale reader isolation, Close from callbacks, a blocked UI-like subscriber, factory retry, write errors and idempotent disposal; no kernel COM call is invoked. Named-pipe/loopback tests cover silent-peer outgoing data, bidirectional traffic, reopen, actual delegate removal, stale callback suppression, event-only buffering, observer exceptions, partial TCP bind rollback, EOF cancellation, concurrent replacement and rebinding released ports.

Simulator tests execute exact CameraInstance/CameraSimulatorCore class extraction with ephemeral loopback listeners, temp CSVs and fake blocked writers. They prove quoted/header first-column handling, old-listener cleanup isolation, queued-send generation checks, client-list concurrency and sends off the calling thread. They do not construct the form, start a real camera, read production data or certify UI rendering. Header handling is explicit in the UI; it no longer guesses whether an alphanumeric first code is a header.

The linked/extracted boundaries are deliberate: no PLC writes, real serial/registry/PnP operations, printer ACK semantics, vendor scanner implementation, WebView2 rendering or production network/database/cloud access are certified. The separate QA owner performs the main 200000-code ingress benchmark.
