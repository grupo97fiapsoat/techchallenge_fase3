#!/bin/bash
# init-database.sh - Script para inicialização do banco de dados
# Este script executa as migrations do Entity Framework Core

set -e

echo "🔧 Inicializando banco de dados..."
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

# Verificar se estamos no diretório correto
if [ ! -f "src/FastFood.Api/FastFood.Api.csproj" ]; then
    log_error "Este script deve ser executado no diretório raiz do projeto!"
    exit 1
fi

# Verificar se o SDK do .NET está disponível
if ! command -v dotnet &> /dev/null; then
    log_error "SDK do .NET não encontrado!"
    exit 1
fi

log_info "SDK do .NET encontrado: $(dotnet --version)"

# Navegar para o projeto da API
cd src/FastFood.Api

# 1. Verificar se existem migrations
log_info "Verificando migrations existentes..."
MIGRATIONS_DIR="../FastFood.Infrastructure/Migrations"

if [ ! -d "$MIGRATIONS_DIR" ] || [ -z "$(ls -A $MIGRATIONS_DIR 2>/dev/null)" ]; then
    log_warning "Nenhuma migration encontrada. Criando migration inicial..."
    
    # Criar migration inicial
    dotnet ef migrations add InitialCreate --project ../FastFood.Infrastructure --startup-project . --output-dir Migrations
    
    if [ $? -eq 0 ]; then
        log_success "Migration inicial criada com sucesso!"
    else
        log_error "Erro ao criar migration inicial!"
        exit 1
    fi
else
    log_info "Migrations existentes encontradas em $MIGRATIONS_DIR"
fi

# 2. Aguardar o SQL Server estar pronto (se em container)
if [ ! -z "$SQL_SERVER_HOST" ]; then
    log_info "Aguardando SQL Server estar pronto..."
    
    # Tentar conectar por até 60 segundos
    for i in {1..60}; do
        if dotnet ef database can-connect --project ../FastFood.Infrastructure --startup-project . > /dev/null 2>&1; then
            log_success "Conexão com banco de dados estabelecida!"
            break
        fi
        
        if [ $i -eq 60 ]; then
            log_error "Timeout: Não foi possível conectar ao banco de dados após 60 segundos"
            exit 1
        fi
        
        echo -n "."
        sleep 1
    done
fi

# 3. Executar migrations
log_info "Executando migrations..."
dotnet ef database update --project ../FastFood.Infrastructure --startup-project .

if [ $? -eq 0 ]; then
    log_success "Migrations executadas com sucesso!"
else
    log_error "Erro ao executar migrations!"
    exit 1
fi

# 4. Verificar se o banco foi criado corretamente
log_info "Verificando estrutura do banco..."
if dotnet ef database can-connect --project ../FastFood.Infrastructure --startup-project . > /dev/null 2>&1; then
    log_success "Banco de dados está funcionando corretamente!"
else
    log_error "Problema na verificação do banco de dados!"
    exit 1
fi

echo ""
log_success "🎉 Inicialização do banco de dados concluída com sucesso!"
echo ""
