[CmdletBinding(DefaultParameterSetName = 'Build')]
param(
    [Parameter(ParameterSetName = 'Build')]
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Debug'
)


$srcDir = Join-Path $PSScriptRoot 'src\PPCLI.PowerShell.Predictor'
dotnet publish -c $Configuration $srcDir

Write-Host "`nThe module 'PPCLI.PowerShell.Predictor' is published to 'PPCLI.PowerShell.Predictor.Module\PPCLI.PowerShell.Predictor'`n" -ForegroundColor Green