$ErrorActionPreference = 'Stop'

$logPath = 'C:\Users\Denis\Desktop\Razvoj web aplikacija u ASP.NET MVC tehnologiji\projekt\agent_log.txt'
$payload = [Console]::In.ReadToEnd().Trim()

if ([string]::IsNullOrWhiteSpace($payload)) {
    $payload = '{"note":"empty stdin payload"}'
}

$entry = "[{0}] {1}" -f (Get-Date -Format o), $payload
Add-Content -Path $logPath -Value $entry
