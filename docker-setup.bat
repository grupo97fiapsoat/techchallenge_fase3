@echo off
setlocal enabledelayedexpansion
:: FastFood API - Docker Setup Script for Windows

echo [FASTFOOD API] Setup Script for Windows
echo ============================================

:: Check if Docker is installed
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Docker nao esta instalado. Por favor, instale o Docker Desktop primeiro.
    echo Acesse: https://www.docker.com/products/docker-desktop/
    pause
    exit /b 1
)

:: Check if Docker Compose is installed
docker-compose --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Docker Compose nao esta instalado. Por favor, instale o Docker Desktop com Docker Compose.
    pause
    exit /b 1
)

:: Check if .env file exists
if not exist ".env" (
    echo [AVISO] Arquivo .env nao encontrado. Usando configuracoes padrao...
)

:: Check if certificates exist, if not suggest generation
if not exist ".\certs\fastfood-dev.pfx" (
    echo [INFO] Certificados de desenvolvimento nao encontrados.
    echo [DICA] Execute: .\scripts\generate-dev-certs.bat para gerar certificados
    echo [DICA] Ou use WSL: bash ./scripts/generate-dev-certs.sh
    echo.
    set /p choice=Deseja continuar sem HTTPS [s] ou gerar certificados agora [n]? (s/n): 
    if /i "!choice!"=="n" (
        echo Executando geracao de certificados...
        call .\scripts\generate-dev-certs.bat
        if %errorlevel% neq 0 (
            echo [ERRO] Falha na geracao de certificados. Continuando com HTTP apenas.
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

echo [ERRO] Comando invalido: %ACTION%
echo Use 'docker-setup.bat help' para ver os comandos disponiveis.
exit /b 1

:start
echo [INFO] Iniciando a aplicacao FastFood API...
docker-compose down --remove-orphans
docker-compose up --build -d

echo [INFO] Aguardando os servicos ficarem prontos...
timeout /t 10 /nobreak >nul

echo.
echo [SUCESSO] FastFood API foi iniciada!
echo.
echo [INFO] Informacoes sobre Database Migrations:
echo    * As migrations sao executadas automaticamente
echo    * Container 'migrations' executa antes da API
echo    * Aguarde a conclusao para usar a API
echo.
echo [DICA] Para monitorar as migrations:
echo    docker-compose logs migrations
echo.
echo [URLS] URLs disponiveis:
echo    * API: http://localhost:5000
echo    * Swagger: http://localhost:5000/swagger
echo    * Health Check: http://localhost:5000/health
echo.
echo [DB] Banco de dados:
echo    * Servidor: localhost:1433
echo    * Database: FastFoodDb
echo    * Usuario: sa
echo.
echo [COMANDOS] Comandos uteis:
echo    * Ver logs: docker-setup.bat logs
echo    * Logs migrations: docker-compose logs migrations
echo    * Parar: docker-setup.bat stop
echo    * Restart: docker-setup.bat restart
echo.
goto :end

:stop
echo [INFO] Parando a aplicacao FastFood API...
docker-compose down
echo [SUCESSO] Aplicacao parada com sucesso!
goto :end

:restart
echo [INFO] Reiniciando a aplicacao FastFood API...
docker-compose down
docker-compose up --build -d
echo [SUCESSO] Aplicacao reiniciada com sucesso!
goto :end

:logs
echo [INFO] Exibindo logs da aplicacao...
docker-compose logs -f
goto :end

:status
echo [INFO] Status dos servicos:
docker-compose ps
goto :end

:clean
echo [INFO] Limpando containers e volumes...
docker-compose down --volumes --remove-orphans
docker system prune -f
echo [SUCESSO] Limpeza concluida!
goto :end

:help
echo Uso: docker-setup.bat [comando]
echo.
echo Comandos disponiveis:
echo   start    - Inicia a aplicacao (padrao)
echo   stop     - Para a aplicacao
echo   restart  - Reinicia a aplicacao
echo   logs     - Exibe os logs
echo   status   - Mostra o status dos servicos
echo   clean    - Limpa containers e volumes
echo   help     - Exibe esta ajuda
goto :end

:end
pause
