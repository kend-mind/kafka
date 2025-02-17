start /min cmd /c "C:\services\kafka\bin\windows\zookeeper-server-start.bat C:\services\kafka\config\zookeeper.properties"
timeout /t 5
start /min cmd /c "C:\services\kafka\bin\windows\kafka-server-start.bat C:\services\kafka\config\server.properties"