@echo off

SET PATH=%PATH%;.

REM Net 6.0 has to be used until https://github.com/quamotion/dotnet-packaging is updated for Net 8.0 or later

SET FRAMEWORK=net6.0
SET RUNTIME1=linux-x64
SET RUNTIME2=linux-arm64
SET RUNTIME3=linux-arm

SET ScriptsDir=.\_Scripts\Build

SET TARGETDIR=.
SET SOURCEDIR1=.\bin\Release\%FRAMEWORK%\%RUNTIME1%
SET SOURCEDIR2=.\bin\Release\%FRAMEWORK%\%RUNTIME2%
SET SOURCEDIR3=.\bin\Release\%FRAMEWORK%\%RUNTIME3%
SET DROPBOXDIR=%USERPROFILE%\Dropbox\Public\EBuEf

echo.
echo Please make sure that DNS in your WSL is working. Check file /etc/resolv.conf in case of issues.
echo See also https://stackoverflow.com/questions/62314789/no-internet-connection-on-wsl-ubuntu-windows-subsystem-for-linux/67756837#67756837
echo.

echo.
echo ##### Create EBuEf2IVU #####
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

SET SERVICENAME=ebuef2ivucrew

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

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME1%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME1%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR1%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR1%\%DEBFILE% %DROPBOXDIR%

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME2%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME2%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR2%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR2%\%DEBFILE% %DROPBOXDIR%

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME3%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME3%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR3%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR3%\%DEBFILE% %DROPBOXDIR%

powershell "%ScriptsDir%\Update_VersionBuild.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
echo.

REM **********************************************************************************

SET SERVICENAME=ebuef2ivupath

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

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME1%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME1%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR1%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR1%\%DEBFILE% %DROPBOXDIR%

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME2%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME2%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR2%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR2%\%DEBFILE% %DROPBOXDIR%

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME3%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME3%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR3%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR3%\%DEBFILE% %DROPBOXDIR%

powershell "%ScriptsDir%\Update_VersionBuild.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
echo.

REM **********************************************************************************

SET SERVICENAME=ebuef2ivuvehicle

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

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME1%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME1%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR1%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR1%\%DEBFILE% %DROPBOXDIR%

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME2%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME2%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR2%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR2%\%DEBFILE% %DROPBOXDIR%

SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME3%.deb

wsl -e bash ./Create_Linux.sh %SERVICENAME% %FRAMEWORK% %RUNTIME3%
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR3%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR3%\%DEBFILE% %DROPBOXDIR%

powershell "%ScriptsDir%\Update_VersionBuild.ps1 -projectPaths '.\Programs\%SERVICENAME%\%SERVICENAME%.csproj'"
echo.

REM **********************************************************************************

echo.
echo.
echo ***
echo *** Restore installation ***
echo ***
echo.

dotnet restore

REM wsl -e bash ./Create_Docker.sh
REM echo.

echo.
echo Run the following commands in your Linux subsystem to create the repective Docker files.
echo.
echo cd /mnt/c/Users/mgr/Entwicklung/EBuEf2IVU
echo docker login git.tu-berlin.de:5000
echo sh Create_Docker.sh
echo.
echo Check the name server on debian in case of git connection errors: <https://stackoverflow.com/a/67756837/5103334>
echo.

echo.
pause