FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /EBuEf2IVUCrew

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o output

FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /EBuEf2IVUCrew
COPY --from=build-env /EBuEf2IVUCrew/output .
ENTRYPOINT ["dotnet", "EBuEf2IVUCrew.dll"]
