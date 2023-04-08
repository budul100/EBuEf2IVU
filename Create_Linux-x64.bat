@echo off

SET PATH=%PATH%;.

SET FRAMEWORK=net6.0
SET RUNTIME=linux-x64

SET ScriptsDir=.\_Scripts\Build

SET TARGETDIR=.
SET SOURCEDIR=.\bin\Release\%FRAMEWORK%\%RUNTIME%
SET DROPBOXDIR=%USERPROFILE%\Dropbox\Public\EBuEf

echo.
echo ##### Create EBuEf2IVU #####
echo.
echo Create any docker files first by starting the Debian bash and running sh Create_Docker.sh
echo.

echo.
CHOICE /C mb /N /M "Shall the [b]uild (x.x._X_.0) or the [m]inor version (x._X_.0.0) be increased?"
SET VERSIONSELECTION=%ERRORLEVEL%
echo.

echo.
echo ***
echo *** Clean projects ***
echo ***
echo.

powershell "%ScriptsDir%\CleanFolders.ps1 -baseDir ."

DEL /q %TARGETDIR%\*.deb

REM **********************************************************************************

SET SERVICENAME=EBuEf2IVUCrew
SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

echo.
echo.
echo ***
echo *** Build %SERVICENAME% ***
echo ***
echo.

if /i "%VERSIONSELECTION%" == "1" (

	powershell "%ScriptsDir%\Update_VersionMinor.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
	echo.

)

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

wsl -e bash ./Create_Linux-x64.sh %SERVICENAME% %FRAMEWORK% %RUNTIME%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %DROPBOXDIR%

powershell "%ScriptsDir%\Update_VersionBuild.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
echo.

REM **********************************************************************************

SET SERVICENAME=EBuEf2IVUPath
SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

echo.
echo.
echo ***
echo *** Build %SERVICENAME% ***
echo ***
echo.

if /i "%VERSIONSELECTION%" == "1" (

	powershell "%ScriptsDir%\Update_VersionMinor.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
	echo.

)

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

wsl -e bash ./Create_Linux-x64.sh %SERVICENAME% %FRAMEWORK% %RUNTIME%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %DROPBOXDIR%

powershell "%ScriptsDir%\Update_VersionBuild.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
echo.

REM **********************************************************************************

SET SERVICENAME=EBuEf2IVUVehicle
SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

echo.
echo.
echo ***
echo *** Build %SERVICENAME% ***
echo ***
echo.

if /i "%VERSIONSELECTION%" == "1" (

	powershell "%ScriptsDir%\Update_VersionMinor.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
	echo.

)

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

wsl -e bash ./Create_Linux-x64.sh %SERVICENAME% %FRAMEWORK% %RUNTIME%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %DROPBOXDIR%

powershell "%ScriptsDir%\Update_VersionBuild.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
echo.

REM **********************************************************************************

dotnet restore

echo.
pause