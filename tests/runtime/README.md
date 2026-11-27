# Isolated Runtime regressions

Run only while holding the team's exclusive build/test slot.

```
MSBuild.exe tests\runtime\RuntimeRegression.csproj /t:Build /p:Configuration=Release /p:Platform=x64 /m:1 /nr:false /v:minimal
tests\runtime\bin\Release\RuntimeRegression.exe
```

Ten groups cover O06–O10. MQTT methods are exact production source with only
the two certificate-file constructor statements omitted by a checked extractor.
The 5s healthcheck delay remains unchanged. Google, both PLC components and the
TCP server (including designer disposal hooks) are source-linked. Auth INIT/Save
are extracted unchanged into a wrapper with fake controls, user lookup and log.
The extractor fails if a method or certificate boundary no longer matches, and
writes SHA256 of every input to `obj/source-sha256.json`.

MQTT/Google/PLC implementations are explicit in-memory fakes. No SDK DLL is
referenced, no TLS/cert credentials are loaded, no PLC socket can be created,
and no production DB is used. Auth test paths and download files use GUID-named
temporary directories removed on completion. Only the TCP server test opens
network sockets: IPv4 loopback, ephemeral ports. No production application
entrypoint is executed. The harness does not certify actual MQTT TLS, Google
SDK networking, PLC UDP/device behavior, or physical camera acquisition.

PLC Dispose may wait for a synchronous SDK Write already in flight (configured
receive timeout 1000ms); status callbacks are outside the lock and never joined.
The component does not take ownership of external calls through its public plc
field. TCP callbacks remain raw chunks with the existing encoding behavior.
