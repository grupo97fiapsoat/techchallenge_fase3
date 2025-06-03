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

echo %BLUE%🔎 Verificando instalação...%RESET%

:: Check if containers are running
echo %BLUE%🐳 Verificando containers...%RESET%
docker-compose ps

:: Check database connection
echo %BLUE%🗄️  Testando conexão com banco de dados...%RESET%
docker-compose exec -T db sqlcmd -S localhost -U sa -P "FastFood2024" -Q "SELECT GETDATE() as CurrentTime, @@VERSION as SQLVersion" 2>nul
if 0 neq 0 (
    echo %RED%❌ Não foi possível conectar ao banco de dados%RESET%
    exit /b 1
)
echo %GREEN%✅ Conexão com banco de dados OK%RESET%

:: Start API container if not running
echo %BLUE%🚀 Iniciando API se necessário...%RESET%
docker-compose up -d api

:: Wait for API to be ready
echo %BLUE%⏳ Aguardando API ficar pronta...%RESET%
timeout /t 10 >nul

:: Test API endpoints
echo %BLUE%🌐 Testando endpoints da API...%RESET%
curl -k -s https://localhost:5001/health >nul 2>&1
if 0 equ 0 (
    echo %GREEN%✅ API HTTPS respondendo%RESET%
) else (
    echo %YELLOW%⚠️  API HTTPS não está respondendo%RESET%
)

curl -s http://localhost:5000/health >nul 2>&1
if 0 equ 0 (
    echo %GREEN%✅ API HTTP respondendo%RESET%
) else (
    echo %YELLOW%⚠️  API HTTP não está respondendo%RESET%
)

:: Check certificates
echo %BLUE%🔐 Verificando certificados...%RESET%
if exist "certs\fastfood-dev.pfx" (
    echo %GREEN%✅ Certificado SSL encontrado%RESET%
) else (
    echo %RED%❌ Certificado SSL não encontrado%RESET%
)

:: Show final status
echo %CYAN%📊 Status Final:%RESET%
echo %WHITE%   • Containers: %RESET%
docker-compose ps --format "table {{.Name}}\t{{.Status}}"

echo %GREEN%✅ Verificação de instalação concluída%RESET%

exit /b 0
