param (
	[string] $baseDir
)

$baseDir = if ([System.IO.Path]::IsPathRooted("$baseDir\")) {"$baseDir\"} else {[System.IO.Path]::GetFullPath((Join-Path $pwd "$baseDir\"))} 

Write-Host "All subfolders on $baseDir cleaned now."

$currentFolder = $baseDir

do 
{
  $dirs = Get-ChildItem $currentFolder -directory -recurse | Where-Object { (Get-ChildItem $_.fullName).count -eq 0 } | Select-Object -expandproperty FullName
  $dirs | Foreach-Object { Remove-Item $_ }
} 
while ($dirs.count -gt 0)

Get-ChildItem $baseDir -include bin,obj -Recurse | ForEach-Object ($_) { Remove-Item $_.FullName -Force -Recurse }