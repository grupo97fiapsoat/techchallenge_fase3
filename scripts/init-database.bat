@echo off
REM init-database.bat - Script para inicialização do banco de dados no Windows
REM Este script executa as migrations do Entity Framework Core

setlocal EnableDelayedExpansion

echo 🔧 Inicializando banco de dados...
echo 📅 Data/Hora: %date% %time%

REM Verificar se estamos no diretório correto
if not exist "src\FastFood.Api\FastFood.Api.csproj" (
    echo ❌ Este script deve ser executado no diretório raiz do projeto!
    exit /b 1
)

REM Verificar se o SDK do .NET está disponível
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ SDK do .NET não encontrado!
    exit /b 1
)

echo ℹ️  SDK do .NET encontrado
dotnet --version

REM Navegar para o projeto da API
cd src\FastFood.Api

REM 1. Verificar se existem migrations
echo ℹ️  Verificando migrations existentes...
set MIGRATIONS_DIR=..\FastFood.Infrastructure\Migrations

if not exist "%MIGRATIONS_DIR%" (
    echo ⚠️  Nenhuma migration encontrada. Criando migration inicial...
    goto create_migration
)

dir "%MIGRATIONS_DIR%\*.cs" >nul 2>&1
if errorlevel 1 (
    echo ⚠️  Nenhuma migration encontrada. Criando migration inicial...
    goto create_migration
) else (
    echo ℹ️  Migrations existentes encontradas em %MIGRATIONS_DIR%
    goto update_database
)

:create_migration
REM Criar migration inicial
dotnet ef migrations add InitialCreate --project ..\FastFood.Infrastructure --startup-project . --output-dir Migrations

if errorlevel 1 (
    echo ❌ Erro ao criar migration inicial!
    exit /b 1
) else (
    echo ✅ Migration inicial criada com sucesso!
)

:update_database
REM 2. Aguardar o SQL Server estar pronto (se em container)
if defined SQL_SERVER_HOST (
    echo ℹ️  Aguardando SQL Server estar pronto...
    
    REM Tentar conectar por até 60 segundos
    for /l %%i in (1,1,60) do (
        dotnet ef database can-connect --project ..\FastFood.Infrastructure --startup-project . >nul 2>&1
        if not errorlevel 1 (
            echo ✅ Conexão com banco de dados estabelecida!
            goto execute_migrations
        )
        
        if %%i==60 (
            echo ❌ Timeout: Não foi possível conectar ao banco de dados após 60 segundos
            exit /b 1
        )
        
        echo|set /p="."
        timeout /t 1 >nul
    )
)

:execute_migrations
REM 3. Executar migrations
echo ℹ️  Executando migrations...
dotnet ef database update --project ..\FastFood.Infrastructure --startup-project .

if errorlevel 1 (
    echo ❌ Erro ao executar migrations!
    exit /b 1
) else (
    echo ✅ Migrations executadas com sucesso!
)

REM 4. Verificar se o banco foi criado corretamente
echo ℹ️  Verificando estrutura do banco...
dotnet ef database can-connect --project ..\FastFood.Infrastructure --startup-project . >nul 2>&1
if errorlevel 1 (
    echo ❌ Problema na verificação do banco de dados!
    exit /b 1
) else (
    echo ✅ Banco de dados está funcionando corretamente!
)

echo.
echo 🎉 Inicialização do banco de dados concluída com sucesso!
echo.
