function Set-FileContent([String] $path, [String] $replace, [String] $replaceWith)
{
	(Get-Content $path) | Foreach-Object {$_ -replace $replace, $replaceWith} | Out-File -Encoding "Default" $path
}