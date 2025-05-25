@echo off
REM FastFood API - Docker Setup Script
REM Codificacao: ANSI (sem BOM)

echo ========================================================================
echo                        FASTFOOD API - SETUP COMPLETO
echo ========================================================================
echo [INFO] Iniciando configuracao automatica
echo ========================================================================

echo [STEP 1] Verificando Docker...
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Docker nao encontrado! Instale o Docker Desktop.
    pause
    exit /b 1
)

docker ps >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Docker daemon nao esta rodando!
    echo [ACAO] Inicie o Docker Desktop primeiro.
    pause
    exit /b 1
)
echo [OK] Docker esta funcionando.

echo.
echo [STEP 2] Configurando ambiente...
if not exist ".env" (
    echo # FastFood API Environment > .env
    echo DB_PASSWORD=FastFood2025 >> .env
    echo ASPNETCORE_ENVIRONMENT=Development >> .env
    echo CERT_PASSWORD=fastfood123 >> .env
    echo MSSQL_PID=Express >> .env
    echo [OK] Arquivo .env criado.
) else (
    echo [OK] Arquivo .env ja existe.
)

echo.
echo [STEP 3] Limpando containers antigos...
docker-compose down --remove-orphans >nul 2>&1
echo [OK] Limpeza concluida.

echo.
echo [STEP 4] Criando certificados...
if not exist "certs" mkdir certs
if not exist "certs\fastfood-dev.pfx" (
    echo [INFO] Gerando certificados de desenvolvimento...
    if exist "scripts\generate-dev-certs.bat" (
        cd scripts
        call generate-dev-certs.bat
        cd ..
    ) else (
        echo [WARN] Script de certificados nao encontrado. Criando dummy...
        echo dummy-certificate > certs\fastfood-dev.pfx
    )
) else (
    echo [OK] Certificados ja existem.
)
echo [OK] Certificados prontos.

echo.
echo [STEP 5] Validando configuracao Docker...
docker-compose config >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Problema no docker-compose.yml!
    echo [DEBUG] Executando docker-compose config para diagnostico:
    docker-compose config
    pause
    exit /b 1
)
echo [OK] Configuracao Docker validada.

echo.
echo [STEP 6] Construindo imagens Docker...
echo [INFO] Este processo pode demorar alguns minutos...
docker-compose build --no-cache
if %errorlevel% neq 0 (
    echo [ERRO] Falha ao construir imagens Docker!
    echo [DEBUG] Verifique os Dockerfiles e tente novamente.
    pause
    exit /b 1
)
echo [OK] Imagens Docker construidas com sucesso.

echo.
echo [STEP 7] Iniciando containers...
docker-compose up -d
if %errorlevel% neq 0 (
    echo [ERRO] Falha ao iniciar containers!
    echo [DEBUG] Executando logs para diagnostico:
    docker-compose logs
    pause
    exit /b 1
)
echo [OK] Containers iniciados.

echo.
echo [STEP 8] Aguardando database ficar pronto...
echo [INFO] Aguardando 15 segundos para SQL Server inicializar...
timeout /t 15 /nobreak >nul

echo [STEP 8] Executando migrations...
echo [INFO] Tentando executar migrations...
docker-compose exec -T fastfood-migrations dotnet ef database update --no-build >nul 2>&1
if %errorlevel% neq 0 (
    echo [WARN] Metodo exec falhou. Tentando metodo alternativo...
    docker-compose run --rm fastfood-migrations >nul 2>&1
    if %errorlevel% neq 0 (
        echo [WARN] Migrations podem ter falhado. Verifique manualmente.
        echo [CMD] docker-compose logs fastfood-migrations
    ) else (
        echo [OK] Migrations executadas via run.
    )
) else (
    echo [OK] Migrations executadas via exec.
)

echo.
echo ========================================================================
echo                           SETUP CONCLUIDO!
echo ========================================================================
echo API disponivel em:
echo  - HTTP:  http://localhost:5000
echo  - HTTPS: https://localhost:5001
echo  - Swagger: http://localhost:5000/swagger
echo.
echo Banco de dados:
echo  - Server: localhost,1434
echo  - Usuario: sa
echo  - Senha: FastFood2025
echo.
echo Comandos uteis:
echo  - Ver logs: docker-compose logs -f
echo  - Parar: docker-compose down
echo  - Reiniciar: docker-compose restart
echo  - Status: docker-compose ps
echo ========================================================================
pause
