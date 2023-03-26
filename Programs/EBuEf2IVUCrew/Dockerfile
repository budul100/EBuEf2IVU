FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /Programs/EBuEf2IVUCrew

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0
WORKDIR /Programs/EBuEf2IVUCrew
COPY --from=build-env /Programs/EBuEf2IVUCrew/out .
ENTRYPOINT ["dotnet", "EBuEf2IVUCrew.dll"]
