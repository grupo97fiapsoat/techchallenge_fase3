@echo off
echo ========================================
echo Executando migrations do Entity Framework
echo ========================================

cd %~dp0\src\FastFood.Api

echo Verificando dependencias...
dotnet restore

echo Atualizando o banco de dados...
dotnet ef database update --verbose

if %ERRORLEVEL% NEQ 0 (
    echo ERRO: Falha ao executar migrations!
    exit /b %ERRORLEVEL%
)

echo.
echo ========================================
echo Migrations aplicadas com sucesso!
echo ========================================
echo.
