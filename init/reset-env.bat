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

echo %RED%🔄 RESET COMPLETO DO AMBIENTE%RESET%
echo %YELLOW%⚠️  Esta ação irá remover TODOS os containers, volumes e dados%RESET%
echo %WHITE%   Tem certeza que deseja continuar? (s/N): %RESET%
set /p "confirm="

if /i "" neq "s" (
    echo %CYAN%❌ Reset cancelado pelo usuário%RESET%
    exit /b 0
)

echo %BLUE%🛑 Parando todos os containers...%RESET%
docker-compose down --remove-orphans

echo %BLUE%🗑️  Removendo volumes...%RESET%
docker-compose down -v --remove-orphans

echo %BLUE%🧹 Limpando sistema Docker...%RESET%
docker system prune -f

echo %BLUE%🗑️  Removendo imagens FastFood...%RESET%
for /f "tokens=3" %i in ('docker images --format "table {{.Repository}}\t{{.Tag}}\t{{.ID}}" | findstr fastfood') do (
    docker rmi %i -f 2>nul
)

echo %BLUE%📁 Limpando arquivos locais...%RESET%
if exist "certs\" rmdir /s /q "certs"
if exist ".env" del ".env"
if exist "logs\" rmdir /s /q "logs"

echo %GREEN%✅ Reset completo concluído%RESET%
echo %YELLOW%💡 Execute o inicializador novamente para reinstalar%RESET%

exit /b 0
