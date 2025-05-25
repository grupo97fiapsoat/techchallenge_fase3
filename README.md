# FastFood API

Sistema de autoatendimento para restaurantes fast-food, desenvolvido como parte do Tech Challenge da Pós-Graduação em Software Architecture da FIAP.

## 🚀 Tecnologias

- .NET 8
- SQL Server 2022
- Docker & Docker Compose
- Swagger/OpenAPI para documentação
- Domain-Driven Design (DDD)
- CQRS com MediatR
- Entity Framework Core
- FluentValidation
- xUnit para testes

## 📋 Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (opcional, apenas para desenvolvimento)

## 🛠️ Configuração

### Variáveis de Ambiente

O projeto já inclui um arquivo `.env` com configurações padrão. Se necessário, você pode ajustar:

```env
# Database Configuration
DB_PASSWORD=FastFood@2024!#

# Application Configuration  
ASPNETCORE_ENVIRONMENT=Development

# SSL/TLS Certificate Configuration
CERT_PASSWORD=fastfood123

# SQL Server Configuration
MSSQL_PID=Express

# Timezone
TZ=America/Sao_Paulo
```

### 🔐 Configuração HTTPS

Por padrão, a aplicação está configurada para usar **HTTPS** seguindo as melhores práticas de segurança.

#### Primeira execução - Gerar certificados de desenvolvimento

**WSL Ubuntu/Linux:**

```bash
chmod +x ./scripts/generate-dev-certs.sh
./scripts/generate-dev-certs.sh
```

**Windows:**

```cmd
.\scripts\generate-dev-certs.bat
```

> ⚠️ **Importante**: Os certificados gerados são autoassinados e destinados apenas para **desenvolvimento**. Para produção, use certificados válidos de uma CA confiável.

## 🐳 Executando com Docker

### Opção 1: Script de inicialização rápida (Recomendado para Windows)

```cmd
git clone https://github.com/seu-usuario/fastfood.git
cd fastfood
run-fastfood.bat
```

### Opção 2: Script completo com opções avançadas

**WSL Ubuntu/Linux:**

```bash
git clone https://github.com/seu-usuario/fastfood.git
cd fastfood
chmod +x docker-setup.sh
./docker-setup.sh start
```

**Windows:**

```cmd
git clone https://github.com/seu-usuario/fastfood.git
cd fastfood
docker-setup.bat start
```

O script irá:

- Verificar se Docker e Docker Compose estão instalados
- Fazer build e iniciar os containers
- Aguardar que os serviços estejam prontos
- Exibir URLs e informações úteis

### Opção 3: Docker Compose manual

**Comandos do script completo:**

**Linux/WSL:**

```bash
./docker-setup.sh stop      # Para a aplicação
./docker-setup.sh restart   # Reinicia a aplicação
./docker-setup.sh logs      # Exibe logs
./docker-setup.sh status    # Mostra status dos serviços
./docker-setup.sh clean     # Limpa containers e volumes
./docker-setup.sh help      # Ajuda
```

**Windows:**

```cmd
docker-setup.bat stop      # Para a aplicação
docker-setup.bat restart   # Reinicia a aplicação
docker-setup.bat logs      # Exibe logs
docker-setup.bat status    # Mostra status dos serviços
docker-setup.bat clean     # Limpa containers e volumes
docker-setup.bat help      # Ajuda
```

### Opção 4: Docker Compose manual

1. Clone o repositório:

```bash
git clone https://github.com/seu-usuario/fastfood.git
cd fastfood
```

1. Execute os containers:

```bash
docker-compose up --build -d
```

1. Verifique se os serviços estão rodando:

```bash
docker-compose ps
```

### 🌐 URLs da Aplicação

A API estará disponível em:

- **HTTPS (Recomendado)**: <https://localhost:5001>
- **HTTP (Fallback)**: <http://localhost:5000>
- **Swagger UI**: <https://localhost:5001/swagger> ou <http://localhost:5000/swagger>
- **Health Check**: <https://localhost:5001/health> ou <http://localhost:5000/health>

> 🔒 **Recomendação**: Use sempre HTTPS (porta 5001) para seguir as melhores práticas de segurança.

## 🗄️ Banco de Dados

- **SGBD**: SQL Server 2022 (Express Edition em Docker)
- **Porta**: 1433
- **Volume Persistente**: `fastfood-data` (dados preservados entre restarts)
- **Inicialização**: Migrations automáticas via container init
- **Credenciais**: Configuradas via variáveis de ambiente (.env)

### 🔄 Sistema de Migrations

O projeto implementa um sistema robusto de migrations que garante que o banco de dados esteja sempre atualizado automaticamente.

#### Como Funciona

1. **Container de Inicialização**: Um container dedicado (`fastfood-migrations`) executa as migrations antes da API iniciar
1. **Sequência de Startup**: `db` (SQL Server) → `migrations` (Init Container) → `api` (FastFood API)
1. **Migrations Automáticas**: Todas as migrations pendentes são aplicadas automaticamente
1. **Verificação de Saúde**: Sistema verifica se o banco está pronto antes de aplicar migrations

#### Comando CLI

A aplicação suporta comandos CLI para operações de banco:

```bash
# Aplicar migrations manualmente
docker exec fastfood-api dotnet FastFood.Api.dll --migrate

# Verificar conexão com banco
docker exec fastfood-api dotnet FastFood.Api.dll --check-db
```

#### Scripts Manuais

Para situações especiais, existem scripts para executar migrations manualmente:

**Linux/WSL:**

```bash
./scripts/init-database.sh
```

**Windows:**

```cmd
.\scripts\init-database.bat
```

#### Logs de Migrations

Para acompanhar o processo de migrations:

```bash
# Logs do container de migrations
docker-compose logs migrations

# Logs da API (inclui resultado das migrations CLI)
docker-compose logs api

# Todos os logs em tempo real
docker-compose logs -f
```

#### Resolução de Problemas

Se houver problemas com migrations:

1. **Verificar status dos containers**:

```bash
docker-compose ps
```

1. **Verificar logs de erros**:

```bash
docker-compose logs migrations
```

1. **Executar migration manual**:

```bash
./scripts/init-database.sh  # Linux/WSL
.\scripts\init-database.bat # Windows
```

1. **Reset completo** (⚠️ **APAGA TODOS OS DADOS**):

```bash
docker-compose down -v  # Remove volumes
docker-compose up --build -d  # Recria tudo
```

## 🏗️ Arquitetura

O projeto segue os princípios do Domain-Driven Design (DDD) e Clean Architecture, organizado em camadas:

### Camadas

- **Domain**: Entidades, agregados, value objects e regras de negócio
- **Application**: Casos de uso, DTOs, comandos e queries (CQRS)
- **Infrastructure**: Implementações de persistência, serviços externos
- **API**: Controllers e configuração da API REST

### Padrões Implementados

- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Value Objects
- Domain Events
- Fluent Validation
- Middleware de Erros Global
- Testes Unitários

## 📝 API Endpoints

### Clientes

- `POST /api/v1/customers` - Cadastro de cliente
- `GET /api/v1/customers/cpf/{cpf}` - Busca cliente por CPF

### Produtos

- `POST /api/v1/products` - Cadastro de produto
- `PUT /api/v1/products/{id}` - Atualização de produto
- `DELETE /api/v1/products/{id}` - Remoção de produto
- `GET /api/v1/products/category/{category}` - Lista produtos por categoria

### Pedidos

- `POST /api/v1/orders` - Criação de pedido
- `GET /api/v1/orders` - Lista pedidos com filtros
- `GET /api/v1/orders/{id}` - Busca pedido por ID
- `PUT /api/v1/orders/{id}/status` - Atualiza status do pedido
- `POST /api/v1/orders/{id}/checkout` - Processa pagamento do pedido

## ⚙️ Desenvolvimento

Para desenvolver localmente:

1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
1. Restaure os pacotes:

```bash
dotnet restore
```

1. Execute os testes:

```bash
dotnet test
```

1. Execute a API (requer SQL Server local ou via Docker):

```bash
dotnet run --project src/FastFood.Api/FastFood.Api.csproj
```

## 🧪 Testes

O projeto inclui:

- Testes unitários
- Testes de integração
- Testes de API

Execute os testes com:

```bash
dotnet test
```

## 📚 Documentação

- Documentação da API via Swagger: <https://localhost:5001/swagger>
- Arquivos de definição da API em `/docs/api/`
- Collection do Postman em `/docs/postman/`

## 🔐 Segurança

Esta aplicação implementa várias camadas de segurança conforme as melhores práticas:

- **HTTPS por Padrão**: Certificados SSL/TLS configurados para comunicação segura
- **Certificados Seguros**: RSA 2048 bits para desenvolvimento, produção ready para CAs válidas
- **Ambientes Separados**: Configurações distintas para desenvolvimento e produção
- **Validação de Inputs**: FluentValidation para todas as entradas da API
- **Logging Estruturado**: Rastreamento completo de operações
- **Tratamento de Erros Global**: Middleware para captura e tratamento seguro de exceções
- **Dados Sensíveis Protegidos**: Variáveis de ambiente para credenciais
- **Health Checks Seguros**: Monitoramento via HTTPS

Para mais detalhes sobre a configuração de segurança, consulte a seção [🔐 Configuração HTTPS](#-configuração-https) acima.

## 📜 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.
