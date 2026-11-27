[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][ValidatePattern('^[a-zA-Z0-9][a-zA-Z0-9-]{0,50}$')][string]$Label,
    [Parameter(Mandatory = $true)][string]$AppBinary,
    [Parameter(Mandatory = $true)][string]$SourceCheckpoint,
    [Parameter(Mandatory = $true)][string]$BuildSlotGrant,
    [string]$AppDependencyDirectory = 'D:\DuAn\MASANSolution\MASAN-SERIALIZATION\bin\x64\Release',
    [string]$ArtifactRoot = 'D:\DuAn\MASANSolution-coordination\review-perf-20261127\artifacts',
    [ValidateRange(1, 1000)][int]$Iterations = 1000,
    [ValidateRange(1, 1000)][int]$BulkIterations = 50,
    [ValidateRange(0, 20)][int]$Warmup = 5,
    [switch]$ExpectBaseline,
    [string]$MsBuild
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# The caller must obtain the shared build/test slot before invoking this script.
# This project references a compiled EXE only; it has no production ProjectReference.
if ([string]::IsNullOrWhiteSpace($BuildSlotGrant)) { throw 'An explicit CEO build-slot grant is required.' }
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..'))
$app = (Resolve-Path -LiteralPath $AppBinary).Path
$dependencies = (Resolve-Path -LiteralPath $AppDependencyDirectory).Path
$root = [IO.Path]::GetFullPath($ArtifactRoot)
$output = Join-Path $root "storage-$Label"
$repoPrefix = $repo.TrimEnd('\') + '\'
if ($output.StartsWith($repoPrefix, [StringComparison]::OrdinalIgnoreCase) -or $output.Equals($repo, [StringComparison]::OrdinalIgnoreCase)) { throw 'Artifacts must be outside the repository.' }
foreach ($token in @('Record_', 'carton_', 'Send_AWS_', 'Recive_AWS_')) {
    if ($output.Contains($token)) { throw "Artifact path contains production exclusion token: $token" }
}
if (Test-Path -LiteralPath $output) { throw "Use a fresh label; output already exists: $output" }
foreach ($relative in @('System.Data.SQLite.dll', 'Newtonsoft.Json.dll', 'SunnyUI.dll', 'SunnyUI.Common.dll', 'x64\SQLite.Interop.dll')) {
    if (-not (Test-Path -LiteralPath (Join-Path $dependencies $relative) -PathType Leaf)) { throw "Missing dependency: $relative" }
}
if (-not $MsBuild) {
    $command = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
    if ($command) { $MsBuild = $command.Source }
    else {
        $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
        if (Test-Path -LiteralPath $vswhere) {
            $found = @(& $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe')
            if ($found.Count -gt 0) { $MsBuild = $found[0] }
        }
    }
}
if (-not $MsBuild -or -not (Test-Path -LiteralPath $MsBuild -PathType Leaf)) { throw 'Pass -MsBuild with an installed full-framework MSBuild.exe path.' }

$null = New-Item -ItemType Directory -Path $output
$bin = Join-Path $output 'build\bin'
$obj = Join-Path $output 'build\obj'
$sourcePath = Join-Path $repo 'MASAN-SERIALIZATION\Production\ProductionHelper.cs'
$binaryHash = (Get-FileHash -LiteralPath $app -Algorithm SHA256).Hash
$dependencyHashes = @{}
foreach ($relative in @('System.Data.SQLite.dll', 'Newtonsoft.Json.dll', 'SunnyUI.dll', 'SunnyUI.Common.dll', 'x64\SQLite.Interop.dll')) {
    $dependencyHashes[$relative] = (Get-FileHash -LiteralPath (Join-Path $dependencies $relative) -Algorithm SHA256).Hash
}
$provenance = [ordered]@{
    recordedUtc = [DateTime]::UtcNow.ToString('o')
    sourceCheckpointDeclaredByCaller = $SourceCheckpoint
    sourceCheckpointNote = 'The caller must match this label to the build. Runtime application bytes are independently SHA256 verified.'
    selectedApplication = $app
    selectedApplicationSha256 = $binaryHash
    dependencyDirectory = $dependencies
    dependencySha256 = $dependencyHashes
    gitHeadAtHarnessBuild = (& git -C $repo rev-parse HEAD | Out-String).Trim()
    workingTreeStatusAtHarnessBuild = @(& git -C $repo status --short)
    workingTreeProductionHelperSha256 = (Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256).Hash
    workingTreeSourceNote = 'This is the live working-tree file hash at harness build time, not proof that the selected prebuilt EXE was compiled from that file.'
    harnessSha256 = (Get-FileHash -LiteralPath (Join-Path $PSScriptRoot 'Program.cs') -Algorithm SHA256).Hash
    initializationHarnessSha256 = (Get-FileHash -LiteralPath (Join-Path $PSScriptRoot 'POInitialization.cs') -Algorithm SHA256).Hash
    refillHarnessSha256 = (Get-FileHash -LiteralPath (Join-Path $PSScriptRoot 'PORefill.cs') -Algorithm SHA256).Hash
    projectSha256 = (Get-FileHash -LiteralPath (Join-Path $PSScriptRoot 'Storage.Integration.csproj') -Algorithm SHA256).Hash
    runnerSha256 = (Get-FileHash -LiteralPath $PSCommandPath -Algorithm SHA256).Hash
    buildSlotGrant = $BuildSlotGrant
    directLookupIterationsPerScenario = $Iterations
    bulkIterationsPerScenario = $BulkIterations
    warmupCalls = $Warmup
    expectBaseline = $ExpectBaseline.IsPresent
    msbuild = $MsBuild
}
$provenancePath = Join-Path $output 'provenance.json'
$provenance | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $provenancePath -Encoding UTF8
$buildArgs = @((Join-Path $PSScriptRoot 'Storage.Integration.csproj'), '/t:Rebuild', '/p:Configuration=Release', "/p:AppBinary=$app", "/p:AppDependencyDirectory=$dependencies", "/p:OutputPath=$bin\", "/p:IntermediateOutputPath=$obj\", '/v:minimal', '/nologo')
& $MsBuild @buildArgs 2>&1 | Tee-Object -FilePath (Join-Path $output 'build.log')
$buildExit = $LASTEXITCODE
if ($buildExit -ne 0) { throw "Storage harness build failed with exit code $buildExit. See $output\build.log" }
$copiedApp = Join-Path $bin 'MASAN-SERIALIZATION.exe'
if ((Get-FileHash -LiteralPath $app -Algorithm SHA256).Hash -ne $binaryHash -or (Get-FileHash -LiteralPath $copiedApp -Algorithm SHA256).Hash -ne $binaryHash) { throw 'The application binary changed during preparation or the output copy does not match.' }
$runArgs = @('--label', $Label, '--output', $output, '--source-checkpoint', $SourceCheckpoint, '--provenance', $provenancePath, '--iterations', $Iterations.ToString(), '--bulk-iterations', $BulkIterations.ToString(), '--warmup', $Warmup.ToString())
if ($ExpectBaseline) { $runArgs += '--expect-baseline' }
& (Join-Path $bin 'Storage.Integration.exe') @runArgs 2>&1 | Tee-Object -FilePath (Join-Path $output 'run.log')
$runExit = $LASTEXITCODE
if ($runExit -ne 0) { throw "Storage regression/benchmark failed with exit code $runExit. See $output\results.json and run.log" }
$result = Get-Content -LiteralPath (Join-Path $output 'results.json') -Raw | ConvertFrom-Json
if ($result.ApplicationSha256 -ne $binaryHash) { throw 'Runtime loaded application SHA256 differs from the selected EXE.' }
Write-Output "Storage run finished: $($result.RunOutcome); ProductConformant=$($result.ProductConformant); $output"

