@echo off
:: FastFood API - Script de inicializacao simples para Windows
:: Este script inicia a aplicacao FastFood completa com Docker Compose

echo ================================================
echo           FASTFOOD API - STARTUP
echo ================================================
echo.

:: Verifica se Docker esta instalado
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Docker nao encontrado. Instale Docker Desktop primeiro.
    echo Download: https://www.docker.com/products/docker-desktop/
    pause
    exit /b 1
)

echo [INFO] Parando containers existentes...
docker-compose down --remove-orphans

echo [INFO] Iniciando FastFood API com todas as dependencias...
docker-compose up --build -d

echo.
echo [AGUARDE] Inicializando servicos (30 segundos)...
timeout /t 30 /nobreak >nul

echo.
echo ================================================
echo         FASTFOOD API INICIADA COM SUCESSO!
echo ================================================
echo.
echo URLs da aplicacao:
echo   API Base: http://localhost:5000
echo   Swagger:  http://localhost:5000/swagger
echo   Health:   http://localhost:5000/health
echo.
echo Banco de dados SQL Server:
echo   Servidor: localhost:1433
echo   Database: FastFoodDb
echo   Usuario:  sa
echo   Senha:    (definida no arquivo .env)
echo.
echo Comandos uteis:
echo   Ver logs:        docker-compose logs -f
echo   Ver migrations:  docker-compose logs migrations
echo   Status:          docker-compose ps
echo   Parar tudo:      docker-compose down
echo.
echo ================================================
echo.

pause
