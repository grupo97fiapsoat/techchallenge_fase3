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

:: Auto-create utility scripts if they don't exist
if not exist "init\create-scripts.bat" (
    echo %YELLOW%🔧 Criando scripts de utilitários...%RESET%
    if not exist "init" mkdir "init"
    call "%~dp0init\create-scripts.bat" 2>nul
)

:: Generate individual scripts
if not exist "init\check-prereq.bat" (
    echo %BLUE%📦 Gerando scripts auxiliares...%RESET%
    cd /d "%~dp0init"
    call create-scripts.bat
    cd /d "%~dp0"
)

cls
echo.
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo %BOLD%%CYAN%║                    🚀 FASTFOOD API - QUICK LAUNCHER 🚀                        ║%RESET%
echo %BOLD%%CYAN%║                          Lançador Rápido v2.0                                ║%RESET%
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo.

:MAIN_MENU
echo %BOLD%%YELLOW%🎯 MENU PRINCIPAL:%RESET%
echo.
echo %WHITE%   🚀 QUICK ACTIONS:%RESET%
echo %GREEN%   1️⃣  Instalação Express (5 min)%RESET%
echo %GREEN%   2️⃣  Start API (se já instalado)%RESET%
echo %GREEN%   3️⃣  Monitor em Tempo Real%RESET%
echo.
echo %WHITE%   🔧 SETUP AVANÇADO:%RESET%
echo %CYAN%   4️⃣  Instalação Completa (init.bat)%RESET%
echo %CYAN%   5️⃣  Ajuda e Documentação%RESET%
echo %CYAN%   6️⃣  Verificar Sistema%RESET%
echo.
echo %WHITE%   🛠️  UTILITIES:%RESET%
echo %YELLOW%   7️⃣  Logs e Debug%RESET%
echo %YELLOW%   8️⃣  Reset/Cleanup%RESET%
echo %YELLOW%   9️⃣  Status dos Serviços%RESET%
echo.
echo %WHITE%   0️⃣  Sair%RESET%
echo.
set /p "choice=%CYAN%🎯 Escolha uma opção: %RESET%"

if "%choice%"=="1" goto EXPRESS_INSTALL
if "%choice%"=="2" goto QUICK_START
if "%choice%"=="3" goto REAL_TIME_MONITOR
if "%choice%"=="4" goto FULL_INSTALLER
if "%choice%"=="5" goto HELP_DOCS
if "%choice%"=="6" goto SYSTEM_CHECK
if "%choice%"=="7" goto LOGS_DEBUG
if "%choice%"=="8" goto RESET_CLEANUP
if "%choice%"=="9" goto SERVICE_STATUS
if "%choice%"=="0" goto EXIT
goto MAIN_MENU

:EXPRESS_INSTALL
echo.
echo %BOLD%%GREEN%🚀 INSTALAÇÃO EXPRESS INICIADA...%RESET%
echo %WHITE%   Esta opção fará a instalação completa automaticamente%RESET%
echo %WHITE%   Tempo estimado: 5-10 minutos%RESET%
echo.
pause

:: Run express installation
call init.bat
if !errorlevel! equ 0 (
    echo.
    echo %BOLD%%GREEN%✅ INSTALAÇÃO EXPRESS CONCLUÍDA!%RESET%
    echo.
    call :ShowSuccessMessage
) else (
    echo.
    echo %BOLD%%RED%❌ Falha na instalação express%RESET%
    echo %WHITE%💡 Tente a instalação completa (opção 4)%RESET%
)
pause
goto MAIN_MENU

:QUICK_START
echo.
echo %BOLD%%BLUE%🚀 INICIANDO API RAPIDAMENTE...%RESET%
echo.

:: Check if already running
docker-compose ps | findstr "Up" >nul 2>&1
if !errorlevel! equ 0 (
    echo %GREEN%✅ Serviços já estão rodando!%RESET%
    goto :show_quick_access
)

:: Quick start
echo %BLUE%🐳 Iniciando containers...%RESET%
docker-compose up -d

:: Wait and verify
echo %BLUE%⏳ Aguardando API ficar pronta...%RESET%
timeout /t 15 >nul

:show_quick_access
echo.
echo %BOLD%%GREEN%🎉 API ESTÁ PRONTA!%RESET%
echo.
echo %CYAN%📍 Acesso Rápido:%RESET%
echo %WHITE%   • Swagger: https://localhost:5001/swagger%RESET%
echo %WHITE%   • API HTTP: http://localhost:5000%RESET%
echo %WHITE%   • Health: http://localhost:5000/health%RESET%
echo.
pause
goto MAIN_MENU

:REAL_TIME_MONITOR
echo.
echo %BOLD%%BLUE%📊 Iniciando Monitor em Tempo Real...%RESET%
echo.
call monitor.bat
goto MAIN_MENU

:FULL_INSTALLER
echo.
echo %BOLD%%BLUE%🔧 Iniciando Instalador Completo...%RESET%
echo.
call init.bat
pause
goto MAIN_MENU

:HELP_DOCS
echo.
echo %BOLD%%BLUE%📚 Abrindo Documentação...%RESET%
echo.
call help.bat
goto MAIN_MENU

:SYSTEM_CHECK
echo.
echo %BOLD%%BLUE%🔍 VERIFICAÇÃO DO SISTEMA%RESET%
echo.

echo %BLUE%🔍 Docker Status:%RESET%
docker --version
if !errorlevel! neq 0 (
    echo %RED%❌ Docker não encontrado%RESET%
) else (
    echo %GREEN%✅ Docker OK%RESET%
)

echo.
echo %BLUE%🔍 Containers Status:%RESET%
docker-compose ps 2>nul
if !errorlevel! neq 0 (
    echo %YELLOW%⚠️  Nenhum container encontrado%RESET%
)

echo.
echo %BLUE%🔍 Portas em Uso:%RESET%
netstat -an | findstr ":5000" >nul && echo %YELLOW%⚠️  Porta 5000 em uso%RESET% || echo %GREEN%✅ Porta 5000 livre%RESET%
netstat -an | findstr ":5001" >nul && echo %YELLOW%⚠️  Porta 5001 em uso%RESET% || echo %GREEN%✅ Porta 5001 livre%RESET%
netstat -an | findstr ":1433" >nul && echo %YELLOW%⚠️  Porta 1433 em uso%RESET% || echo %GREEN%✅ Porta 1433 livre%RESET%

echo.
echo %BLUE%🔍 Arquivos Importantes:%RESET%
if exist ".env" (echo %GREEN%✅ .env encontrado%RESET%) else (echo %RED%❌ .env não encontrado%RESET%)
if exist "docker-compose.yml" (echo %GREEN%✅ docker-compose.yml encontrado%RESET%) else (echo %RED%❌ docker-compose.yml não encontrado%RESET%)
if exist "certs\fastfood-dev.pfx" (echo %GREEN%✅ Certificado SSL encontrado%RESET%) else (echo %YELLOW%⚠️  Certificado SSL não encontrado%RESET%)

pause
goto MAIN_MENU

:LOGS_DEBUG
echo.
echo %BOLD%%BLUE%📄 LOGS E DEBUG%RESET%
echo.
echo %WHITE%   1️⃣  Logs da API%RESET%
echo %WHITE%   2️⃣  Logs do Banco%RESET%
echo %WHITE%   3️⃣  Logs de Migração%RESET%
echo %WHITE%   4️⃣  Logs do Sistema%RESET%
echo %WHITE%   5️⃣  Status Detalhado%RESET%
echo %WHITE%   0️⃣  Voltar%RESET%
echo.
set /p "log_choice=%CYAN%Escolha: %RESET%"

if "%log_choice%"=="1" (
    echo %BLUE%📄 Logs da API:%RESET%
    docker-compose logs --tail=50 api
)
if "%log_choice%"=="2" (
    echo %BLUE%📄 Logs do Banco:%RESET%
    docker-compose logs --tail=50 db
)
if "%log_choice%"=="3" (
    echo %BLUE%📄 Logs de Migração:%RESET%
    docker-compose logs migrations
)
if "%log_choice%"=="4" (
    echo %BLUE%📄 Logs do Sistema:%RESET%
    if exist "logs" (
        dir logs\*.log
        echo.
        set /p "log_file=%CYAN%Digite o nome do arquivo de log: %RESET%"
        if exist "logs\!log_file!" type "logs\!log_file!"
    ) else (
        echo %YELLOW%⚠️  Pasta de logs não encontrada%RESET%
    )
)
if "%log_choice%"=="5" (
    echo %BLUE%📊 Status Detalhado:%RESET%
    docker-compose ps
    echo.
    docker stats --no-stream
)
if "%log_choice%"=="0" goto MAIN_MENU

pause
goto MAIN_MENU

:RESET_CLEANUP
echo.
echo %BOLD%%RED%🛠️  RESET E CLEANUP%RESET%
echo.
echo %WHITE%   1️⃣  Reset Suave (restart containers)%RESET%
echo %WHITE%   2️⃣  Reset Médio (rebuild images)%RESET%
echo %WHITE%   3️⃣  Reset Completo (limpar tudo)%RESET%
echo %WHITE%   4️⃣  Limpar Logs Antigos%RESET%
echo %WHITE%   0️⃣  Voltar%RESET%
echo.
set /p "reset_choice=%CYAN%Escolha: %RESET%"

if "%reset_choice%"=="1" (
    echo %YELLOW%🔄 Reset Suave...%RESET%
    docker-compose restart
    echo %GREEN%✅ Containers reiniciados%RESET%
)
if "%reset_choice%"=="2" (
    echo %YELLOW%🔄 Reset Médio...%RESET%
    docker-compose down
    docker-compose build --no-cache
    docker-compose up -d
    echo %GREEN%✅ Imagens reconstruídas%RESET%
)
if "%reset_choice%"=="3" (
    echo %RED%🔄 Reset Completo...%RESET%
    call init.bat
    echo %GREEN%✅ Reset completo realizado%RESET%
)
if "%reset_choice%"=="4" (
    echo %YELLOW%🗑️  Limpando logs antigos...%RESET%
    if exist "logs" (
        forfiles /p logs /s /m *.log /d -7 /c "cmd /c del @path" 2>nul
        echo %GREEN%✅ Logs antigos removidos%RESET%
    ) else (
        echo %YELLOW%⚠️  Pasta de logs não encontrada%RESET%
    )
)
if "%reset_choice%"=="0" goto MAIN_MENU

pause
goto MAIN_MENU

:SERVICE_STATUS
echo.
echo %BOLD%%BLUE%📊 STATUS DOS SERVIÇOS%RESET%
echo.

:: Docker status
docker info >nul 2>&1
if !errorlevel! equ 0 (
    echo %GREEN%✅ Docker Engine: Running%RESET%
) else (
    echo %RED%❌ Docker Engine: Stopped%RESET%
)

:: Containers status
echo.
echo %CYAN%🐳 Containers:%RESET%
docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}" 2>nul

:: API endpoints test
echo.
echo %CYAN%🌐 API Endpoints:%RESET%
timeout /t 1 >nul
curl -s -o nul -w "HTTP: %%{http_code}" http://localhost:5000/health 2>nul
echo.
curl -k -s -o nul -w "HTTPS: %%{http_code}" https://localhost:5001/health 2>nul
echo.

:: Database test
echo.
echo %CYAN%🗄️  Database:%RESET%
docker-compose exec -T db sqlcmd -S localhost -U sa -P "FastFood2024" -Q "SELECT 1" >nul 2>&1
if !errorlevel! equ 0 (
    echo %GREEN%✅ SQL Server: Connected%RESET%
) else (
    echo %RED%❌ SQL Server: Not Connected%RESET%
)

:: Resource usage
echo.
echo %CYAN%💾 Resource Usage:%RESET%
docker stats --no-stream --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}" 2>nul

pause
goto MAIN_MENU

:EXIT
echo.
echo %BOLD%%CYAN%👋 Obrigado por usar o FastFood API Quick Launcher!%RESET%
echo.
exit /b 0

:ShowSuccessMessage
echo %BOLD%%GREEN%🎉 FASTFOOD API PRONTO PARA USO!%RESET%
echo.
echo %WHITE%🔗 Links Rápidos:%RESET%
echo %CYAN%   • Swagger UI: https://localhost:5001/swagger%RESET%
echo %CYAN%   • API HTTP:   http://localhost:5000%RESET%
echo %CYAN%   • Health:     http://localhost:5000/health%RESET%
echo.
echo %WHITE%🔧 Comandos Úteis:%RESET%
echo %YELLOW%   launcher.bat  %RESET%- Este menu
echo %YELLOW%   monitor.bat   %RESET%- Monitor em tempo real
echo %YELLOW%   help.bat      %RESET%- Documentação completa
echo.
exit /b 0
