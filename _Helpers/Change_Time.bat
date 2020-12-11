@echo off

SET RestartService1=Interface-Server_EBUEF_EBuEf@IVU.World
SET RestartService2=Interface-Worker_EBUEF_EBuEf@IVU.World

echo.
echo +++++ Anpassung EBuEf-Uhrzeit +++++
echo.

:check_permissions

net session >nul 2>&1
if %errorLevel% == 0 goto run_script

echo ACHTUNG! Dieses Skript kann nur mit Administrationsrechten ausgefuehrt werden.

goto end_script
	
:run_script

ECHO.
ECHO Soll fuer den Rechner eine neue Uhrzeit gesetzt werden oder soll die Uhrzeit mit der aktuellen Zeit synchronisiert werden?
@CHOICE /C:ns /M "Bitte die Auswahl OHNE Eingabetaste angeben: [N]eu setzen | [S]ynchronisieren."

ECHO.
ECHO.

IF ERRORLEVEL 2 GOTO sync_time
IF ERRORLEVEL 1 GOTO set_time

:sync_time:

ECHO.
ECHO Der Rechner wird auf die aktuelle Uhrzeit synchronisiert.

REG add "HKLM\SYSTEM\CurrentControlSet\Services\W32Time\Parameters" /f /v "Type" /t REG_SZ /d "STP"

ECHO.
w32tm /resync /nowait

if %errorLevel% == 0 (
	goto restart_services 
)
else (
	goto end_script
)

:set_time

ECHO.
ECHO Bitte die Stunde der gewuenschten neuen Uhrzeit als zweistellige Zahl angeben.
ECHO.
set "Stunde=07"
set /p "Stunde=Bitte Stunde angeben oder [ENTER] druecken fuer Standardwert [%Stunde%]: "

ECHO.
ECHO Bitte die Minute der gewuenschten neuen Uhrzeit als zweistellige Zahl angeben.
ECHO.
set "Minute=58"
set /p "Minute=Bitte Minute angeben oder [ENTER] druecken fuer Standardwert [%Minute%]: "

REG add "HKLM\SYSTEM\CurrentControlSet\Services\W32Time\Parameters" /f /v "Type" /t REG_SZ /d "NoSync"

ECHO.
TIME %Stunde%:%Minute%:00

if %errorLevel% == 0 goto restart_services 
else goto end_script

:restart_services

ECHO.
ECHO Die relevanten Windows-Dienste werden neu gestartet.

ECHO.
net stop %RestartService1% 
net start %RestartService1%

ECHO.
net stop %RestartService2% 
net start %RestartService2%

ECHO.
ECHO Das Setzen der Uhrzeit und der Neustart der Dienste wurde durchgefuehrt.

goto end_script

:end_script

pause >nul
