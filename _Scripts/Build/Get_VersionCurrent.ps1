function Get-ProjectVersion([string] $projectPath)
{
	$result = $null

	$projectInfo = Get-Content -Path $projectPath

	foreach($line in $projectInfo)
	{
		if ($line -cmatch '<AssemblyVersion>(?<major>\d+)(\.(?<minor>\d+))(\.(?<build>\d+))(\.(?<revision>\d+))?</AssemblyVersion>') 
		{
			$versionMajor = $matches['major']
			$versionMinor = $matches['minor']
			$versionBuild = $matches['build']
			$versionRevision = $matches['revision']

			$result = $versionMajor 

			if ($null -eq $versionMinor)
			{
				$versionMinor = '0'
			}
			$result += '.' + $versionMinor 

			if ($null -eq $versionBuild)
			{
				$versionBuild = '0'
			}
			$result += '.' + $versionBuild 

			if (!($null -eq $versionRevision))
			{
				$result += '.' + $versionRevision 
			}

			return $result
		}
	}

	return $result
}