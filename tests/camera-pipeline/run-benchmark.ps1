[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Artifacts,
    [string]$Coordination = 'D:\DuAn\MASANSolution-coordination\review-perf-20261127',
    [string]$BaselineRef = '267e4675f17da0564787bb5c8a71a21b98dd25d5',
    [ValidateSet('current','baseline','both')][string]$Mode = 'both',
    [switch]$CompileOnly,
    [switch]$Smoke,
    [int]$ProcessTimeoutSeconds = 300
)
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$target = [IO.Path]::GetFullPath($Artifacts)
if ($target.Equals($repo, [StringComparison]::OrdinalIgnoreCase) -or $target.StartsWith($repo.TrimEnd('\') + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Artifacts must be outside repo.' }
if ($target.StartsWith('C:\MasanSerialization', [StringComparison]::OrdinalIgnoreCase)) { throw 'Production directories are forbidden.' }
if (Test-Path -LiteralPath $target) { throw 'Artifact run directory must be fresh.' }
$slot = Get-Content -LiteralPath (Join-Path $Coordination 'BUILD-SLOT.md') -Raw
if ($slot -notmatch '(?im)^Owner:\s*(?:QA(?:\s|01a2192e-14ed-7920-9638-3a6e8fc70139)|01a2192e-14ed-7920-9638-3a6e8fc70139)') { throw 'QA does not own shared build/test slot. Obtain explicit CEO grant first.' }
New-Item -ItemType Directory -Path $target | Out-Null

function SourceCheckpoint {
    $paths = @(
        (Join-Path $repo 'MASAN-SERIALIZATION\Views\Dashboards\FDashboard.cs'),
        (Join-Path $repo 'MASAN-SERIALIZATION\Infrastructure\Globals.cs'),
        (Join-Path $repo 'MASAN-SERIALIZATION\Production\ProductionHelper.cs'),
        (Join-Path $repo 'MASAN-SERIALIZATION\Views\ProductionInfo\PPOInfo.cs'),
        (Join-Path $repo 'MASAN-SERIALIZATION\Enums\eState.cs')
    ) + @(Get-ChildItem -LiteralPath $PSScriptRoot -File | ForEach-Object FullName)
    $result = [ordered]@{}
    foreach ($path in ($paths | Sort-Object -Unique)) { $result[$path.Substring($repo.Length + 1)] = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant() }
    return $result
}

$before = SourceCheckpoint
$before | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $target 'source-before.json') -Encoding utf8
@{ timestamp = (Get-Date -Format o); repo = $repo; head = (& git -C $repo rev-parse HEAD); branch = (& git -C $repo branch --show-current); baselineRef = $BaselineRef; command = $MyInvocation.Line; mode = $Mode; slot = $slot; status = @(& git -C $repo status --short) } | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $target 'run.json') -Encoding utf8
$runs = @()
foreach ($variant in @('baseline','current')) {
    if ($Mode -ne 'both' -and $Mode -ne $variant) { continue }
    $work = Join-Path $target $variant
    New-Item -ItemType Directory -Path $work | Out-Null
    $ref = if ($variant -eq 'baseline') { $BaselineRef } else { 'working-tree' }
    $extract = Join-Path $work 'extracted'
    $extractArgs = @('-X','utf8', (Join-Path $PSScriptRoot 'extract_production.py'), '--repo', $repo, '--output', $extract, '--ref', $ref)
    & python @extractArgs *> (Join-Path $work 'extract.log')
    $extractExit = $LASTEXITCODE
    if ($extractExit -ne 0) { throw "Extraction failed ($extractExit). See $work\extract.log. No retries/method fallback." }
    $bin = Join-Path $work 'bin\'
    $obj = Join-Path $work 'obj\'
    $buildArgs = @('msbuild', (Join-Path $PSScriptRoot 'CameraPipeline.Bench.csproj'), '/t:Build', '/nologo', '/verbosity:minimal',
        "/p:ExtractedSource=$extract\Production.Extracted.cs", "/p:OutputPath=$bin", "/p:IntermediateOutputPath=$obj", "/p:BaseIntermediateOutputPath=$obj")
    ('dotnet ' + ($buildArgs -join ' ')) | Set-Content -LiteralPath (Join-Path $work 'build-command.txt') -Encoding utf8
    & dotnet @buildArgs *> (Join-Path $work 'build.log')
    $buildExit = $LASTEXITCODE
    if ($buildExit -ne 0) { throw "Build failed ($buildExit). See $work\build.log." }
    if ($CompileOnly) {
        $runs += [pscustomobject]@{ variant = $variant; extractionExit = $extractExit; buildExit = $buildExit; testExit = $null; phase = 'compile-only'; log = (Join-Path $work 'build.log') }
        $runs | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $target 'execution.json') -Encoding utf8
        continue
    }
    $results = Join-Path $work 'results'
    $exe = Join-Path $bin 'CameraPipeline.Bench.exe'
    $runArgs = ('"{0}" {1}' -f $results, $variant)
    if ($Smoke) { $runArgs += ' smoke' }
    ($exe + ' ' + $runArgs) | Set-Content -LiteralPath (Join-Path $work 'test-command.txt') -Encoding utf8
    $process = Start-Process -FilePath $exe -ArgumentList $runArgs -WorkingDirectory $work -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $work 'test.log') -RedirectStandardError (Join-Path $work 'test.stderr.log')
    # Retain the native handle before waiting: Windows PowerShell's Start-Process
    # proxy can otherwise return a null ExitCode after a short-lived child exits.
    $processHandle = $process.Handle
    $processId = $process.Id
    $timedOut = -not $process.WaitForExit($ProcessTimeoutSeconds * 1000)
    if ($timedOut) {
        # Kill only the exact harness process started above; never MSBuild/testhost/other app.
        $process.Kill(); $process.WaitForExit()
    }
    $process.WaitForExit()
    $testExit = if ($timedOut) { -999 } else { $process.ExitCode }
    if ($null -eq $testExit) { throw "Harness ended without an observable exit code (pid=$processId); do not infer success from its log." }
    $runs += [pscustomobject]@{ variant = $variant; extractionExit = $extractExit; buildExit = $buildExit; processId = $processId; testExit = $testExit; timedOut = $timedOut; log = (Join-Path $work 'test.log'); results = $results }
    $runs | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $target 'execution.json') -Encoding utf8
    if ($timedOut -or $testExit -eq 3) { throw "Harness did not complete safely (exit=$testExit). Review logs before another run." }
}
$after = SourceCheckpoint
$after | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $target 'source-after.json') -Encoding utf8
$drift = @($before.Keys | Where-Object { $before[$_] -ne $after[$_] })
if ($drift.Count -gt 0 -and -not $CompileOnly -and -not $Smoke) { throw ('Source changed during measurement; results are not a final frozen checkpoint: ' + ($drift -join ', ')) }
$current = @($runs | Where-Object variant -eq 'current')
if (-not $CompileOnly -and $current.Count -gt 0 -and $current[0].testExit -ne 0) { exit 2 }
exit 0
