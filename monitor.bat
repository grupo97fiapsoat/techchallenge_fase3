@echo off
chcp 65001 >nul 2>&1
setlocal EnableDelayedExpansion

:: Colors
set "RED=[91m"
set "GREEN=[92m"
set "YELLOW=[93m"
set "BLUE=[94m"
set "MAGENTA=[95m"
set "CYAN=[96m"
set "WHITE=[97m"
set "RESET=[0m"
set "BOLD=[1m"

:: Header
cls
echo.
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo %BOLD%%CYAN%║                         📊 FASTFOOD API - STATUS MONITOR                      ║%RESET%
echo %BOLD%%CYAN%║                            Monitor de Status em Tempo Real                    ║%RESET%
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo.

:MONITOR_LOOP
cls
echo.
echo %BOLD%%CYAN%📊 FASTFOOD API - STATUS MONITOR%RESET%
echo %WHITE%Atualizado em: %date% %time%%RESET%
echo.

:: Check Docker status
echo %BOLD%%BLUE%🐳 STATUS DOCKER:%RESET%
docker info >nul 2>&1
if !errorlevel! equ 0 (
    echo %GREEN%   ✅ Docker está rodando%RESET%
) else (
    echo %RED%   ❌ Docker não está rodando%RESET%
)

:: Check containers
echo.
echo %BOLD%%BLUE%📦 CONTAINERS:%RESET%
docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}" 2>nul
if !errorlevel! neq 0 (
    echo %RED%   ❌ Não foi possível obter status dos containers%RESET%
)

:: Check API endpoints
echo.
echo %BOLD%%BLUE%🌐 ENDPOINTS DA API:%RESET%
curl -s -o nul -w "%%{http_code}" http://localhost:5000/health 2>nul | findstr "200" >nul
if !errorlevel! equ 0 (
    echo %GREEN%   ✅ HTTP  (5000): Respondendo%RESET%
) else (
    echo %RED%   ❌ HTTP  (5000): Não responde%RESET%
)

curl -k -s -o nul -w "%%{http_code}" https://localhost:5001/health 2>nul | findstr "200" >nul
if !errorlevel! equ 0 (
    echo %GREEN%   ✅ HTTPS (5001): Respondendo%RESET%
) else (
    echo %RED%   ❌ HTTPS (5001): Não responde%RESET%
)

:: Check database
echo.
echo %BOLD%%BLUE%🗄️  BANCO DE DADOS:%RESET%
docker-compose exec -T db sqlcmd -S localhost -U sa -P "FastFood2024" -Q "SELECT 1" >nul 2>&1
if !errorlevel! equ 0 (
    echo %GREEN%   ✅ SQL Server: Conectado%RESET%
    
    :: Get database info
    for /f "tokens=*" %%i in ('docker-compose exec -T db sqlcmd -S localhost -U sa -P "FastFood2024" -Q "SELECT COUNT(*) FROM sys.databases WHERE name='FastFoodDb'" -h -1 2^>nul') do (
        set "db_exists=%%i"
    )
    
    if "!db_exists!"=="1" (
        echo %GREEN%   ✅ Database: FastFoodDb existe%RESET%
    ) else (
        echo %YELLOW%   ⚠️  Database: FastFoodDb não encontrada%RESET%
    )
) else (
    echo %RED%   ❌ SQL Server: Não conectado%RESET%
)

:: Check certificates
echo.
echo %BOLD%%BLUE%🔐 CERTIFICADOS:%RESET%
if exist "certs\fastfood-dev.pfx" (
    echo %GREEN%   ✅ Certificado SSL: Presente%RESET%
) else (
    echo %RED%   ❌ Certificado SSL: Ausente%RESET%
)

:: Check environment
echo.
echo %BOLD%%BLUE%⚙️  AMBIENTE:%RESET%
if exist ".env" (
    echo %GREEN%   ✅ Arquivo .env: Presente%RESET%
) else (
    echo %RED%   ❌ Arquivo .env: Ausente%RESET%
)

:: Resource usage
echo.
echo %BOLD%%BLUE%💾 USO DE RECURSOS:%RESET%
for /f "tokens=2 delims= " %%i in ('docker stats --no-stream --format "table {{.CPUPerc}}" 2^>nul ^| findstr "%"') do (
    echo %CYAN%   🖥️  CPU Docker: %%i%RESET%
    goto :cpu_done
)
:cpu_done

for /f "tokens=2 delims= " %%i in ('docker stats --no-stream --format "table {{.MemUsage}}" 2^>nul ^| findstr "/"') do (
    echo %CYAN%   💾 Memory Docker: %%i%RESET%
    goto :mem_done
)
:mem_done

:: Quick actions menu
echo.
echo %BOLD%%YELLOW%🔧 AÇÕES RÁPIDAS:%RESET%
echo %WHITE%   R - Atualizar Status    L - Ver Logs    S - Parar Tudo%RESET%
echo %WHITE%   I - Iniciar API         D - Restart DB  Q - Sair%RESET%
echo.

:: Get user input with timeout
choice /c RLSIDQ /t 5 /d R /m "Escolha uma ação (auto-refresh em 5s)"
set "action=!errorlevel!"

if !action! equ 1 goto MONITOR_LOOP
if !action! equ 2 goto SHOW_LOGS
if !action! equ 3 goto STOP_ALL
if !action! equ 4 goto START_API
if !action! equ 5 goto RESTART_DB
if !action! equ 6 goto EXIT_MONITOR

goto MONITOR_LOOP

:SHOW_LOGS
echo.
echo %BOLD%%BLUE%📄 LOGS RECENTES:%RESET%
docker-compose logs --tail=20 api
echo.
pause
goto MONITOR_LOOP

:STOP_ALL
echo.
echo %BOLD%%YELLOW%🛑 Parando todos os serviços...%RESET%
docker-compose down
echo %GREEN%✅ Serviços parados%RESET%
timeout /t 3 >nul
goto MONITOR_LOOP

:START_API
echo.
echo %BOLD%%BLUE%🚀 Iniciando API...%RESET%
docker-compose up -d api
echo %GREEN%✅ API iniciada%RESET%
timeout /t 3 >nul
goto MONITOR_LOOP

:RESTART_DB
echo.
echo %BOLD%%BLUE%🔄 Reiniciando banco de dados...%RESET%
docker-compose restart db
echo %GREEN%✅ Banco reiniciado%RESET%
timeout /t 3 >nul
goto MONITOR_LOOP

:EXIT_MONITOR
echo.
echo %BOLD%%CYAN%👋 Monitor encerrado%RESET%
exit /b 0
