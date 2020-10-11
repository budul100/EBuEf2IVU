@echo off

SET FRAMEWORK=netcoreapp3.1
SET RUNTIME=linux-x64

SET TARGETDIR=.
SET SOURCEDIR=.\bin\Release\%FRAMEWORK%\%RUNTIME%
SET DROPBOXDIR=%USERPROFILE%\Dropbox\Public\EBuEf

echo.
echo.
echo ***
echo *** Clean solution ***
echo ***
echo.

FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

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

dotnet restore .\Programs\%SERVICENAME%\%SERVICENAME%.csproj 
echo.

dotnet build -c release -f %FRAMEWORK% .\Programs\%SERVICENAME%\%SERVICENAME%.csproj 
echo.

BASH -c "sh Create_Linux-x64.sh %SERVICENAME%"
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %DROPBOXDIR%

REM **********************************************************************************

SET SERVICENAME=EBuEf2IVUPath
SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

echo.
echo.
echo ***
echo *** Build %SERVICENAME% ***
echo ***
echo.

dotnet restore .\Programs\%SERVICENAME%\%SERVICENAME%.csproj 
echo.

dotnet build -c release -f %FRAMEWORK% .\Programs\%SERVICENAME%\%SERVICENAME%.csproj 
echo.

BASH -c "sh Create_Linux-x64.sh %SERVICENAME%"
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %DROPBOXDIR%

REM **********************************************************************************

SET SERVICENAME=EBuEf2IVUVehicle
SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

echo.
echo.
echo ***
echo *** Build %SERVICENAME% ***
echo ***
echo.

dotnet restore .\Programs\%SERVICENAME%\%SERVICENAME%.csproj 
echo.

dotnet build -c release -f %FRAMEWORK% .\Programs\%SERVICENAME%\%SERVICENAME%.csproj 
echo.

BASH -c "sh Create_Linux-x64.sh %SERVICENAME%"
echo.

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %TARGETDIR%
XCOPY /y .\Programs\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %DROPBOXDIR%

REM **********************************************************************************

dotnet restore

pause