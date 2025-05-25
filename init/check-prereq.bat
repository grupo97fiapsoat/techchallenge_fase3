@echo off
chcp 65001 >nul 2>&1
setlocal EnableDelayedExpansion

:: Colors
set "RED=[91m"
set "GREEN=[92m"
set "YELLOW=[93m"
set "BLUE=[94m"
set "CYAN=[96m"
set "WHITE=[97m"
set "RESET=[0m"

echo %BLUE%🔍 Verificando Docker...%RESET%
docker --version >nul 2>&1
if 0 neq 0 (
    echo %RED%❌ Docker não está instalado ou não está no PATH%RESET%
    echo %YELLOW%💡 Instale o Docker Desktop: https://www.docker.com/products/docker-desktop%RESET%
    exit /b 1
)
echo %GREEN%✅ Docker encontrado%RESET%

echo %BLUE%🔍 Verificando Docker Compose...%RESET%
docker-compose --version >nul 2>&1
if 0 neq 0 (
    echo %RED%❌ Docker Compose não está disponível%RESET%
    echo %YELLOW%💡 Instale o Docker Compose ou use Docker Desktop%RESET%
    exit /b 1
)
echo %GREEN%✅ Docker Compose encontrado%RESET%

echo %BLUE%🔍 Verificando se o Docker está rodando...%RESET%
docker info >nul 2>&1
if 0 neq 0 (
    echo %RED%❌ Docker não está rodando%RESET%
    echo %YELLOW%💡 Inicie o Docker Desktop ou serviço Docker%RESET%
    exit /b 1
)
echo %GREEN%✅ Docker está rodando%RESET%

echo %BLUE%🔍 Verificando PowerShell...%RESET%
powershell -Command "Get-Host" >nul 2>&1
if 0 neq 0 (
    echo %RED%❌ PowerShell não está disponível%RESET%
    exit /b 1
)
echo %GREEN%✅ PowerShell encontrado%RESET%

echo %BLUE%🔍 Verificando porta 1433 (SQL Server)...%RESET%
netstat -an | findstr ":1433" >nul 2>&1
if 0 equ 0 (
    echo %YELLOW%⚠️  Porta 1433 já está em uso%RESET%
    echo %WHITE%   Isso pode causar conflitos com o SQL Server%RESET%
)

echo %BLUE%🔍 Verificando portas 5000/5001 (API)...%RESET%
netstat -an | findstr ":5000" >nul 2>&1
if 0 equ 0 (
    echo %YELLOW%⚠️  Porta 5000 já está em uso%RESET%
)
netstat -an | findstr ":5001" >nul 2>&1
if 0 equ 0 (
    echo %YELLOW%⚠️  Porta 5001 já está em uso%RESET%
)

echo %GREEN%✅ Verificação de pré-requisitos concluída%RESET%
exit /b 0
