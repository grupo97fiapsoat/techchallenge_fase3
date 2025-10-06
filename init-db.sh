#!/bin/bash

# --- Espera inicial ---
echo "Aguardando RDS iniciar..."
sleep 60

# --- Variáveis de conexão ---
RDS_ENDPOINT="${RDS_ENDPOINT:-fastfood-sqlserver.c54y2e8cq0v5.sa-east-1.rds.amazonaws.com,1433}"
DB_USER="${DB_USER:-sa}"
DB_PASS="${DB_PASS:-FastFood2025}"

# --- Detecta sqlcmd ---
SQLCMD_PATH=$(which sqlcmd 2>/dev/null)
if [ -z "$SQLCMD_PATH" ]; then
    echo "ERRO: sqlcmd não encontrado no PATH. Instale o SQL Server Tools."
    exit 1
fi
echo "sqlcmd encontrado em: $SQLCMD_PATH"

# --- Verifica/cria banco ---
echo "Verificando se o banco de dados FastFood existe no RDS..."
SQL_QUERY="IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'FastFood')
BEGIN
    PRINT 'Criando banco de dados FastFood...';
    CREATE DATABASE FastFood;
    PRINT 'Banco de dados FastFood criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'Banco de dados FastFood já existe.';
END"

"$SQLCMD_PATH" -S "$RDS_ENDPOINT" -U "$DB_USER" -P "$DB_PASS" -d master -Q "$SQL_QUERY"

# --- Detecta dotnet ---
DOTNET_PATH=$(which dotnet 2>/dev/null)
if [ -z "$DOTNET_PATH" ]; then
    echo "ERRO: dotnet não encontrado no PATH. Instale o .NET SDK."
    exit 1
fi
echo "dotnet encontrado em: $DOTNET_PATH"

# --- Executa migrações EF Core ---
PROJECT_PATH="./src/FastFood.Api"  # ajuste para o caminho correto do projeto
if [ ! -d "$PROJECT_PATH" ]; then
    echo "ERRO: diretório do projeto não encontrado: $PROJECT_PATH"
    exit 1
fi

cd "$PROJECT_PATH" || exit 1
echo "Executando migrações EF Core..."
"$DOTNET_PATH" ef database update --context FastFoodDbContext --no-build

echo "Setup concluído!"
