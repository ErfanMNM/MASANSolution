$path = 'D:\DuAn\MASANSolution\code_1000.txt'
$codeTxt = [System.IO.File]::ReadAllBytes('D:\DuAn\MASANSolution\code.txt')
$bytes = [System.IO.File]::ReadAllBytes($path)

# Split file by LF and verify each line's content length (excluding CRLF)
$lines = @()
$start = 0
for ($i = 0; $i -lt $bytes.Length; $i++) {
    if ($bytes[$i] -eq 0x0A) {
        $end = $i - 1
        if ($end -ge $start -and $bytes[$end] -eq 0x0D) { $end-- }
        $lineBytes = $bytes[$start..$end]
        $lines += , $lineBytes
        $start = $i + 1
    }
}
Write-Host "Lines: $($lines.Count)"
Write-Host "code.txt length: $($codeTxt.Length)"
Write-Host "First line length: $($lines[0].Length)"
Write-Host "Last line length:  $($lines[-1].Length)"

# Check uniqueness: every line except base64 should be identical to code.txt
$prefixBytes = $codeTxt[0..82]   # prefix including "ESC DN0089," (83 bytes)
$dataPrefixBytes = $codeTxt[83..127]  # the fixed data prefix (45 bytes: 10...RunM, ESC 1, 91EE11, ESC 1, 192)
$suffixBytes = $codeTxt[172..($codeTxt.Length - 1)]  # the suffix

Write-Host "prefix bytes: $($prefixBytes.Length)"
Write-Host "dataPrefix bytes: $($dataPrefixBytes.Length)"
Write-Host "suffix bytes: $($suffixBytes.Length)"

$bad = 0
$base64Samples = @()
for ($i = 0; $i -lt $lines.Count; $i++) {
    $L = $lines[$i]
    if ($L.Length -ne $codeTxt.Length) {
        $bad++
        if ($bad -le 3) { Write-Host "Line $($i+1) length mismatch: $($L.Length) vs $($codeTxt.Length)" }
        continue
    }
    # Check prefix
    $lineOk = $true
    for ($j = 0; $j -lt $prefixBytes.Length; $j++) {
        if ($L[$j] -ne $prefixBytes[$j]) { $lineOk = $false; break }
    }
    # Check data prefix
    if ($lineOk) {
        for ($j = 0; $j -lt $dataPrefixBytes.Length; $j++) {
            if ($L[83 + $j] -ne $dataPrefixBytes[$j]) { $lineOk = $false; break }
        }
    }
    # Check suffix
    if ($lineOk) {
        $sufStart = $L.Length - $suffixBytes.Length
        for ($j = 0; $j -lt $suffixBytes.Length; $j++) {
            if ($L[$sufStart + $j] -ne $suffixBytes[$j]) { $lineOk = $false; break }
        }
    }
    if (-not $lineOk) { $bad++ }
    # Save base64 sample
    if ($i -lt 5) {
        $b64 = -join ($L[128..171] | ForEach-Object { if ($_ -lt 0x20 -or $_ -gt 0x7E) { '?' } else { [char]$_ } })
        $base64Samples += $b64
    }
}
Write-Host "Bad lines: $bad"
Write-Host ""
Write-Host "Base64 samples (line 1-5):"
foreach ($s in $base64Samples) { Write-Host "  $s" }
