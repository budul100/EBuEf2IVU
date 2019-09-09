SET FILENAME=EBuEf2IVUCore.*.*.*.linux-x64.deb
SET SOURCEDIR=.\EBuef2IVUCore\bin\Release\netcoreapp2.2\linux-x64
SET TARGETDIR=.
SET DROPBOXDIR=%USERPROFILE%\Dropbox\Public\EBuEf

del /q %TARGETDIR%\%FILENAME%
del /q %DROPBOXDIR%\%FILENAME%

dotnet deb -c Release -r linux-x64 -f netcoreapp2.2

xcopy /y %SOURCEDIR%\%FILENAME% %TARGETDIR%
xcopy /y %SOURCEDIR%\%FILENAME% %DROPBOXDIR%

pause