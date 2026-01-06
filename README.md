# EBuEf2IVU Docker Build System

## Inhaltsverzeichnis

- [Voraussetzungen](#voraussetzungen)
- [Verwendung](#verwendung)
  - [Grundlegende Befehle](#grundlegende-befehle)
  - [Parameter](#parameter)
  - [Workflows](#workflows)
- [Features](#features)
- [Beispiele](#beispiele)
- [Troubleshooting](#troubleshooting)
- [FAQ](#faq)

---

## Voraussetzungen

### Software

| Komponente             | Version  | Beschreibung                 |
| ---------------------- | -------- | ---------------------------- |
| **Windows PowerShell** | 5.1+     | Script-Runtime               |
| **.NET SDK**           | 8.0+     | Zum Kompilieren der Solution |
| **Docker Desktop**     | Latest   | Container-Runtime            |
| **Git**                | Optional | Für Version Control          |

---

## Installation

### 1. Repository klonen

```
git clone <repository-url>
cd EBuEf2IVU
```

### 2. Voraussetzungen prüfen

```
# .NET SDK prüfen
dotnet --version

# Docker prüfen
docker --version
docker ps
```

### 3. Docker Registry Login (optional)

Nur erforderlich bei `-Push`:

```
docker login git.tu-berlin.de:5000
```

**Hinweis:** Falls PowerShell Execution Policy Fehler auftreten:

```
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

---

## Verwendung

### Grundlegende Befehle

```
# In das Script-Verzeichnis wechseln
cd Additionals\Scripts

# Standard-Build (Build-Increment)
.\Build.ps1

# Vollständiger Release-Build
.\Build.ps1 -VersionBump minor -Push -UpdateCompose

# Entwicklungs-Build ohne Version-Update
.\Build.ps1 -VersionBump none -Tag "dev"

# Multi-Platform Release
.\Build.ps1 -VersionBump build -MultiPlatform -Push

# Offline-TAR-Export
.\Build.ps1 -CreateTar
```

---

## Parameter

| Parameter        | Typ        | Standard                                 | Beschreibung                                                 |
| ---------------- | ---------- | ---------------------------------------- | ------------------------------------------------------------ |
| `-Services`      | `string[]` | `@("crew", "path", "vehicle")`           | Services die getestet werden sollen                          |
| `-VersionBump`   | `string`   | `"build"`                                | Version-Update: `minor`, `build`, `none`                     |
| `-Push`          | `switch`   | `false`                                  | Push zu Docker Registry                                      |
| `-CreateTar`     | `switch`   | `false`                                  | TAR-Archiv für Offline-Deployment (nicht mit MultiPlatform)  |
| `-Tag`           | `string`   | `"latest"`                               | Custom Image-Tag (wird bei VersionBump überschrieben)        |
| `-Registry`      | `string`   | `git.tu-berlin.de:5000/ebuef/ebueftools` | Docker Registry URL                                          |
| `-ProjectName`   | `string`   | `"ebuef2ivu"`                            | Projekt-Name für Image-Tagging                               |
| `-OutputDir`     | `string`   | `$env:USERPROFILE\Dropbox\Public\EBuEf`  | Ausgabeverzeichnis für TAR                                   |
| `-MultiPlatform` | `switch`   | `false`                                  | Multi-Platform Build (AMD64+ARM64, nicht mit CreateTar)      |
| `-UpdateCompose` | `switch`   | `false`                                  | Aktualisiert `docker-compose.yml`                            |

### Parameter-Details

#### `-VersionBump`

- **`minor`**: Erhöht Minor-Version, setzt Build auf 0
  - `2.0.5` → `2.1.0`
- **`build`**: Erhöht Build-Version
  - `2.0.5` → `2.0.6`
- **`none`**: Keine Version-Änderung (verwendet aktuelle Version)

#### `-MultiPlatform`

- Erstellt Images für **AMD64** (x86_64) und **ARM64** (aarch64)
- Benötigt Docker Buildx
- Erstellt automatisch Builder `ebuef2ivu-builder`
- **Limitation**: TAR-Export nicht möglich

---

## Features

### Automatisches Versioning

Das Script aktualisiert automatisch die Version in `Shareds.Host.csproj`:

```
<PropertyGroup>
  <Version>2.0.0</Version>
  <FileVersion>2.0.0</FileVersion>
  <AssemblyVersion>2.0.0</AssemblyVersion>
</PropertyGroup>
```

**Beispiel:**

```
.\Build.ps1 -VersionBump build
# → Version 2.0.0 → 2.0.1
# → Image-Tag: git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu:2.0.1
```

---

### Multi-Platform Support

Erstellt Images für verschiedene Architekturen:

```
.\Build.ps1 -MultiPlatform -Push
```

**Unterstützte Plattformen:**

- `linux/amd64` (Intel/AMD x86_64)
- `linux/arm64` (ARM64 / Apple Silicon / Raspberry Pi)

**Image-Manifest anzeigen:**

```
docker buildx imagetools inspect git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu:2.0.1
```

---

### Service-Tests

Das Script startet automatisch alle konfigurierten Services (`crew`, `path`, `vehicle`) und prüft deren Status:

```
> Teste Service: crew
✓ Service 'crew' läuft erfolgreich
  info: EBuEf2IVU.Workers.Crew[0]
  Starting EBuEf2IVU Crew Service...
```

**Services anpassen:**

```
.\Build.ps1 -Services @("crew", "path")
```

---

### Docker Compose Update

Aktualisiert automatisch alle `image:`-Zeilen in `docker-compose.yml`:

**Vorher:**

```
services:
  ebuef2ivu-crew:
    image: git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu:latest
```

**Nachher (bei Version 2.0.1):**

```
services:
  ebuef2ivu-crew:
    image: git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu:2.0.1
```

**Verwendung:**

```
.\Build.ps1 -VersionBump build -UpdateCompose
```

Erstellt automatisch `docker-compose.yml.backup` vor Änderungen.

---

### TAR-Export für Offline-Deployments

Exportiert Image als TAR-Datei für Systeme ohne Internet:

```
.\Build.ps1 -CreateTar
# Erstellt: ebuef2ivu-2.0.1.tar
```

**TAR-Datei laden:**

```
docker load --input ebuef2ivu-2.0.1.tar
```

**Hinweis:** Nicht kompatibel mit `-MultiPlatform`

---

## Workflows

### Entwicklung (Lokaler Build)

```
# Schneller Build ohne Version-Update
.\Build.ps1 -VersionBump none -Tag "dev"

# Mit Docker Compose testen
docker-compose up -d
docker-compose logs -f ebuef2ivu-crew
```

---

### Pre-Release (Build-Increment)

```
# Build-Version erhöhen (2.0.5 → 2.0.6)
.\Build.ps1 -VersionBump build -UpdateCompose

# Lokal testen
docker-compose up -d
```

---

### Release (Minor-Update + Registry)

```
# Minor-Version erhöhen (2.0.6 → 2.1.0)
.\Build.ps1 -VersionBump minor -Push -UpdateCompose

# Optional: TAR für Offline-Deployment
.\Build.ps1 -VersionBump none -CreateTar
```

---

### Production (Multi-Platform)

```
# Multi-Platform Build mit Registry-Push
.\Build.ps1 -VersionBump build -MultiPlatform -Push -UpdateCompose

# Auf Ziel-System deployen
docker-compose pull
docker-compose up -d
```

---

### Hotfix (Nur Build-Update)

```
# Schnelles Build-Increment + Push
.\Build.ps1 -VersionBump build -Push

# Docker Compose manuell aktualisieren
docker-compose pull
docker-compose up -d
```

---

## Troubleshooting

### Problem: "Docker läuft nicht"

**Lösung:**

```
# Docker Desktop starten
Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"

# Warten bis bereit
docker ps
```

---

### Problem: "Build-Fehler gefunden"

**Ursache:** Solution kompiliert nicht

**Lösung:**

```
# In Solution-Root wechseln
cd ..\..

# Manueller Build-Test
dotnet build Shareds\Host\Shareds.Host.csproj

# Clean Build
.\Additionals\Scripts\Clean.bat
dotnet restore
```

---

### Problem: "Registry-Login fehlgeschlagen"

**Lösung:**

```
# Manueller Login
docker login git.tu-berlin.de:5000

# Mit Token (CI/CD)
echo $TOKEN | docker login git.tu-berlin.de:5000 -u $USERNAME --password-stdin
```

---

### Problem: "Multi-Platform Build schlägt fehl"

**Ursache:** Buildx nicht korrekt konfiguriert

**Lösung:**

```
# Builder neu erstellen
docker buildx rm ebuef2ivu-builder
docker buildx create --name ebuef2ivu-builder --driver docker-container --use
docker buildx inspect --bootstrap
```

---

### Problem: "Service-Test schlägt fehl"

**Ursache:** Container startet nicht korrekt

**Diagnose:**

```
# Container manuell starten
docker run -it --rm -e SERVICE_NAME=crew git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu:2.0.1

# Logs prüfen
docker logs <container-id>
```

---

### Problem: "TAR-Datei zu groß"

**Ursache:** Image enthält unnötige Layer

**Lösung:**

```
# Multi-Stage Dockerfile optimieren
# Oder Image-Größe prüfen
docker images git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu

# Layer-History anzeigen
docker history git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu:2.0.1
```

---

## FAQ

### **Kann ich das Script ohne PowerShell ausführen?**

Nein, das Script benötigt PowerShell. Alternativ kann `pwsh` (PowerShell Core) verwendet werden:

```
pwsh -File Build.ps1 -VersionBump build
```

---

### **Wie setze ich die Version manuell?**

Die Version wird in `Shareds\Host\Shareds.Host.csproj` gespeichert:

```
<Version>2.5.0</Version>

```

Dann mit `-VersionBump none` bauen:

```
.\Build.ps1 -VersionBump none
```

---

### **Kann ich mehrere Tags gleichzeitig pushen?**

Ja, das Script pusht automatisch zwei Tags:

- **Versioned Tag**: `git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu:2.0.1`
- **Latest Tag**: `git.tu-berlin.de:5000/ebuef/ebueftools/ebuef2ivu:latest`

---

### **Wie deploye ich auf einem Server ohne Internet?**

1. **TAR-Export erstellen:**
   
```
   .\Build.ps1 -CreateTar -OutputDir "C:\Temp"
```

2. **TAR auf Server kopieren** (USB, Netzwerk, etc.)

3. **Auf Server laden:**
   
```
   docker load --input ebuef2ivu-2.0.1.tar
   docker-compose up -d
```

---

### **Kann ich nur bestimmte Services testen?**

Ja, über den `-Services` Parameter:

```
.\Build.ps1 -Services @("crew")
```

---

### **Wie aktualisiere ich nur docker-compose.yml?**

```
.\Build.ps1 -VersionBump none -UpdateCompose
```

Dies verwendet die aktuelle Version aus der `.csproj` Datei.

---

### **Unterstützt das Script CI/CD-Pipelines?**

Ja! Beispiel für GitLab CI:

```
build-docker:
  script:
    - pwsh -File Additionals/Scripts/Build.ps1 -VersionBump build -Push -MultiPlatform
  only:
    - main
```