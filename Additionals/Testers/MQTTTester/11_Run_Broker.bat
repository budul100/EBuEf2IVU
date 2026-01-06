@ECHO OFF

SET MOSQUITTOPATH=C:\Program Files\mosquitto

ECHO.
ECHO Starting Mosquitto MQTT broker.
ECHO Please download Mosquitto from https://mosquitto.org/ if missing.
ECHO.

"%MOSQUITTOPATH%\mosquitto.exe" -v

PAUSE