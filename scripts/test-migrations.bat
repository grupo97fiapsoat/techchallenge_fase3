@echo off
REM test-migrations.bat - Script de teste rápido para verificar migrations no Windows

echo 🧪 TESTE RÁPIDO - Sistema de Migrations
echo ======================================
echo.

REM 1. Verificar se Docker está rodando
echo ℹ️  Verificando Docker...
docker ps >nul 2>&1
if errorlevel 1 (
    echo ❌ Docker não está rodando ou não há permissão
    exit /b 1
)
echo ✅ Docker OK

REM 2. Limpar ambiente anterior
echo ℹ️  Limpando ambiente anterior...
docker-compose down -v --remove-orphans >nul 2>&1
echo ✅ Ambiente limpo

REM 3. Iniciar containers
echo ℹ️  Iniciando containers...
docker-compose up -d

REM 4. Aguardar migrations
echo ℹ️  Aguardando conclusão das migrations...
for /l %%i in (1,1,120) do (
    docker-compose ps migrations | findstr "Exit 0" >nul 2>&1
    if not errorlevel 1 (
        echo ✅ Migrations concluídas!
        goto migrations_done
    )
    
    if %%i==120 (
        echo ❌ Timeout: Migrations não concluídas em 2 minutos
        docker-compose logs migrations
        exit /b 1
    )
    
    echo|set /p="."
    timeout /t 1 >nul
)

:migrations_done

REM 5. Verificar API
echo ℹ️  Verificando API...
for /l %%i in (1,1,60) do (
    curl -f -s http://localhost:5000/health >nul 2>&1
    if not errorlevel 1 (
        echo ✅ API respondendo!
        goto api_ready
    )
    
    if %%i==60 (
        echo ❌ API não respondeu após 1 minuto
        docker-compose logs api
        exit /b 1
    )
    
    echo|set /p="."
    timeout /t 1 >nul
)

:api_ready

REM 6. Testar endpoint
echo ℹ️  Testando endpoints...
curl -f -s http://localhost:5000/api/products >nul 2>&1
if not errorlevel 1 (
    echo ✅ Endpoint /api/products OK
) else (
    echo ⚠️  Endpoint /api/products não respondeu (pode ser normal se não há dados)
)

REM 7. Verificar logs das migrations
echo ℹ️  Verificando logs das migrations...
docker-compose logs migrations | findstr "Database migration completed" >nul 2>&1
if not errorlevel 1 (
    echo ✅ Migrations executadas com sucesso!
) else (
    echo ❌ Migrations não foram executadas corretamente
    docker-compose logs migrations
    exit /b 1
)

REM 8. Resumo final
echo.
echo ✅ 🎉 TESTE CONCLUÍDO COM SUCESSO!
echo.
echo 📊 Resultados:
echo    ✅ Docker funcionando
echo    ✅ Containers iniciados
echo    ✅ Migrations executadas
echo    ✅ API respondendo
echo    ✅ Health check OK
echo.
echo 📍 URLs disponíveis:
echo    • API: http://localhost:5000
echo    • Swagger: http://localhost:5000/swagger
echo    • Health: http://localhost:5000/health
echo.
echo 🔧 Para ver logs:
echo    docker-compose logs migrations
echo    docker-compose logs api
echo.
echo 🛑 Para parar:
echo    docker-compose down
echo.

pause
