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

echo %BLUE%🗃️  Executando migrações do banco de dados...%RESET%

:: Start migrations container
echo %BLUE%🚀 Iniciando container de migrações...%RESET%
docker-compose up migrations
if 0 neq 0 (
    echo %RED%❌ Falha nas migrações via docker-compose%RESET%
    echo %YELLOW%🔧 Tentando método alternativo...%RESET%
ECHO está desativado.
    :: Alternative: run migrations directly
    docker-compose run --rm migrations
    if 0 neq 0 (
        echo %RED%❌ Falha nas migrações alternativas%RESET%
        echo %YELLOW%💡 Verifique se o banco de dados está rodando%RESET%
        exit /b 1
    )
)

echo %GREEN%✅ Migrações executadas com sucesso%RESET%

exit /b 0
