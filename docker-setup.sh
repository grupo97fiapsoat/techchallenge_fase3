#!/bin/bash
# FastFood API - Docker Setup Script for WSL Ubuntu

set -e

echo "🚀 FastFood API - Setup Script"
echo "==============================="

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker não está instalado. Por favor, instale o Docker primeiro."
    echo "Siga as instruções em: https://docs.docker.com/engine/install/ubuntu/"
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "❌ Docker Compose não está instalado. Por favor, instale o Docker Compose primeiro."
    echo "Siga as instruções em: https://docs.docker.com/compose/install/"
    exit 1
fi

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo "⚠️  Arquivo .env não encontrado. Criando um com valores padrão..."
    cp .env .env.backup 2>/dev/null || true
    echo "✅ Arquivo .env criado com sucesso!"
fi

# Check if certificates exist, if not generate them
if [ ! -f "./certs/fastfood-dev.pfx" ]; then
    echo "🔐 Certificados de desenvolvimento não encontrados. Gerando..."
    if command -v openssl &> /dev/null; then
        chmod +x ./scripts/generate-dev-certs.sh
        ./scripts/generate-dev-certs.sh
    else
        echo "❌ OpenSSL não está instalado. Por favor, instale o OpenSSL:"
        echo "   sudo apt-get update && sudo apt-get install openssl"
        exit 1
    fi
fi

# Function to start the application
start_app() {
    echo "🔧 Iniciando a aplicação FastFood API..."
    
    # Stop any existing containers
    docker-compose down --remove-orphans
    
    # Build and start the containers
    docker-compose up --build -d
    
    echo "⏳ Aguardando os serviços ficarem prontos..."
    
    # Wait for database to be ready
    echo "📊 Verificando se o banco de dados está pronto..."
    until docker-compose exec -T db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "${DB_PASSWORD:-FastFood@2024!#}" -Q "SELECT 1" &> /dev/null; do
        echo "   ⏳ Aguardando SQL Server..."
        sleep 5
    done
    echo "   ✅ SQL Server está pronto!"
    
    # Wait for API to be ready
    echo "🌐 Verificando se a API está pronta..."
    until curl -f http://localhost:5000/health &> /dev/null; do
        echo "   ⏳ Aguardando API..."
        sleep 5
    done
    echo "   ✅ API está pronta!"
    
    echo ""
    echo "🎉 FastFood API está executando com sucesso!"
    echo ""
    echo "📍 URLs disponíveis:"
    echo "   • API: http://localhost:5000"
    echo "   • Swagger: http://localhost:5000/swagger"
    echo "   • Health Check: http://localhost:5000/health"
    echo ""
    echo "📊 Banco de dados:"
    echo "   • Servidor: localhost:1433"
    echo "   • Database: FastFoodDb"
    echo "   • Usuário: sa"
    echo ""
    echo "🔧 Comandos úteis:"
    echo "   • Ver logs: docker-compose logs -f"
    echo "   • Parar: docker-compose down"
    echo "   • Restart: docker-compose restart"
    echo ""
}

# Function to stop the application
stop_app() {
    echo "🛑 Parando a aplicação FastFood API..."
    docker-compose down
    echo "✅ Aplicação parada com sucesso!"
}

# Function to show logs
show_logs() {
    echo "📋 Exibindo logs da aplicação..."
    docker-compose logs -f
}

# Function to clean up
cleanup() {
    echo "🧹 Limpando containers e volumes..."
    docker-compose down --volumes --remove-orphans
    docker system prune -f
    echo "✅ Limpeza concluída!"
}

# Function to show status
show_status() {
    echo "📊 Status dos serviços:"
    docker-compose ps
}

# Main menu
case "${1:-start}" in
    "start")
        start_app
        ;;
    "stop")
        stop_app
        ;;
    "restart")
        stop_app
        start_app
        ;;
    "logs")
        show_logs
        ;;
    "status")
        show_status
        ;;
    "clean")
        cleanup
        ;;
    "help")
        echo "Uso: $0 [comando]"
        echo ""
        echo "Comandos disponíveis:"
        echo "  start    - Inicia a aplicação (padrão)"
        echo "  stop     - Para a aplicação"
        echo "  restart  - Reinicia a aplicação"
        echo "  logs     - Exibe os logs"
        echo "  status   - Mostra o status dos serviços"
        echo "  clean    - Limpa containers e volumes"
        echo "  help     - Exibe esta ajuda"
        ;;
    *)
        echo "❌ Comando inválido: $1"
        echo "Use '$0 help' para ver os comandos disponíveis."
        exit 1
        ;;
esac
