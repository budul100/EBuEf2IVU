@ECHO OFF

SET MQTTBROKER=localhost
SET MQTTTOPIC=mqtt4453
SET MQTTMESSAGE={"zugnummer":"4033","decoder":null,"simulationszeit":"1970-01-01 21:00:00","betriebsstelle":"XAB","signaltyp":"ESig","start_gleis":"1","ziel_gleis":"2","modus":"istzeit"}

ECHO.
ECHO Send status message.
ECHO.

ECHO.
python.exe MQTTSender.py
ECHO.

PAUSE