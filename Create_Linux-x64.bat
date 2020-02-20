@echo off

SET FRAMEWORK=netcoreapp3.1
SET RUNTIME=linux-x64

SET TARGETDIR=.
SET SOURCEDIR=.\bin\Release\%FRAMEWORK%\%RUNTIME%
SET DROPBOXDIR=%USERPROFILE%\Dropbox\Public\EBuEf

echo ***
echo *** Clean solution ***
echo ***

FOR /F "tokens=*" %%G IN ('DIR /B /AD /S bin') DO RMDIR /S /Q "%%G"
FOR /F "tokens=*" %%G IN ('DIR /B /AD /S obj') DO RMDIR /S /Q "%%G"

DEL /q %TARGETDIR%\*.deb

SET SERVICENAME=EBuEf2IVUCrew
SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

echo ***
echo *** Build %SERVICENAME% ***
echo ***

dotnet restore .\%SERVICENAME%\%SERVICENAME%.csproj 
dotnet build -c release -f %FRAMEWORK% .\%SERVICENAME%\%SERVICENAME%.csproj 

BASH -c "sh Create_Linux-x64.sh %SERVICENAME%"

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %TARGETDIR%
XCOPY /y .\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %DROPBOXDIR%


SET SERVICENAME=EBuEf2IVUVehicle
SET DEBFILE=%SERVICENAME%.*.*.*.%RUNTIME%.deb

echo ***
echo *** Build %SERVICENAME% ***
echo ***

dotnet restore .\%SERVICENAME%\%SERVICENAME%.csproj 
dotnet build -c release -f %FRAMEWORK% .\%SERVICENAME%\%SERVICENAME%.csproj 

BASH -c "sh Create_Linux-x64.sh %SERVICENAME%"

DEL /q %DROPBOXDIR%\%DEBFILE%

XCOPY /y .\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %TARGETDIR%
XCOPY /y .\%SERVICENAME%\%SOURCEDIR%\%DEBFILE% %DROPBOXDIR%

dotnet restore

pause