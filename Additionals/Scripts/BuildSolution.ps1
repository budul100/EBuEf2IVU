<#
.SYNOPSIS
    Baut und pusht EBuEf2IVU Docker Images für Multi-Service-Deployment
.DESCRIPTION
    Automatisiertes Build-Script für Docker Images mit integriertem Versioning,
    Multi-Platform-Support (AMD64/ARM64), Service-Tests und optionalem Registry-Push.
.PARAMETER Services
    Zu testende Services (Standard: crew, path, vehicle)
.PARAMETER VersionBump
    Art des Version-Updates: minor, build oder none (Standard: build)
.PARAMETER Push
    Push zu Docker Registry durchführen
.PARAMETER CreateTar
    Erstelle tar-Archiv für Offline-Deployment
.PARAMETER Tag
    Custom Tag (wird bei VersionBump überschrieben, Standard: latest)
.PARAMETER Registry
    Docker Registry URL
.PARAMETER ProjectName
    Projekt-Name für Image-Tagging
.PARAMETER OutputDir
    Ausgabeverzeichnis für tar-Archive
.PARAMETER MultiPlatform
    Aktiviert Multi-Platform-Build (linux/amd64, linux/arm64)
.PARAMETER UpdateCompose
    Aktualisiert docker-compose.yml mit neuer Image-Version
.EXAMPLE
    .\Build-DockerImages.ps1
    Standard-Build mit automatischem Build-Version-Increment NACH erfolgreichem Build
#>

[CmdletBinding()]
param(
    [string[]]$Services = @("crew", "path", "vehicle"),
    [ValidateSet("minor", "build", "none")]
    [string]$VersionBump = "build",
    [switch]$Push,
    [switch]$CreateTar,
    [string]$Tag = "latest",
    [string]$Registry = "git.tu-berlin.de:5000/ebuef/ebueftools",
    [string]$ProjectName = "ebuef2ivu",
    [string]$OutputDir = "$env:USERPROFILE\Dropbox\Public\EBuEf",
    [switch]$MultiPlatform,
    [switch]$UpdateCompose
)

$ErrorActionPreference = "Stop"
$InformationPreference = "Continue"

# ============================================================================
# Helper Functions
# ============================================================================

function Write-Step { 
    param([string]$Message) 
    Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host "  $Message" -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━`n" -ForegroundColor Cyan
}

function Write-Success { 
    param([string]$Message) 
    Write-Host "✓ $Message" -ForegroundColor Green 
}

function Write-Info { 
    param([string]$Message) 
    Write-Host "ℹ $Message" -ForegroundColor Blue 
}

function Write-Warn { 
    param([string]$Message) 
    Write-Host "⚠ $Message" -ForegroundColor Yellow 
}

function Write-Fail { 
    param([string]$Message) 
    Write-Host "✗ $Message" -ForegroundColor Red 
    exit 1
}

function Test-CommandExists {
    param([string]$Command)
    return $null -ne (Get-Command $Command -ErrorAction SilentlyContinue)
}

function Get-ProjectVersion {
    param([string]$ProjectPath)
    
    if (-not (Test-Path $ProjectPath)) {
        Write-Warn "Projekt-Datei nicht gefunden: $ProjectPath"
        return $null
    }
    
    try {
        [xml]$csproj = Get-Content $ProjectPath
        $version = $csproj.Project.PropertyGroup.Version
        
        if ([string]::IsNullOrWhiteSpace($version)) {
            Write-Warn "Version-Tag in .csproj nicht gefunden"
            return $null
        }
        
        return $version
    }
    catch {
        Write-Warn "Fehler beim Lesen der Version: $_"
        return $null
    }
}

function Update-ProjectVersion {
    param(
        [string]$ProjectPath,
        [ValidateSet("minor", "build")]
        [string]$BumpType
    )
    
    if (-not (Test-Path $ProjectPath)) {
        Write-Fail "Projekt-Datei nicht gefunden: $ProjectPath"
    }
    
    try {
        [xml]$csproj = Get-Content $ProjectPath
        $currentVersion = $csproj.Project.PropertyGroup.Version
        
        if ([string]::IsNullOrWhiteSpace($currentVersion)) {
            Write-Fail "Keine Version in .csproj gefunden"
        }
        
        $versionParts = $currentVersion.Split('.')
        
        if ($BumpType -eq "minor") {
            # Increment Minor, reset Build
            $major = [int]$versionParts[0]
            $minor = [int]$versionParts[1] + 1
            $build = 0
            $newVersion = "$major.$minor.$build"
        }
        else {
            # Increment Build
            $major = [int]$versionParts[0]
            $minor = [int]$versionParts[1]
            $build = [int]$versionParts[2] + 1
            $newVersion = "$major.$minor.$build"
        }
        
        # Update Version, FileVersion und AssemblyVersion
        $csproj.Project.PropertyGroup.Version = $newVersion
        # $csproj.Project.PropertyGroup.FileVersion = $newVersion
        # $csproj.Project.PropertyGroup.AssemblyVersion = $newVersion
        
        $csproj.Save($ProjectPath)
        
        return $newVersion
    }
    catch {
        Write-Fail "Fehler beim Version-Update: $_"
    }
}

function Update-DockerCompose {
    param(
        [string]$ComposePath,
        [string]$ImageName,
        [string]$Tag
    )
    
    if (-not (Test-Path $ComposePath)) {
        Write-Warn "docker-compose.yml nicht gefunden: $ComposePath"
        return
    }
    
    try {
        Write-Info "Aktualisiere docker-compose.yml mit Image-Tag: $Tag"
        
        $content = Get-Content $ComposePath -Raw
        
        # Regex für image-Zeile (mit oder ohne Tag)
        $pattern = '(\s+image:\s+)([^\s:]+)(:[^\s]+)?'
        $replacement = "`${1}$ImageName`:$Tag"
        
        # Aktualisiere alle image-Zeilen
        $updatedContent = $content -replace $pattern, $replacement
        
        # Speichere Backup
        $backupPath = "$ComposePath.backup"
        Copy-Item $ComposePath $backupPath -Force
        Write-Info "Backup erstellt: $backupPath"
        
        # Schreibe aktualisierte Datei
        $updatedContent | Set-Content $ComposePath -NoNewline
        
        Write-Success "docker-compose.yml aktualisiert mit Tag: $Tag"
        
        # Zeige Änderungen
        Write-Host "`nAktualisierte Image-Zeilen:" -ForegroundColor Yellow
        $updatedContent -split "`n" | Where-Object { $_ -match '^\s+image:' } | ForEach-Object {
            Write-Host "  $_" -ForegroundColor White
        }
    }
    catch {
        Write-Warn "Fehler beim Aktualisieren von docker-compose.yml: $_"
    }
}

function Initialize-DockerBuildx {
    Write-Info "Prüfe Docker Buildx..."
    
    # Prüfe ob buildx verfügbar ist
    $buildxVersion = docker buildx version 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-Fail "Docker Buildx nicht verfügbar! Bitte Docker Desktop aktualisieren."
    }
    
    Write-Success "Docker Buildx verfügbar"
    
    # Prüfe/Erstelle Multi-Platform Builder
    $builderName = "ebuef2ivu-builder"
    $existingBuilder = docker buildx ls | Select-String $builderName
    
    if (-not $existingBuilder) {
        Write-Info "Erstelle Multi-Platform Builder: $builderName"
        docker buildx create --name $builderName --driver docker-container --use
        
        if ($LASTEXITCODE -ne 0) {
            Write-Fail "Fehler beim Erstellen des Builders"
        }
        
        Write-Success "Builder erstellt: $builderName"
    }
    else {
        Write-Info "Verwende existierenden Builder: $builderName"
        docker buildx use $builderName
    }
    
    # Bootstrap Builder (lädt Images)
    Write-Info "Initialisiere Builder..."
    docker buildx inspect --bootstrap | Out-Null
    
    if ($LASTEXITCODE -ne 0) {
        Write-Warn "Builder-Bootstrap fehlgeschlagen, wird beim Build initialisiert"
    }
}

# ============================================================================
# Main Script
# ============================================================================

# Header
Clear-Host
Write-Host @"

╔═══════════════════════════════════════════════════╗
║                                                   ║
║           EBuEf2IVU Docker Build System           ║
║               Multi-Service Container             ║
║                                                   ║
╚═══════════════════════════════════════════════════╝

"@ -ForegroundColor Cyan

Write-Info "Projekt: $ProjectName"
Write-Info "Registry: $Registry"
Write-Info "Services: $($Services -join ', ')"
if ($MultiPlatform) {
    Write-Info "Plattformen: linux/amd64, linux/arm64"
}
Write-Host ""

# Pfade ermitteln
$scriptRoot = $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($scriptRoot)) {
    $scriptRoot = Get-Location
}

$solutionRoot = Split-Path (Split-Path $scriptRoot -Parent) -Parent
$hostProjectPath = Join-Path $solutionRoot "Shareds\Host\Host.csproj"
$dockerfilePath = Join-Path $solutionRoot "Shareds\Host\Dockerfile"
$dockerComposePath = Join-Path $solutionRoot "docker-compose.yml"

Write-Info "Solution Root: $solutionRoot"
Write-Info "Host Project: $hostProjectPath"

# ============================================================================
# Voraussetzungen prüfen
# ============================================================================

Write-Step "Prüfe Voraussetzungen"

# 1. Host-Projekt vorhanden?
if (-not (Test-Path $hostProjectPath)) {
    Write-Fail "Host-Projekt nicht gefunden: $hostProjectPath"
}
Write-Success "Host-Projekt gefunden"

# 2. Dockerfile vorhanden?
if (-not (Test-Path $dockerfilePath)) {
    Write-Fail "Dockerfile nicht gefunden: $dockerfilePath"
}
Write-Success "Dockerfile gefunden"

# 3. docker-compose.yml vorhanden?
if (Test-Path $dockerComposePath) {
    Write-Success "docker-compose.yml gefunden"
}
else {
    Write-Warn "docker-compose.yml nicht gefunden (Optional)"
}

# 4. .NET 8 SDK installiert?
if (Test-CommandExists "dotnet") {
    $dotnetOutput = dotnet --version
    $currentVersion = [System.Version]$dotnetOutput

    if ($currentVersion.Major -ge 8) {
        Write-Success "dotnet SDK ist ausreichend aktuell: $dotnetOutput"
    }
    else {
        Write-Warn "dotnet SDK zu alt ($dotnetOutput). Benötigt wird mindestens Version 8."
    }
}
else {
    Write-Fail ".NET SDK nicht gefunden! Bitte aktuelles .NET SDK installieren."
}

# 5. Docker vorhanden und läuft?
if (-not (Test-CommandExists "docker")) {
    Write-Fail "Docker nicht gefunden! Bitte Docker Desktop installieren."
}

try {
    $null = docker ps 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Fail "Docker läuft nicht! Bitte Docker Desktop starten."
    }
    Write-Success "Docker ist verfügbar und läuft"
}
catch {
    Write-Fail "Docker-Verbindung fehlgeschlagen: $_"
}

# 6. Multi-Platform: Buildx initialisieren
if ($MultiPlatform) {
    Initialize-DockerBuildx
}

# 7. Solution buildbar?
Write-Info "Prüfe ob Solution kompiliert..."
Push-Location $solutionRoot
try {
    $buildOutput = dotnet build "Shareds\Host\Host.csproj" --configuration Release --no-incremental -v quiet 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Solution kompiliert erfolgreich"
    }
    else {
        Write-Fail "Build-Fehler gefunden! Bitte erst Solution-Fehler beheben."
    }
}
finally {
    Pop-Location
}

# ============================================================================
# Aktuelle Version ermitteln (für Docker-Build-Tag)
# ============================================================================

# Lies aktuelle Version für Build-Tag
$currentVersion = Get-ProjectVersion -ProjectPath $hostProjectPath

if ($null -ne $currentVersion) {
    Write-Info "Aktuelle Version: $currentVersion"
    
    if ($Tag -eq "latest") {
        $Tag = $currentVersion
        Write-Info "Tag für Build gesetzt auf aktuelle Version: $Tag"
    }
}
else {
    if ($Tag -eq "latest") {
        Write-Warn "Keine Version in .csproj gefunden, verwende 'latest' Tag"
    }
}

# ============================================================================
# Docker Build
# ============================================================================

Write-Step "Baue Multi-Service Docker Image"

$imageName = "$Registry/$ProjectName"
$fullImageName = "${imageName}:${Tag}"
$latestImageName = "${imageName}:latest"

Write-Info "Image-Name: $fullImageName"
Write-Info "Latest-Tag: $latestImageName"

Push-Location $solutionRoot
try {
    if ($MultiPlatform) {
        # Multi-Platform Build mit Buildx
        Write-Info "Multi-Platform Build wird gestartet..."
        
        $buildArgs = @(
            "buildx", "build",
            "--platform", "linux/amd64,linux/arm64",
            "-t", $fullImageName,
            "-t", $latestImageName,
            "-f", $dockerfilePath,
            "--build-arg", "BUILD_CONFIGURATION=Release"
        )
        
        # Bei Push direkt hochladen, sonst nur lokal cachen
        if ($Push) {
            $buildArgs += "--push"
            Write-Info "Images werden direkt zur Registry gepusht (Multi-Platform)"
        }
        else {
            $buildArgs += "--load"
            Write-Warn "Multi-Platform-Images werden nur für linux/amd64 geladen (--load Limitation)"
        }
        
        $buildArgs += $solutionRoot
        
        Write-Host "> docker $($buildArgs -join ' ')" -ForegroundColor DarkGray
        docker @buildArgs
        
        if ($LASTEXITCODE -ne 0) {
            Write-Fail "Docker Buildx Build fehlgeschlagen!"
        }
        
        Write-Success "Multi-Platform Images erfolgreich gebaut"
        Write-Info "Unterstützte Plattformen: linux/amd64, linux/arm64"
    }
    else {
        # Standard Single-Platform Build
        $buildArgs = @(
            "build",
            "-t", $fullImageName,
            "-t", $latestImageName,
            "-f", $dockerfilePath,
            "--build-arg", "BUILD_CONFIGURATION=Release",
            $solutionRoot
        )
        
        Write-Host "> docker $($buildArgs -join ' ')" -ForegroundColor DarkGray
        docker @buildArgs
        
        if ($LASTEXITCODE -ne 0) {
            Write-Fail "Docker Build fehlgeschlagen!"
        }
        
        Write-Success "Image erfolgreich gebaut"
    }
}
finally {
    Pop-Location
}

# Image-Informationen anzeigen (nur bei Single-Platform)
if (-not $MultiPlatform -or -not $Push) {
    $imageSize = docker images $imageName --format "{{.Size}}" | Select-Object -First 1
    $imageId = docker images $fullImageName --format "{{.ID}}" | Select-Object -First 1
    
    if ($imageSize) {
        Write-Info "Image-Größe: $imageSize"
    }
    if ($imageId) {
        Write-Info "Image-ID: $imageId"
    }
}

# ============================================================================
# Service Tests (nur bei nicht-MultiPlatform oder ohne Push)
# ============================================================================

if (-not $MultiPlatform -or -not $Push) {
    Write-Step "Teste Services im Image"
    
    $allServicesOk = $true
    
    foreach ($service in $Services) {
        Write-Host "`n> Teste Service: $service" -ForegroundColor Yellow
        
        try {
            # Starte Container mit explizitem Array um Backtick-Probleme zu vermeiden
            $runArgs = @(
                "run", "-d",
                "--name", "test_$service",
                "-e", "EBUEF2IVU_SERVICE=$service",
                "-e", "EBUEF2IVU_SETTINGS=/app/Settings/settings.$service.docker.xml",
                $fullImageName
            )

            $containerId = docker @runArgs
            
            if ($LASTEXITCODE -ne 0) {
                Write-Warn "Service '$service' konnte nicht gestartet werden"
                $allServicesOk = $false
                continue
            }
            
            # Warte 5 Sekunden
            Start-Sleep -Seconds 5
            
            # Prüfe Container-Status
            $containerStatus = docker inspect $containerId --format "{{.State.Status}}" 2>&1
            
            if ($containerStatus -eq "running") {
                Write-Success "Service '$service' läuft erfolgreich"
                
                # Zeige erste Log-Zeilen
                $logs = docker logs $containerId 2>&1 | Select-Object -First 3
                $logs | ForEach-Object { Write-Host "  $_" -ForegroundColor DarkGray }
            }
            else {
                Write-Warn "Service '$service' Status: $containerStatus"
                $allServicesOk = $false
                
                # Zeige Fehler-Logs
                $errorLogs = docker logs $containerId 2>&1 | Select-Object -Last 10
                Write-Host "  Fehler-Logs:" -ForegroundColor Red
                $errorLogs | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
            }
            
            # Cleanup
            docker rm -f $containerId 2>&1 | Out-Null
        }
        catch {
            Write-Warn "Service '$service' Test fehlgeschlagen: $_"
            $allServicesOk = $false
        }
    }
    
    if ($allServicesOk) {
        Write-Success "`nAlle Service-Tests erfolgreich ✓"
    }
    else {
        Write-Warn "`nEinige Service-Tests sind fehlgeschlagen!"
    }
}
else {
    Write-Info "Service-Tests übersprungen (Multi-Platform-Images in Registry)"
}

# ============================================================================
# Docker Push (nur wenn nicht bereits bei buildx gepusht)
# ============================================================================

if ($Push -and -not $MultiPlatform) {
    Write-Step "Pushe Image zu Registry"
    
    # Extrahiere Registry-Host (ohne Repository-Pfad)
    $registryHost = ($Registry -split '/')[0]
    Write-Info "Registry-Host: $registryHost"
    
    # Versuche direkt zu pushen - Docker verwendet gecachte Credentials
    Write-Info "Verwende gespeicherte Registry-Credentials"
    
    # Push versioned tag
    Write-Host "> docker push $fullImageName" -ForegroundColor DarkGray
    docker push $fullImageName
    
    if ($LASTEXITCODE -ne 0) {
        # Push fehlgeschlagen - vermutlich Login-Problem
        Write-Warn "Push fehlgeschlagen - versuche Login..."
        Write-Host "> docker login $registryHost" -ForegroundColor DarkGray
        
        # Interaktiver Login
        docker login $registryHost
        
        if ($LASTEXITCODE -ne 0) {
            Write-Fail "Registry-Login fehlgeschlagen! Bitte Credentials prüfen."
        }
        
        Write-Success "Login erfolgreich - versuche Push erneut..."
        
        # Wiederhole Push
        docker push $fullImageName
        
        if ($LASTEXITCODE -ne 0) {
            Write-Fail "Push von $fullImageName fehlgeschlagen!"
        }
    }
    
    Write-Success "Gepusht: $fullImageName"
    
    # Push latest tag (wenn unterschiedlich)
    if ($Tag -ne "latest") {
        Write-Host "> docker push $latestImageName" -ForegroundColor DarkGray
        docker push $latestImageName
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "Gepusht: $latestImageName"
        }
        else {
            Write-Warn "Push von latest-Tag fehlgeschlagen (nicht kritisch)"
        }
    }
}
elseif ($Push -and $MultiPlatform) {
    Write-Success "Multi-Platform Images bereits zur Registry gepusht"
}

# ============================================================================
# TAR-Archiv erstellen (nur Single-Platform)
# ============================================================================

if ($CreateTar) {
    if ($MultiPlatform) {
        Write-Warn "TAR-Export nicht möglich bei Multi-Platform-Builds"
        Write-Info "Tipp: Verwende ohne -MultiPlatform für TAR-Export"
    }
    else {
        Write-Step "Erstelle tar-Archiv für Offline-Deployment"
        
        $tarFileName = "$ProjectName-$Tag.tar"
        $tarFilePath = Join-Path $solutionRoot $tarFileName
        
        Write-Info "Erstelle: $tarFileName"
        Write-Host "> docker save --output `"$tarFilePath`" $fullImageName" -ForegroundColor DarkGray
        
        docker save --output $tarFilePath $fullImageName
        
        if ($LASTEXITCODE -ne 0) {
            Write-Fail "tar-Erstellung fehlgeschlagen!"
        }
        
        if (Test-Path $tarFilePath) {
            $tarSize = (Get-Item $tarFilePath).Length / 1MB
            $tarSizeRounded = [math]::Round($tarSize, 2)
            # Safe String Formatierung
            $msg = "tar-Archiv erstellt: $tarFileName ($tarSizeRounded MB)"
            Write-Success $msg
            
            # Kopiere zu Dropbox (optional)
            if (Test-Path $OutputDir) {
                $targetPath = Join-Path $OutputDir $tarFileName
                Write-Info "Kopiere zu: $OutputDir"
                Copy-Item $tarFilePath $targetPath -Force
                Write-Success "Kopiert nach: $targetPath"
            }
            else {
                Write-Warn "Dropbox-Verzeichnis nicht gefunden: $OutputDir"
                Write-Info "tar-Datei verbleibt in: $tarFilePath"
            }
        }
        else {
            Write-Fail "tar-Datei wurde nicht erstellt!"
        }
    }
}

# ============================================================================
# Docker-Compose Update
# ============================================================================

if ($UpdateCompose -and (Test-Path $dockerComposePath)) {
    Write-Step "Aktualisiere docker-compose.yml"
    
    Update-DockerCompose -ComposePath $dockerComposePath -ImageName $ProjectName -Tag $Tag
}

# ============================================================================
# Version Bump (GANZ AM ENDE - nach erfolgreichem Push!)
# ============================================================================

if ($VersionBump -ne "none") {
    Write-Step "Version aktualisieren ($VersionBump) - NACH allen Operationen"
    
    $newVersion = Update-ProjectVersion -ProjectPath $hostProjectPath -BumpType $VersionBump
    
    Write-Success "Version aktualisiert auf: $newVersion"
    Write-Info "Aktualisiert: Version, FileVersion, AssemblyVersion"
    Write-Info "Diese Version wird beim NÄCHSTEN Build verwendet"
}

# ============================================================================
# Zusammenfassung
# ============================================================================

Write-Step "Build erfolgreich abgeschlossen! ✨"

Write-Host @"

╔═══════════════════════════════════════════════════╗
║             Deployment-Befehle                    ║
╚═══════════════════════════════════════════════════╝

🔹 Einzelne Services starten:
"@ -ForegroundColor Cyan

foreach ($service in $Services) {
    Write-Host "  docker run -d --name ebuef2ivu-$service -e EBUEF2IVU_SERVICE=$service $fullImageName" -ForegroundColor White
}

Write-Host @"

🔹 Mit Docker Compose:
  docker-compose up -d

🔹 Einzelnen Service mit Compose:
  docker-compose up -d ebuef2ivu-crew

🔹 Image laden (aus tar):
  docker load --input $ProjectName-$Tag.tar

🔹 Image-Info anzeigen:
  docker inspect $fullImageName

🔹 Multi-Platform Manifest anzeigen:
  docker buildx imagetools inspect $fullImageName

🔹 Logs anzeigen:
  docker logs -f ebuef2ivu-crew

🔹 Container stoppen:
  docker-compose down

"@ -ForegroundColor Cyan

# Finale Statistik
Write-Host "╔═══════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                    Statistik                      ║" -ForegroundColor Green
Write-Host "╚═══════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host "  • Image: $fullImageName" -ForegroundColor White

if (-not $MultiPlatform -or -not $Push) {
    if ($imageSize) {
        Write-Host "  • Größe: $imageSize" -ForegroundColor White
    }
}

Write-Host "  • Services: $($Services.Count) getestet" -ForegroundColor White

if ($MultiPlatform) {
    Write-Host "  • Plattformen: linux/amd64, linux/arm64 ✓" -ForegroundColor Green
}

if ($Push) {
    Write-Host "  • Registry: Erfolgreich gepusht ✓" -ForegroundColor Green
}

if ($CreateTar -and -not $MultiPlatform) {
    Write-Host "  • TAR-Export: Erfolgreich erstellt ✓" -ForegroundColor Green
}

if ($UpdateCompose) {
    Write-Host "  • docker-compose.yml: Aktualisiert ✓" -ForegroundColor Green
}

if ($VersionBump -ne "none") {
    Write-Host "  • Version: Auf $newVersion erhöht (für nächsten Build) ✓" -ForegroundColor Green
}

Write-Host ""

Write-Success "Build-Prozess abgeschlossen! 🎉"