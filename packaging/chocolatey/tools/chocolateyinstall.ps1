$ErrorActionPreference = 'Stop'

$packageArgs = @{
  packageName    = 'mbdc'
  unzipLocation  = "$($MyInvocation.MyCommand.Definition | Split-Path)"
  url64bit       = '{{URL64}}'
  checksum64     = '{{CHECKSUM64}}'
  checksumType64 = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs

$toolsDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Install-BinFile -Name 'mbdc' -Path (Join-Path $toolsDir 'mbdc.exe')
