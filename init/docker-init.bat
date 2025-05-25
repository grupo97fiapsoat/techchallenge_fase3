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

echo %BLUE%🐳 Inicializando ambiente Docker...%RESET%

:: Stop and remove existing containers
echo %YELLOW%🛑 Parando containers existentes...%RESET%
docker-compose down --remove-orphans 2>nul

:: Clean up old images and volumes if requested
echo %BLUE%🧹 Deseja limpar volumes e imagens antigas? (s/N): %RESET%
set /p "cleanup="
if /i "" equ "s" (
    echo %YELLOW%🗑️  Removendo volumes e imagens...%RESET%
    docker-compose down -v --remove-orphans 2>nul
    docker system prune -f 2>nul
    echo %GREEN%✅ Limpeza concluída%RESET%
)

:: Pull latest images
echo %BLUE%⬇️  Baixando imagens mais recentes...%RESET%
docker-compose pull
if 0 neq 0 (
    echo %YELLOW%⚠️  Falha no download de algumas imagens, continuando...%RESET%
)

:: Build application images
echo %BLUE%🔨 Construindo imagens da aplicação...%RESET%
docker-compose build --no-cache
if 0 neq 0 (
    echo %RED%❌ Falha na construção das imagens%RESET%
    exit /b 1
)

:: Start database first
echo %BLUE%🗄️  Iniciando banco de dados...%RESET%
docker-compose up -d db
if 0 neq 0 (
    echo %RED%❌ Falha ao iniciar o banco de dados%RESET%
    exit /b 1
)

:: Wait for database to be ready
echo %BLUE%⏳ Aguardando banco de dados ficar pronto...%RESET%
set "db_ready=false"
for /l %i in (1,1,30) do (
    docker-compose exec -T db sqlcmd -S localhost -U sa -P "FastFood2024" -Q "SELECT 1" >nul 2>&1
    if 0 equ 0 (
        set "db_ready=true"
        goto :db_ready
    )
    echo %CYAN%⏳ Tentativa %i/30 - Aguardando SQL Server...%RESET%
    timeout /t 2 >nul
)
:db_ready

if "" equ "false" (
    echo %RED%❌ Timeout: SQL Server não ficou pronto%RESET%
    exit /b 1
)

echo %GREEN%✅ Banco de dados está pronto%RESET%
echo %GREEN%✅ Docker inicializado com sucesso%RESET%

exit /b 0
