pushd ..

dotnet restore .\EBuEf2IVUCrew\EBuEf2IVUCrew.csproj 
dotnet build -c release -f netcoreapp3.1 .\EBuEf2IVUCrew\EBuEf2IVUCrew.csproj 

dotnet restore .\EBuEf2IVUVehicle\EBuEf2IVUVehicle.csproj 
dotnet build -c release -f netcoreapp3.1 .\EBuEf2IVUVehicle\EBuEf2IVUVehicle.csproj 

pause