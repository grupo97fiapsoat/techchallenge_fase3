#!/bin/bash
# test-migrations.sh - Script de teste rápido para verificar migrations

set -e

echo "🧪 TESTE RÁPIDO - Sistema de Migrations"
echo "======================================"
echo ""

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# 1. Verificar se Docker está rodando
log_info "Verificando Docker..."
if ! docker ps &> /dev/null; then
    log_error "Docker não está rodando ou não há permissão"
    exit 1
fi
log_success "Docker OK"

# 2. Limpar ambiente anterior
log_info "Limpando ambiente anterior..."
docker-compose down -v --remove-orphans &> /dev/null || true
log_success "Ambiente limpo"

# 3. Iniciar containers
log_info "Iniciando containers..."
docker-compose up -d

# 4. Aguardar migrations
log_info "Aguardando conclusão das migrations..."
for i in {1..120}; do
    if docker-compose ps migrations | grep -q "Exit 0"; then
        log_success "Migrations concluídas!"
        break
    fi
    
    if [ $i -eq 120 ]; then
        log_error "Timeout: Migrations não concluídas em 2 minutos"
        docker-compose logs migrations
        exit 1
    fi
    
    echo -n "."
    sleep 1
done

# 5. Verificar API
log_info "Verificando API..."
for i in {1..60}; do
    if curl -f -s http://localhost:5000/health > /dev/null 2>&1; then
        log_success "API respondendo!"
        break
    fi
    
    if [ $i -eq 60 ]; then
        log_error "API não respondeu após 1 minuto"
        docker-compose logs api
        exit 1
    fi
    
    echo -n "."
    sleep 1
done

# 6. Testar endpoint
log_info "Testando endpoints..."
if curl -f -s http://localhost:5000/api/products > /dev/null 2>&1; then
    log_success "Endpoint /api/products OK"
else
    log_warning "Endpoint /api/products não respondeu (pode ser normal se não há dados)"
fi

# 7. Verificar logs das migrations
log_info "Verificando logs das migrations..."
if docker-compose logs migrations | grep -q "Database migration completed"; then
    log_success "Migrations executadas com sucesso!"
else
    log_error "Migrations não foram executadas corretamente"
    docker-compose logs migrations
    exit 1
fi

# 8. Resumo final
echo ""
log_success "🎉 TESTE CONCLUÍDO COM SUCESSO!"
echo ""
echo "📊 Resultados:"
echo "   ✅ Docker funcionando"
echo "   ✅ Containers iniciados"
echo "   ✅ Migrations executadas"
echo "   ✅ API respondendo"
echo "   ✅ Health check OK"
echo ""
echo "📍 URLs disponíveis:"
echo "   • API: http://localhost:5000"
echo "   • Swagger: http://localhost:5000/swagger"
echo "   • Health: http://localhost:5000/health"
echo ""
echo "🔧 Para ver logs:"
echo "   docker-compose logs migrations"
echo "   docker-compose logs api"
echo ""
echo "🛑 Para parar:"
echo "   docker-compose down"
echo ""
