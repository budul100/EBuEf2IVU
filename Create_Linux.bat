SET FILENAME=EBuEf2IVUCore.*.*.*.linux-arm.deb
SET SOURCEDIR=.\EBuef2IVUCore\bin\Release\netcoreapp2.2\linux-arm
SET TARGETDIR=.
SET DROPBOXDIR=%USERPROFILE%\Dropbox\Public\EBuEf

del /q %TARGETDIR%\%FILENAME%
del /q %DROPBOXDIR%\%FILENAME%

REM dotnet publish -c Release -r linux-arm -f netcoreapp2.2 --self-contained
dotnet deb -c Release -r linux-arm -f netcoreapp2.2

xcopy /y %SOURCEDIR%\%FILENAME% %TARGETDIR%
xcopy /y %SOURCEDIR%\%FILENAME% %DROPBOXDIR%

pause