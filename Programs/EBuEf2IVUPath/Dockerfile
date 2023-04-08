FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /EBuEf2IVUPath

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o output

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /EBuEf2IVUPath
COPY --from=build-env /EBuEf2IVUPath/output .
ENTRYPOINT ["dotnet", "EBuEf2IVUPath.dll"]
