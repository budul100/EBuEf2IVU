@ECHO OFF
SETLOCAL EnableDelayedExpansion

:: ========================================================
:: Setup and Navigation
:: ========================================================

:: Navigate to the target directory (2 levels up from the script's location)
PUSHD "%~dp0..\.."

:: Store current path for display purposes
SET "TARGET_DIR=%CD%"

ECHO.
ECHO ========================================================
ECHO  Hiding dot-files and dot-folders recursively in:
ECHO  "%TARGET_DIR%"
ECHO ========================================================
ECHO.

:: ========================================================
:: 1. Process FILES
:: ========================================================
:: Loop recursively (/R) through ALL files (*)
FOR /R "%TARGET_DIR%" %%F IN (*) DO (
    SET "FILENAME=%%~nxF"
    
    :: Check if the first character (substring 0, length 1) is a dot "."
    IF "!FILENAME:~0,1!"=="." (
        ECHO [FILE]   Hiding "%%~nxF" ...
        ATTRIB +H "%%F"
    )
)

:: ========================================================
:: 2. Process FOLDERS
:: ========================================================
:: Loop recursively through directories (/D)
FOR /R "%TARGET_DIR%" /D %%D IN (*) DO (
    SET "DIRNAME=%%~nxD"
    
    :: Check if the first character is a dot "."
    IF "!DIRNAME:~0,1!"=="." (
        ECHO [FOLDER] Hiding "%%~nxD" ...
        ATTRIB +H "%%D"
    )
)

:: Return to original directory
POPD

ECHO.
ECHO Done.
PAUSE