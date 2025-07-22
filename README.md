# 🍔 FastFood API - Tech Challenge Fase 1

Sistema de gerenciamento de pedidos para lanchonete com arquitetura Clean Architecture/Hexagonal Architecture, desenvolvido em .NET 8.

## 📚 Documentação do Projeto

- **Vídeo da Arquitetura**: [Assistir no YouTube](https://www.youtube.com/watch?v=DjWIczeDQyg)
- **Miro Board**: [Acessar Documentação Completa](https://miro.com/app/board/uXjVIFgMg1M=/)
- **Dicionário de Termos**: [Acessar Dicionário de Termos](Dicionario Projeto/Dicionário.pdf) 

A documentação inclui:
- Event Storming dos fluxos de negócio
- Diagramas de Domínio (DDD)
- Arquitetura da Solução
- Detalhes da Implementação


## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Pré-requisitos](#pré-requisitos)
- [Instalação e Execução](#instalação-e-execução)
- [Funcionalidades](#funcionalidades)
- [Documentação da API](#documentação-da-api)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Configurações](#configurações)
- [Scripts Utilitários](#scripts-utilitários)
- [Troubleshooting](#troubleshooting)

## 🎯 Sobre o Projeto

**Inicio dadocumentação relacionada na segunda fase** ------------------

### 📌 Problema 
Uma lanchonete de bairro está em processo de expansão, mas enfrenta dificuldades no atendimento devido à ausência de um sistema de controle de pedidos. A comunicação entre atendentes e cozinha é falha, ocasionando erros, atrasos e insatisfação dos clientes.

### ✅ Solução
O projeto propõe o desenvolvimento de um sistema de autoatendimento estilo fast food, onde os próprios clientes possam montar seus pedidos, pagar via QRCode (Mercado Pago) e acompanhar o progresso do pedido em tempo real, além de permitir ao estabelecimento:

- Gerenciar pedidos, produtos e categorias.
- Acompanhar o preparo.
- Identificar clientes para futuras campanhas.
- Ter controle total via painel administrativo.

-------------------

Sistema completo de gestão de pedidos para lanchonetes que permite:

- **Gestão de Clientes**: Cadastro e consulta com validação de CPF
- **Catálogo de Produtos**: Criação e gerenciamento de produtos por categoria
- **Sistema de Pedidos**: Fluxo completo desde criação até entrega
- **Pagamentos**: Integração com MercadoPago via QR Code
- **Pedidos Anônimos**: Suporte para clientes não cadastrados
- **Acompanhamento**: Status em tempo real dos pedidos

## 🛠 Tecnologias

### Backend
- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server 2022** - Banco de dados
- **MediatR** - Implementação de CQRS
- **AutoMapper** - Mapeamento de objetos
- **FluentValidation** - Validação de dados
- **JWT Bearer** - Autenticação e autorização
- **Swagger/OpenAPI** - Documentação da API

### DevOps e Infraestrutura
- **Docker & Docker Compose** - Containerização
- **Health Checks** - Monitoramento de saúde
- **Migrations** - Versionamento do banco de dados

### Integrações
- **MercadoPago API** - Processamento de pagamentos
- **QR Code** - Geração para pagamentos

## 🏗 Arquitetura

Projeto segue os princípios da **Clean Architecture** e **Hexagonal Architecture**, separando as camadas de apresentação, aplicação, domínio e infraestrutura. A estrutura do projeto é organizada da seguinte forma:

```
📁 src/
├── 🔷 FastFood.Api/          # Camada de Apresentação
│   ├── Controllers/          # Endpoints da API
│   ├── Middlewares/         # Middleware customizados
│   └── Extensions/          # Extensões e configurações
├── 🔶 FastFood.Application/ # Camada de Aplicação
│   ├── Commands/           # Comandos (Write)
│   ├── Queries/           # Consultas (Read)
│   ├── DTOs/              # Objetos de transferência
│   └── Handlers/          # Manipuladores CQRS
├── 🔸 FastFood.Domain/     # Camada de Domínio
│   ├── Entities/          # Entidades de negócio
│   ├── Repositories/      # Contratos de repositório
│   └── Services/          # Serviços de domínio
└── 🔹 FastFood.Infrastructure/ # Camada de Infraestrutura
    ├── Data/              # Contexto EF e Configurações
    ├── Repositories/      # Implementações de repositório
    └── Services/          # Serviços externos
```

## 📋 Pré-requisitos

### Para execução via Docker (Recomendado)
- **Docker Desktop** instalado e rodando
- **Git** para clonar o repositório
- **Windows 10/11** ou **WSL2**

### Para execução manual
- **.NET 8 SDK** instalado
- **SQL Server** (pode ser via Docker)
- **Visual Studio 2022** ou **VS Code**

## 🚀 Instalação e Execução

### 📦 Opção 1: Execução Automática (Docker)

**Windows (Recomendado):**
```cmd
# Clone o repositório
git clone <repository-url>
cd techchallenge_fase1

# Execute o script de setup automático
docker-setup.bat
```

**WSL2/Ubuntu:**
```bash
# Clone o repositório
git clone <repository-url>
cd techchallenge_fase1

# Torne os scripts executáveis
chmod +x migrate.sh init-db.sh scripts/*.sh

# Execute o setup
./scripts/init-database.sh
docker-compose up -d
```

### 🔧 Opção 2: Execução Manual

**1. Configurar banco de dados:**
```cmd
# Inicie apenas o SQL Server
docker-compose up -d db

# Aguarde o banco ficar pronto (30 segundos)
timeout /t 30
```

**2. Executar migrations:**
```cmd
cd src\FastFood.Api
dotnet ef database update
```

**3. Executar a aplicação:**
```cmd
dotnet run --project src\FastFood.Api\FastFood.Api.csproj
```

### 🌐 Acessar a aplicação

Após a execução bem-sucedida:

- **API Base**: http://localhost:5000
- **API HTTPS**: https://localhost:5001  
- **Swagger**: https://localhost:5001/swagger
- **Health Check**: http://localhost:5000/health

## ✨ Funcionalidades

### 👥 Gestão de Clientes
- **Cadastro**: Nome, email e CPF com validação
- **Consulta**: Busca por CPF (endpoint público)
- **Listagem**: Todos os clientes (protegido)
- **Validação**: Algoritmo oficial de CPF brasileiro

### 🛍 Catálogo de Produtos
- **Categorias**: Lanche, Acompanhamento, Bebida, Sobremesa
- **Gestão**: CRUD completo de produtos
- **Consulta**: Filtro por categoria (endpoint público)
- **Imagens**: Suporte a múltiplas imagens por produto

### 📋 Sistema de Pedidos

**Fluxo completo:**
1. **Criar Pedido** → Status: `Pending`
2. **Gerar QR Code** → Status: `AwaitingPayment`
3. **Confirmar Pagamento** → Status: `Paid`
4. **Processar** → Status: `Processing`
5. **Finalizar** → Status: `Ready` → `Completed`

**Status disponíveis:**
- `Pending` - Pedido criado, aguardando checkout
- `AwaitingPayment` - QR Code gerado, aguardando pagamento
- `Paid` - Pagamento confirmado
- `Processing` - Em preparo na cozinha
- `Ready` - Pronto para retirada
- `Completed` - Entregue ao cliente
- `Cancelled` - Cancelado

### 💳 Sistema de Pagamentos

**MercadoPago Integration:**
- Geração de QR Code para pagamento
- Webhook para confirmação automática
- Suporte a pagamento PIX, cartão, etc.
- Modo fake para desenvolvimento

**Endpoints de pagamento:**
- `POST /api/v1/orders/{id}/checkout` - Gerar QR Code
- `POST /api/v1/orders/{id}/confirm-payment` - Confirmar pagamento
- `POST /api/webhook/mercadopago` - Webhook do MercadoPago

### 🔒 Autenticação e Autorização

**JWT Authentication:**
- Login com credenciais
- Token com expiração configurável
- Middleware de autorização
- Endpoints públicos e protegidos

## 📚 Documentação da API

### Swagger/OpenAPI
Acesse: http://localhost:5000/swagger

### Documentação Detalhada
- [API de Clientes](docs/api/customers.md)
- [API de Produtos](docs/api/products.md)
- [API de Pedidos](docs/api/orders.md)
- [Configuração MercadoPago](docs/MERCADOPAGO_SETUP.md)
- [Algoritmo de Validação CPF](docs/cpf-validation-algorithm.md)

### Exemplos de Uso

**Criar um pedido anônimo:**
```http
POST /api/v1/orders
Content-Type: application/json

{
  "customerId": null,
  "items": [
    {
      "productId": "uuid-do-produto",
      "quantity": 2
    }
  ]
}
```

**Consultar status do pedido (público):**
```http
GET /api/v1/orders/{orderId}/status
```

**Gerar QR Code para pagamento:**
```http
POST /api/v1/orders/{orderId}/checkout
```

## 📁 Estrutura do Projeto

```
techchallenge_fase1/
├── 📄 README.md                    # Documentação principal
├── 🐳 docker-compose.yml           # Orquestração de containers
├── 🐳 Dockerfile                   # Build da aplicação
├── 🔧 docker-setup.bat            # Setup automático Windows
├── 📋 FastFood.sln                 # Solution .NET
├── 📁 docs/                        # Documentação técnica
│   ├── api/                        # Documentação das APIs
│   ├── MERCADOPAGO_SETUP.md       # Setup do MercadoPago
│   └── migrations.md               # Guia de migrations
├── 📁 init/                        # Scripts de inicialização
│   ├── check-prereq.bat           # Verificação de pré-requisitos
│   ├── setup-env.bat              # Configuração de ambiente
│   └── docker-init.bat            # Inicialização Docker
├── 📁 scripts/                     # Scripts utilitários
│   ├── generate-dev-certs.bat     # Geração de certificados
│   └── init-database.sh           # Inicialização do banco
└── 📁 src/                         # Código fonte
    ├── FastFood.Api/               # API REST
    ├── FastFood.Application/       # Lógica de aplicação
    ├── FastFood.Domain/            # Regras de negócio
    └── FastFood.Infrastructure/    # Acesso a dados
```

## ⚙️ Configurações

### Banco de Dados
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1434;Database=FastFood;User Id=sa;Password=FastFood2025;TrustServerCertificate=true"
  }
}
```

### JWT
```json
{
  "Jwt": {
    "Secret": "SenhaSuper@Segura123ParaTokenJwt-FastFood2025",
    "Issuer": "fastfood-api",
    "Audience": "fastfood-clients",
    "ExpirationMinutes": 60
  }
}
```

### MercadoPago
```json
{
  "MercadoPago": {
    "AccessToken": "TEST-xxx",
    "PublicKey": "TEST-xxx",
    "Environment": "sandbox"
  },
  "UseFakePayment": true
}
```

## 🛠 Scripts Utilitários

### Windows (.bat)
- `docker-setup.bat` - Setup completo automatizado
- `init/check-prereq.bat` - Verificar pré-requisitos
- `init/setup-env.bat` - Configurar ambiente
- `scripts/generate-dev-certs.bat` - Gerar certificados SSL

### Linux/WSL (.sh)
- `migrate.sh` - Executar migrations
- `init-db.sh` - Inicializar banco de dados
- `scripts/init-database.sh` - Setup do banco completo

### Docker Commands
```bash
# Ver logs da aplicação
docker-compose logs -f api

# Reiniciar apenas a API
docker-compose restart api

# Ver status dos containers
docker-compose ps

# Parar todos os containers
docker-compose down

# Rebuild completo
docker-compose down && docker-compose build --no-cache && docker-compose up -d
```

## 🔧 Troubleshooting

### Problemas Comuns

**1. Docker não está rodando**
```
Solução: Inicie o Docker Desktop e aguarde carregar completamente
```

**2. Porta 1434 ocupada**
```bash
# Windows
netstat -ano | findstr :1434
taskkill /PID <PID> /F

# Linux
sudo lsof -i :1434
sudo kill -9 <PID>
```

**3. Certificado SSL inválido**
```bash
# Regenerar certificados
cd scripts
./generate-dev-certs.bat  # Windows
./generate-dev-certs.sh   # Linux
```

**4. Banco de dados não conecta**
```bash
# Verificar se o SQL Server está rodando
docker-compose logs db

# Testar conexão manual
docker exec -it fastfood-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P FastFood2025 -C
```

**5. Migrations falham**
```bash
# Resetar banco e recriar
docker-compose down -v
docker-compose up -d db
# Aguardar 30 segundos
dotnet ef database update --project src/FastFood.Api
```

### Logs e Diagnóstico

**Ver logs específicos:**
```bash
# Logs da API
docker-compose logs -f api

# Logs do banco
docker-compose logs -f db

# Logs das migrations
docker-compose logs migrations
```

**Health Checks:**
- API Health: http://localhost:5000/health
- Database Health: Incluído no health check da API

### Reinicialização Completa

**Windows:**
```cmd
# Parar tudo e limpar
docker-compose down -v
docker system prune -f

# Reexecutar setup
docker-setup.bat
```

**Linux/WSL:**
```bash
# Parar tudo e limpar
docker-compose down -v
docker system prune -f

# Reexecutar setup
./scripts/init-database.sh
docker-compose up -d
```

---

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

**Desenvolvido para o Tech Challenge - Fase 1** 🚀
