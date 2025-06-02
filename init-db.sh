#!/bin/bash
# Aguardar o SQL Server iniciar
sleep 30s

echo "Verificando se o banco de dados FastFood existe..."

# Verificar se o banco de dados já existe e criá-lo se não existir
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FastFood2025 -d master -Q "
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FastFood')
BEGIN
    PRINT 'Criando banco de dados FastFood...';
    CREATE DATABASE FastFood;
    PRINT 'Banco de dados FastFood criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'Banco de dados FastFood já existe.';
END
"

# Executar migrações após a criação do banco de dados
echo "Executando migrações..."
cd /app/src/FastFood.Api
dotnet ef database update --context FastFoodDbContext --no-build
