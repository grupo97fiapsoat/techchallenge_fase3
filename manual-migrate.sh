#!/bin/bash
set -e

echo "[MIGRATION-MANUAL] Iniciando processo de migração manual..."

# Variáveis de ambiente
DB_PASSWORD=${DB_PASSWORD:-FastFood2025}
DB_NAME=FastFood
DB_SERVER=db
DB_USER=sa

# Configurar a string de conexão
export ConnectionStrings__DefaultConnection="Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=true;"

echo "[MIGRATION-MANUAL] String de conexão configurada"
echo "[MIGRATION-MANUAL] Executando dotnet ef database update diretamente..."

# Navegando para o diretório do projeto API
cd /app/src/FastFood.Api

# Instalando a ferramenta dotnet-ef se necessário
if ! command -v dotnet-ef &> /dev/null; then
    echo "[MIGRATION-MANUAL] Instalando dotnet-ef tools..."
    dotnet tool install --global dotnet-ef || true
fi

# Adicionando o caminho da ferramenta ao PATH
export PATH="$PATH:/root/.dotnet/tools"

# Verificando se dotnet-ef está disponível
if ! command -v dotnet-ef &> /dev/null; then
    echo "[MIGRATION-MANUAL] ERRO: dotnet-ef não está disponível no PATH"
    exit 1
fi

# Executando a migração
echo "[MIGRATION-MANUAL] Executando dotnet ef database update..."
dotnet ef database update --verbose

if [ $? -eq 0 ]; then
    echo "[MIGRATION-MANUAL] Migração concluída com sucesso!"
else
    echo "[MIGRATION-MANUAL] ERRO ao executar migrations com dotnet ef"
    exit 1
fi
