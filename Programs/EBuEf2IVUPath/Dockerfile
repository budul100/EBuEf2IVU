# ----------- Build Stage -----------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /EBuEf2IVUPath

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o output

# ----------- Runtime Stage -----------
FROM mcr.microsoft.com/dotnet/runtime:6.0

# Installiere ca-certificates
RUN apt-get update && apt-get install -y ca-certificates \
    && rm -rf /var/lib/apt/lists/*

# Kopiere das Zertifikat ins richtige Verzeichnis
COPY ebuef-ca.crt /usr/local/share/ca-certificates/

# Zertifikate aktualisieren
RUN update-ca-certificates

# App-Files kopieren und starten
WORKDIR /EBuEf2IVUPath
COPY --from=build-env /EBuEf2IVUPath/output .

ENTRYPOINT ["dotnet", "EBuEf2IVUPath.dll"]