#!/bin/bash
# docker-init-database.sh - Script para executar migrations dentro do container Docker
# Este script é executado dentro do container para inicializar o banco

set -e

echo "🔧 [CONTAINER] Inicializando banco de dados..."
echo "📅 Data/Hora: $(date)"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para log colorido
log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Aguardar o SQL Server estar pronto
log_info "Aguardando SQL Server estar disponível..."

# Usar sqlcmd se disponível, senão usar dotnet ef
if command -v sqlcmd &> /dev/null; then
    # Tentar conectar usando sqlcmd
    for i in {1..60}; do
        if sqlcmd -S "$DB_HOST" -U "$DB_USER" -P "$DB_PASSWORD" -Q "SELECT 1" > /dev/null 2>&1; then
            log_success "SQL Server está pronto!"
            break
        fi
        
        if [ $i -eq 60 ]; then
            log_error "Timeout: SQL Server não respondeu após 60 segundos"
            exit 1
        fi
        
        echo -n "."
        sleep 1
    done
else
    # Usar dotnet ef database can-connect
    for i in {1..60}; do
        if dotnet FastFood.Api.dll --check-db > /dev/null 2>&1; then
            log_success "Banco de dados está acessível!"
            break
        fi
        
        if [ $i -eq 60 ]; then
            log_error "Timeout: Não foi possível conectar ao banco após 60 segundos"
            exit 1
        fi
        
        echo -n "."
        sleep 1
    done
fi

# Executar migrations usando a aplicação
log_info "Executando migrations via aplicação..."

# Usar o comando customizado da aplicação para migrations
dotnet FastFood.Api.dll --migrate

if [ $? -eq 0 ]; then
    log_success "Migrations executadas com sucesso!"
else
    log_error "Erro ao executar migrations!"
    exit 1
fi

log_success "🎉 Inicialização do banco concluída!"
