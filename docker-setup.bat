@echo off
setlocal enabledelayedexpansion
:: FastFood API - Docker Setup Script for Windows

echo 🚀 FastFood API - Setup Script for Windows
echo ============================================

:: Check if Docker is installed
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker não está instalado. Por favor, instale o Docker Desktop primeiro.
    echo Acesse: https://www.docker.com/products/docker-desktop/
    pause
    exit /b 1
)

:: Check if Docker Compose is installed
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker Compose não está instalado. Por favor, instale o Docker Desktop com Docker Compose.
    pause
    exit /b 1
)

:: Check if .env file exists
if not exist ".env" (
    echo ⚠️ Arquivo .env não encontrado. Usando configurações padrão...
)

:: Check if certificates exist, if not suggest generation
if not exist ".\certs\fastfood-dev.pfx" (
    echo 🔐 Certificados de desenvolvimento não encontrados.
    echo 💡 Execute: .\scripts\generate-dev-certs.bat para gerar certificados
    echo 💡 Ou use WSL: bash ./scripts/generate-dev-certs.sh
    echo.
    set /p choice=Deseja continuar sem HTTPS [s] ou gerar certificados agora [n]? (s/n): 
    if /i "!choice!"=="n" (
        echo Executando geração de certificados...
        call .\scripts\generate-dev-certs.bat
        if %errorlevel% neq 0 (
            echo ❌ Falha na geração de certificados. Continuando com HTTP apenas.
        )
    )
)

set ACTION=%1
if "%ACTION%"=="" set ACTION=start

if "%ACTION%"=="start" goto :start
if "%ACTION%"=="stop" goto :stop
if "%ACTION%"=="restart" goto :restart
if "%ACTION%"=="logs" goto :logs
if "%ACTION%"=="status" goto :status
if "%ACTION%"=="clean" goto :clean
if "%ACTION%"=="help" goto :help

echo ❌ Comando inválido: %ACTION%
echo Use 'docker-setup.bat help' para ver os comandos disponíveis.
exit /b 1

:start
echo 🔧 Iniciando a aplicação FastFood API...
docker-compose down --remove-orphans
docker-compose up --build -d

echo ⏳ Aguardando os serviços ficarem prontos...
timeout /t 10 /nobreak >nul

echo.
echo 🎉 FastFood API foi iniciada!
echo.
echo 📊 Informações sobre Database Migrations:
echo    • As migrations são executadas automaticamente
echo    • Container 'migrations' executa antes da API
echo    • Aguarde a conclusão para usar a API
echo.
echo 🔍 Para monitorar as migrations:
echo    docker-compose logs migrations
echo.
echo 📍 URLs disponíveis:
echo    • API: http://localhost:5000
echo    • Swagger: http://localhost:5000/swagger
echo    • Health Check: http://localhost:5000/health
echo.
echo 📊 Banco de dados:
echo    • Servidor: localhost:1433
echo    • Database: FastFoodDb
echo    • Usuário: sa
echo.
echo 🔧 Comandos úteis:
echo    • Ver logs: docker-setup.bat logs
echo    • Logs migrations: docker-compose logs migrations
echo    • Parar: docker-setup.bat stop
echo    • Restart: docker-setup.bat restart
echo.
goto :end

:stop
echo 🛑 Parando a aplicação FastFood API...
docker-compose down
echo ✅ Aplicação parada com sucesso!
goto :end

:restart
echo 🔄 Reiniciando a aplicação FastFood API...
docker-compose down
docker-compose up --build -d
echo ✅ Aplicação reiniciada com sucesso!
goto :end

:logs
echo 📋 Exibindo logs da aplicação...
docker-compose logs -f
goto :end

:status
echo 📊 Status dos serviços:
docker-compose ps
goto :end

:clean
echo 🧹 Limpando containers e volumes...
docker-compose down --volumes --remove-orphans
docker system prune -f
echo ✅ Limpeza concluída!
goto :end

:help
echo Uso: docker-setup.bat [comando]
echo.
echo Comandos disponíveis:
echo   start    - Inicia a aplicação (padrão)
echo   stop     - Para a aplicação
echo   restart  - Reinicia a aplicação
echo   logs     - Exibe os logs
echo   status   - Mostra o status dos serviços
echo   clean    - Limpa containers e volumes
echo   help     - Exibe esta ajuda
goto :end

:end
pause
