@echo off
setlocal enabledelayedexpansion
:: FastFood API - Inicializacao Completa End-to-End

echo [FASTFOOD API] Inicializando ambiente completo...
echo ================================================

:: Check if Docker is running
echo [VERIFICACAO] Verificando se Docker esta rodando...
docker info >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Docker nao esta rodando. Inicie o Docker Desktop primeiro.
    echo [DICA] Aguarde o Docker Desktop inicializar completamente antes de executar este script.
    pause
    exit /b 1
)
echo [OK] Docker esta rodando.

:: Check and create .env file if it doesn't exist
if not exist ".env" (
    echo [AVISO] Arquivo .env nao encontrado. Criando automaticamente...
    (
        echo # Environment Variables for FastFood API
        echo # Database Configuration
        echo DB_PASSWORD=FastFood@2024!#
        echo.
        echo # Application Configuration
        echo ASPNETCORE_ENVIRONMENT=Development
        echo.
        echo # SSL/TLS Certificate Configuration
        echo CERT_PASSWORD=fastfood123
        echo.
        echo # SQL Server Configuration
        echo MSSQL_PID=Express
        echo.
        echo # Timezone
        echo TZ=America/Sao_Paulo
    ) > .env
    echo [OK] Arquivo .env criado com configuracoes padrao.
) else (
    echo [OK] Arquivo .env encontrado.
)

:: Stop any running containers and clean up
echo [LIMPEZA] Parando containers existentes...
docker-compose down --remove-orphans >nul 2>&1

:: Generate certificates if needed
if not exist ".\certs" mkdir ".\certs"
if not exist ".\certs\fastfood-dev.pfx" (
    echo [CERTIFICADOS] Gerando certificados de desenvolvimento...
    if exist ".\scripts\generate-dev-certs.bat" (
        call .\scripts\generate-dev-certs.bat >nul 2>&1
        if %errorlevel% equ 0 (
            echo [OK] Certificados gerados com sucesso.
        ) else (
            echo [AVISO] Falha na geracao de certificados. API rodara apenas em HTTP.
        )
    ) else (
        echo [AVISO] Script de certificados nao encontrado. API rodara apenas em HTTP.
    )
) else (
    echo [OK] Certificados ja existem.
)

:: Build and start the application
echo [BUILD] Fazendo build e iniciando containers...
echo [INFO] Isso pode levar alguns minutos na primeira execucao...
docker-compose up --build -d

if %errorlevel% equ 0 (
    echo [OK] Containers iniciados com sucesso!
    
    :: Wait for services to be ready
    echo [AGUARDANDO] Verificando se os servicos estao prontos...
    timeout /t 5 /nobreak >nul
    
    :: Check database
    echo [DB] Verificando banco de dados...
    docker-compose exec -T db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "FastFood@2024!#" -Q "SELECT 1" >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] Banco de dados esta respondendo.
    ) else (
        echo [AVISO] Banco de dados ainda nao esta pronto. Aguarde mais alguns segundos.
    )
    
    :: Check migrations
    echo [MIGRATIONS] Verificando status das migrations...
    docker-compose logs migrations | find "successfully" >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] Migrations executadas com sucesso.
    ) else (
        echo [INFO] Migrations ainda em execucao ou com problemas.
        echo [DICA] Execute 'docker-compose logs migrations' para detalhes.
    )
      :: Check API
    echo [API] Verificando API...
    timeout /t 10 /nobreak >nul
    
    :: Try to check API health (PowerShell fallback if curl not available)
    curl -s -o nul -w "%%{http_code}" http://localhost:5000/health 2>nul | find "200" >nul
    if %errorlevel% equ 0 (
        echo [OK] API esta respondendo.
    ) else (
        :: Fallback using PowerShell
        powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost:5000/health' -TimeoutSec 5; if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } } catch { exit 1 }" >nul 2>&1
        if %errorlevel% equ 0 (
            echo [OK] API esta respondendo.
        ) else (
            echo [AVISO] API ainda nao esta pronta. Aguarde mais alguns segundos.
        )
    )
      echo.
    echo ========================================
    echo [SUCESSO] FastFood API iniciado!
    echo ========================================
    echo.
    echo URLs disponiveis:
    echo - HTTP:    http://localhost:5000
    echo - HTTPS:   https://localhost:5001
    echo - Swagger: http://localhost:5000/swagger
    echo - Health:  http://localhost:5000/health
    echo.
    echo Banco de dados:
    echo - Servidor: localhost:1433
    echo - Database: FastFoodDb
    echo - Usuario:  sa
    echo - Senha:    FastFood@2024!#
    echo.
    echo Status dos containers:
    docker-compose ps
    echo.
    echo Comandos uteis:
    echo - Ver logs:        docker-compose logs -f
    echo - Logs da API:     docker-compose logs -f api
    echo - Logs do banco:   docker-compose logs -f db
    echo - Parar tudo:      docker-compose down
    echo - Reiniciar:       docker-compose restart
    echo.
    echo [DICA] Se a API nao estiver respondendo imediatamente,
    echo        aguarde alguns segundos para os servicos terminarem de inicializar.
    echo.
) else (
    echo [ERRO] Falha ao iniciar a aplicacao!
    echo.
    echo Para diagnosticar o problema:
    echo 1. Execute: docker-compose logs
    echo 2. Verifique se todas as portas estao livres
    echo 3. Verifique se o Docker Desktop tem recursos suficientes
    echo.
)

pause
