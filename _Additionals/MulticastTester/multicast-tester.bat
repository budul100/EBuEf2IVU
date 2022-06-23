@echo off

REM Real time message sender
start "Real-time messages" cmd /c "echo. & echo Real time message sender - Source https://github.com/anastyn/multicast-tester & echo. & java -jar multicast-tester-1.0-SNAPSHOT.jar --sender --group=224.0.0.7 --port=4453"

REM Status message sender
echo. & echo Status message sender - Source https://github.com/anastyn/multicast-tester & echo. & java -jar multicast-tester-1.0-SNAPSHOT.jar --sender --group=224.0.0.8 --port=4454