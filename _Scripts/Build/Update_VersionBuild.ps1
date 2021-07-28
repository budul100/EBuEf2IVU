param (
	[string[]] $projectPaths
)

$scriptpath = split-path -parent $MyInvocation.MyCommand.Definition

. $scriptpath\Get_VersionCurrent.ps1

$currentVersion = Get-ProjectVersion -projectPath $projectPaths[0]

if ($currentVersion -cmatch '(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)(\.(?<revision>\d+))?') 
{
	$updatedVersion = ($matches['major'] + "." + $matches['minor'] + "." + ([int]$matches['build'] + 1)) 

	if (!($null -eq $matches['revision']))
	{
		$updatedVersion = $updatedVersion + ".0"
	}

} 
else 
{

	$updatedVersion = $currentVersion

}

. $scriptpath\Set_FileContent.ps1

foreach ($path in $projectPaths)
{
	Set-FileContent -path $path -replace '(?<=<AssemblyVersion>)[^<]+' -replaceWith "$updatedVersion"
}

exit 0