SET SDKFolder=C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.8 Tools

"%SDKFolder%\xsd.exe" ".\20.3\TrafficNetworkImport.xsd" /classes

REM "%SDKFolder%\wsdl.exe" /language:CS /namespace:EBuEf2IVUVehicleTests /out:. /protocol:SOAP /serverinterface ".\20.3\RealTimeInformationImportFacade.wsdl"