$path = 'D:\DuAn\MASANSolution\code_1000.txt'
$bytes = [System.IO.File]::ReadAllBytes($path)

# Count lines: number of LF (0x0A) bytes
$lfCount = ($bytes | Where-Object { $_ -eq 0x0A }).Count
Write-Host "LF count (lines): $lfCount"

# Look at first 200 bytes vs code.txt (175 bytes)
$codeTxt = [System.IO.File]::ReadAllBytes('D:\DuAn\MASANSolution\code.txt')
Write-Host "code.txt length: $($codeTxt.Length)"

# Take first 175 bytes of new file and compare last 60 bytes to code.txt
$firstLine = $bytes[0..174]
$tail = $firstLine[115..174]
$tailStr = -join ($tail | ForEach-Object { if ($_ -lt 0x20 -or $_ -gt 0x7E) { '[{0:X2}]' -f $_ } else { [char]$_ } })
Write-Host "Line 1 tail (hex): $tailStr"

# Check that header bytes 0-77 match exactly
$match = $true
for ($i = 0; $i -le 77; $i++) {
    if ($bytes[$i] -ne $codeTxt[$i]) { $match = $false; break }
}
Write-Host "Header matches code.txt: $match"

# Check tail pattern: ESC Q 1 ESC Z ETX ESC Z
# Last 5 bytes should be: 1B 51 31 1B 5A 03 1B 5A
$last8 = $bytes[($bytes.Length - 8)..($bytes.Length - 1)]
$last8Hex = ($last8 | ForEach-Object { '{0:X2}' -f $_ }) -join ' '
Write-Host "Last 8 bytes: $last8Hex"
