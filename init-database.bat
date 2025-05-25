@echo off
echo ================================================================
echo          MIGRACAO DIRETA DO BANCO DE DADOS FASTFOOD
echo ================================================================
echo.

REM Definir variáveis
set DB_SERVER=localhost,1434
set DB_USER=sa
set DB_PASSWORD=FastFood2025
set DB_NAME=FastFood

echo Configuracao:
echo - Servidor: %DB_SERVER%
echo - Banco de dados: %DB_NAME%
echo - Usuario: %DB_USER%
echo.

REM Mudar para o diretório da API
cd %~dp0\src\FastFood.Api

REM Configurar string de conexão como variável de ambiente
set ConnectionStrings__DefaultConnection=Server=%DB_SERVER%;Database=%DB_NAME%;User Id=%DB_USER%;Password=%DB_PASSWORD%;TrustServerCertificate=true;MultipleActiveResultSets=true

echo Executando migrations do Entity Framework Core...
dotnet ef database update --verbose

IF %ERRORLEVEL% NEQ 0 (
    echo [ERRO] Falha ao executar as migrations. Codigo: %ERRORLEVEL%
    goto end
)

echo.
echo ================================================================
echo        MIGRATIONS APLICADAS COM SUCESSO!
echo ================================================================
echo.

:end
cd %~dp0
pause
