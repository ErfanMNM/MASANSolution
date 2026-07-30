$path = 'D:\DuAn\MASANSolution\code_1000.txt'
$bytes = [System.IO.File]::ReadAllBytes($path)
$text  = [System.IO.File]::ReadAllText($path)
$lines = $text -split "`r?`n"

Write-Host "Total bytes: $($bytes.Length)"
Write-Host "Line count:  $($lines.Count)"
Write-Host "Line 1 length: $($lines[0].Length)"
Write-Host "Line 1000 length: $($lines[999].Length)"

# Check every line is the same length (= 175)
$badLen = $lines | Where-Object { $_.Length -ne 175 }
Write-Host "Lines with bad length: $(@($badLen).Count)"

# Check header of every line is identical
$hdr = $lines[0].Substring(0, 100)
$badHdr = $lines | Where-Object { $_.Substring(0, 100) -ne $hdr }
Write-Host "Lines with mismatched header: $(@($badHdr).Count)"

# Check AI 192 prefix exists on every line
$bad192 = $lines | Where-Object { $_ -notmatch '192[A-Za-z0-9+/]{43}=' }
Write-Host "Lines with bad AI 192: $(@($bad192).Count)"

# Show a sample
Write-Host ""
Write-Host "=== Line 1 (tail) ==="
Write-Host $lines[0].Substring(110)
Write-Host ""
Write-Host "=== Line 2 (tail) ==="
Write-Host $lines[1].Substring(110)
Write-Host ""
Write-Host "=== Line 1000 (tail) ==="
Write-Host $lines[999].Substring(110)
