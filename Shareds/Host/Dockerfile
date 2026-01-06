# Unter https://aka.ms/customizecontainer erfahren Sie, wie Sie Ihren Debugcontainer anpassen und wie Visual Studio dieses Dockerfile verwendet, um Ihre Images für ein schnelleres Debuggen zu erstellen.

# Diese Stufe wird verwendet, wenn sie von VS im Schnellmodus ausgeführt wird (Standardeinstellung für Debugkonfiguration).
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Diese Stufe wird zum Erstellen des Dienstprojekts verwendet.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiere alle Projekte (csproj-Dateien für besseres Layer-Caching)
# Shareds
COPY ["Shareds/Commons/Commons.csproj", "Shareds/Commons/"]
COPY ["Shareds/Host/Host.csproj", "Shareds/Host/"]

# Workers
COPY ["Workers/Base/Base.csproj", "Workers/Base/"]
COPY ["Workers/Crew/Crew.csproj", "Workers/Crew/"]
COPY ["Workers/Path/Path.csproj", "Workers/Path/"]
COPY ["Workers/Vehicle/Vehicle.csproj", "Workers/Vehicle/"]

# Services
COPY ["Services/CrewChecker/CrewChecker.csproj", "Services/CrewChecker/"]
COPY ["Services/DatabaseConnector/DatabaseConnector.csproj", "Services/DatabaseConnector/"]
COPY ["Services/Message2LegConverter/Message2LegConverter.csproj", "Services/Message2LegConverter/"]
COPY ["Services/Message2TrainRunConverter/Message2TrainRunConverter.csproj", "Services/Message2TrainRunConverter/"]
COPY ["Services/MQTTReceiver/MQTTReceiver.csproj", "Services/MQTTReceiver/"]
COPY ["Services/MulticastReceiver/MulticastReceiver.csproj", "Services/MulticastReceiver/"]
COPY ["Services/RealtimeSender/RealtimeSender.csproj", "Services/RealtimeSender/"]
COPY ["Services/RealtimeSenderIS/RealtimeSenderIS.csproj", "Services/RealtimeSenderIS/"]
COPY ["Services/StateHandler/StateHandler.csproj", "Services/StateHandler/"]
COPY ["Services/TrainPathSender/TrainPathSender.csproj", "Services/TrainPathSender/"]

# Restore für das Host-Projekt (lädt alle Dependencies durch Projektreferenzen)
RUN dotnet restore "Shareds/Host/Host.csproj"

# Kopiere gesamten Source-Code
COPY . .

# Build des Host-Projekts
WORKDIR "/src/Shareds/Host"
RUN dotnet build "Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Diese Stufe wird verwendet, um das Dienstprojekt zu veröffentlichen, das in die letzte Phase kopiert werden soll.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/Shareds/Host"
RUN dotnet publish "Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Diese Stufe wird in der Produktion oder bei Ausführung von VS im regulären Modus verwendet (Standard, wenn die Debugkonfiguration nicht verwendet wird).
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Kopiere Settings-Dateien ins Image
COPY --from=build /src/Shareds/Host/Settings ./Settings

# Erstelle non-root User für Sicherheit
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "ebuef2ivu.dll"]