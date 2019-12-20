REM SET FRAMEWORK=linux-x64
SET FRAMEWORK=linux-arm

SET FILENAME=EBuEf2IVUCore.*.*.*.%FRAMEWORK%.deb
SET SOURCEDIR=.\EBuef2IVUCore\bin\Release\netcoreapp2.2\%FRAMEWORK%
SET TARGETDIR=.
SET DROPBOXDIR=%USERPROFILE%\Dropbox\Public\EBuEf

del /q %TARGETDIR%\%FILENAME%
del /q %DROPBOXDIR%\%FILENAME%

dotnet publish -c Release -r %FRAMEWORK% -f netstandard2.0 --self-contained
dotnet deb -c Release -r %FRAMEWORK% -f netstandard2.0

xcopy /y %SOURCEDIR%\%FILENAME% %TARGETDIR%
xcopy /y %SOURCEDIR%\%FILENAME% %DROPBOXDIR%

pause