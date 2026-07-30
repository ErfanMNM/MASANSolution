$path = 'D:\DuAn\MASANSolution\code_1000.txt'
$codeTxt = [System.IO.File]::ReadAllBytes('D:\DuAn\MASANSolution\code.txt')
$bytes = [System.IO.File]::ReadAllBytes($path)

# Count LF
$lfCount = 0
$firstLfIdx = -1
for ($i = 0; $i -lt $bytes.Length; $i++) {
    if ($bytes[$i] -eq 0x0A) {
        $lfCount++
        if ($firstLfIdx -lt 0) { $firstLfIdx = $i }
    }
}
Write-Host "Lines (LF count): $lfCount"
Write-Host "First line length: $($firstLfIdx + 1) bytes (incl CRLF)"

# Compare header of first line (first 128 bytes) with code.txt
$hdr = $true
for ($i = 0; $i -lt 128; $i++) {
    if ($bytes[$i] -ne $codeTxt[$i]) { $hdr = $false; break }
}
Write-Host "Header (first 128 bytes) matches code.txt: $hdr"

# Compare suffix of first line (last 9 bytes before LF) with code.txt
$idx = $firstLfIdx - 9
$suf = $true
for ($i = 0; $i -lt 9; $i++) {
    if ($bytes[$idx + $i] -ne $codeTxt[172 + $i]) { $suf = $false; break }
}
Write-Host "Suffix (last 9 bytes) matches code.txt: $suf"

# Display line 1 as text (replace ESC with ^[ for readability)
$line1 = ''
for ($i = 0; $i -lt $firstLfIdx; $i++) {
    $b = $bytes[$i]
    if ($b -lt 0x20 -or $b -gt 0x7E) { $line1 += '[0x{0:X2}]' -f $b }
    else { $line1 += [char]$b }
}
Write-Host ""
Write-Host "Line 1 (annotated): $line1"

# Show lines 1, 2, 500, 1000 in raw form
$lines = [System.IO.File]::ReadAllLines($path)
foreach ($n in 1, 2, 500, 1000) {
    $l = $lines[$n - 1]
    Write-Host "--- Line $n (length $($l.Length)) ---"
    Write-Host $l
    Write-Host ""
}
