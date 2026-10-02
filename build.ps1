$ErrorActionPreference = 'Stop'
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
}
if (-not (Test-Path -LiteralPath $compiler)) { throw 'The Windows .NET Framework C# compiler was not found.' }
& $compiler /nologo /target:winexe /optimize+ /r:System.Drawing.dll /r:System.Windows.Forms.dll "/out:$PSScriptRoot\SnakeGame.exe" "$PSScriptRoot\Program.cs"
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
Write-Host "Built $PSScriptRoot\SnakeGame.exe"
