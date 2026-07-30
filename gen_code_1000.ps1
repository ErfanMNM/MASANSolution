# Generate 1000 SATO printer commands, randomising the base64 payload
# inside AI 192 only. Output mimics code.txt (raw text, ESC = 0x1B, ETX = 0x03).

$esc = [char]0x1B
$etx = [char]0x03

# Fixed prefix (78 bytes): everything before the data field
$prefix = "$esc`A$esc`A3V+00000H+0000$esc`CS4$esc`#F7$esc`A1V00300H0300$esc`%0$esc`H0045$esc`V00044$esc`2D51,06,06,000,000$esc`DN0089,"

# Fixed data prefix (45 bytes): from "10" up to and including "192"
# Bytes 78..125 of code.txt -> 48 bytes actually. Let me extract it directly.
$codeTxt = [System.IO.File]::ReadAllBytes('D:\DuAn\MASANSolution\code.txt')
# header is 78 bytes (up to "DN0089,")
# Data prefix: from byte 83 (first byte of data, right after "DN0089,") up to byte 127 (right before base64)
# That's 83..127 = 45 bytes ending with "192"
$dataPrefix = -join ($codeTxt[83..127] | ForEach-Object { [char]$_ })
# base64 is bytes 128..171 = 44 chars
# suffix is bytes 172..180 = 9 bytes
$suffix = -join ($codeTxt[172..180] | ForEach-Object { [char]$_ })

Write-Host "dataPrefix length: $([System.Text.Encoding]::ASCII.GetByteCount($dataPrefix))"
Write-Host "suffix length:     $([System.Text.Encoding]::ASCII.GetByteCount($suffix))"
Write-Host "dataPrefix: $dataPrefix"
Write-Host "suffix: $suffix"

$base64Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"
$rand = New-Object System.Random

$lines = New-Object System.Collections.Generic.List[string]
for ($i = 0; $i -lt 1000; $i++) {
    # 43 random chars + 1 '=' pad
    $sb = New-Object System.Text.StringBuilder
    for ($j = 0; $j -lt 43; $j++) {
        [void]$sb.Append($base64Alphabet[$rand.Next(0, $base64Alphabet.Length)])
    }
    [void]$sb.Append('=')
    $payload = $sb.ToString()

    $line = "$prefix$dataPrefix$payload$suffix"
    $lines.Add($line)
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllLines("D:\DuAn\MASANSolution\code_1000.txt", $lines, $utf8NoBom)

Write-Host "Wrote $($lines.Count) lines to code_1000.txt"
