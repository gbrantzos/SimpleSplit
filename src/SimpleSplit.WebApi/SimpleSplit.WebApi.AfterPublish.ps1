# Get publish folder from parameters
$PublishDir = $args[0]
$developmentSettings = Join-Path $PublishDir -ChildPath "appsettings.Development.json"

if (Test-Path $developmentSettings) {
    Remove-Item $developmentSettings
}

$deployZip = Join-Path (Get-Item $PSScriptRoot).Parent.Parent  -ChildPath "SimpleSplit.Deploy.$(Get-Date -Format yyyyMMdd).zip"
if (Test-Path $deployZip) {
    Remove-Item $deployZip
}

Compress-Archive -Path "$PublishDir*.*" -DestinationPath $deployZip
