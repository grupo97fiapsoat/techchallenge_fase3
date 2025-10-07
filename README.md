# 🍔 FastFood API - Tech Challenge Fase 3

Sistema de gerenciamento de pedidos para lanchonete com arquitetura Clean Architecture/Hexagonal Architecture, desenvolvido em .NET 8.

Este projeto faz parte de um desafio de pós-graduação e contém a infraestrutura necessária para subir uma aplicação .NET API com SQL Server em um cluster Kubernetes local (via Minikube), utilizando Terraform.

📁 Os arquivos de configuração do Kubernetes (hpa, service, secrets, deployment, main, etc.) estão localizados na pasta infra, sendo responsáveis por orquestrar os recursos necessários para o funcionamento da aplicação.

## 📚 Documentação do Projeto

- **Vídeo da Arquitetura fase 3**: [Assistir no YouTube](https://youtu.be/z8k7jxeFkEU?si=dbMU-sliQFxxxsJl)
- **Miro Board DDD Primeira Fase**: [Miro](https://miro.com/app/board/uXjVIFgMg1M=/)
- **Justificativa Escolha do banco e Diagrama Terceira Fase**:[Ir para](#-diagrama-banco-de-dados)
- **Dicionário de Termos Primeira Fase**:
      - [FastFood-Lambda](https://github.com/grupo97fiapsoat/fastfood-lambda)
      - [FastFood-Banco-RDS](https://github.com/grupo97fiapsoat/infra-rds)
      - [FastFood-Infra-k8s](https://github.com/grupo97fiapsoat/infra-k8s)

A documentação inclui:
- Event Storming dos fluxos de negócio
- Diagramas de Domínio (DDD)
- Arquitetura da Solução
- Detalhes da Implementação
- Justificativa escolha do Banco de dados *FASE 3*
- Diagrama de Branco


## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
  - [📌 Requisitos do Negócio](#-requisitos-do-negócio)
  - [🧩 Problema](#-problema)
  - [✅ Solução Proposta](#-solução-proposta)
  - [🧱 Requisitos de Infraestrutura](#-requisitos-de-infraestrutura)
  - [🧭 Fluxo do Sistema](#-fluxo-do-sistema)
  - [👤 Fluxo de ADM](#-fluxo-de-adm)
  - [🧱 Diagrama da Infraestrutura](#-diagrama-da-infraestrutura)
  - [🧱Diagrama do banco de Dados](#-diagrama-banco-de-dados)
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

### 📌 Os Requisitos do Negócio
A lanchonete em questão está passando por um processo de expansão devido ao seu grande sucesso, mas enfrenta sérios desafios operacionais pela ausência de um sistema informatizado. Atualmente, os pedidos são anotados manualmente, o que gera diversos problemas como:

- Erros na comunicação entre atendentes e cozinha; 
- Atrasos na entrega dos pedidos; 
- Perda ou esquecimento de pedidos; 
- Clientes insatisfeitos, o que compromete a fidelização e a reputação do negócio. 

#### 🧩 Problema 
Uma lanchonete de bairro está em processo de expansão, mas enfrenta dificuldades no atendimento devido à ausência de um sistema de controle de pedidos. A comunicação entre atendentes e cozinha é falha, ocasionando erros, atrasos e insatisfação dos clientes.

#### ✅ Solução Proposta
O sistema desenvolvido será um autoatendimento de fast food, permitindo que os próprios clientes realizem seus pedidos de forma autônoma, com as seguintes funcionalidades:

- Identificação do cliente (CPF, cadastro ou anônimo); 
- Montagem personalizada de combos (lanche, acompanhamento, bebida); 
- Pagamento via QRCode (Mercado Pago); 
- Acompanhamento em tempo real do pedido (Recebido → Em preparação → Pronto → Finalizado); 
- Notificações para retirada do pedido. 

### Os requisitos de infraestrutura:

Sistema completo de gestão de pedidos para lanchonetes que permite:

- **Gestão de Clientes**: Cadastro e consulta com validação de CPF
- **Catálogo de Produtos**: Criação e gerenciamento de produtos por categoria
- **Sistema de Pedidos**: Fluxo completo desde criação até entrega
- **Pagamentos**: Integração com MercadoPago via QR Code
- **Pedidos Anônimos**: Suporte para clientes não cadastrados
- **Acompanhamento**: Status em tempo real dos pedidos

## 🧭 Fluxo do Sistema
Este diagrama representa o fluxo completo do cliente, desde a identificação até o pagamento e acompanhamento do pedido:

<img width="626" height="1067" alt="image" src="https://github.com/user-attachments/assets/e80575ca-a52e-4353-8804-120081c852df" />

## 👤 Fluxo de ADM
Este diagrama representa o fluxo administrativo, para consulta de clientes e cadstro de novos itens:

<img width="622" height="976" alt="image" src="https://github.com/user-attachments/assets/89b4514e-5aa4-4541-af27-c89533f000cb" />

## 🧱 Diagrama da Infraestrutura
Este diagrama representa a infraestrutura do projeto:

<img width="699" height="589" alt="image" src="https://github.com/user-attachments/assets/b813b312-6b99-4b70-86fd-aeaa3d9ce48f" />

## 🧱 Diagrama Bando de Dados
Este diagrama representa o banco de Dados:

<img width="1370" height="751" src="https://github.com/user-attachments/assets/fd6a4d97-0f15-40e6-b2b0-dd094d670a3b" />


### 🗃️ Justificativa da Escolha do SQL Server

Optamos pelo SQL Server como banco de dados relacional do sistema por sua integração nativa com o .NET, o que simplifica o desenvolvimento e aumenta a produtividade com o Entity Framework Core e drivers oficiais da Microsoft.
Além de atender bem às necessidades atuais do sistema — como cadastros de clientes, produtos e pedidos —, o SQL Server garante consistência transacional e uma estrutura sólida para evolução futura.
Pensando em escalabilidade, ele suporta recursos essenciais para módulos futuros, como controle de estoque, registros financeiros e relatórios analíticos.
Na nuvem, pode ser utilizado via Amazon RDS for SQL Server ou Azure SQL Database, oferecendo backup automatizado, alta disponibilidade, escalabilidade e criptografia.
Por fim, o SQL Server se destaca pela segurança e conformidade, com suporte a TDE, Always Encrypted e mascaramento dinâmico de dados, essenciais para proteger informações sensíveis como CPF.

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
- Minikube – Cluster Kubernetes local para testes
- Kubernetes (k8s) – Orquestração de containers
- Terraform – Provisionamento de infraestrutura como código (IaC)

### Integrações
- **MercadoPago API** - Processamento de pagamentos
- **QR Code** - Geração para pagamentos

## 🏗 Arquitetura

Projeto segue os princípios da **Clean Architecture**, separando as camadas de aplicação, domínio e infraestrutura. A estrutura do projeto é organizada da seguinte forma:

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

Antes de começar, tenha os seguintes softwares instalados na sua máquina:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (com Kubernetes habilitado)
- Conta na AWS com permissões para EKS, RDS, Lambda e API Gateway
- [Terraform](https://developer.hashicorp.com/terraform)
- [kubectl](https://kubernetes.io/docs/tasks/tools/)
- [AWS CLI para Windows](https://docs.aws.amazon.com/pt_br/cli/latest/userguide/getting-started-install.html)


### Para execução via Docker (Recomendado)
- **Docker Desktop** instalado e rodando
- **Git** para clonar o repositório
- **Windows 10/11** ou **WSL2**

### Para execução manual
- **.NET 8 SDK** instalado
- **SQL Server** (pode ser via Docker)
- **Visual Studio 2022** ou **VS Code**

## 🚀 Execução e Demonstração – Fase 3 (Cloud)

### 📁 Etapa 1 – Clonar o projeto

```bash
git clone https://github.com/seu-usuario/grupo97fiapsoat
cd fastfood-api
```

### ✅ Etapa 2 – Configurar as credenciais da AWS
```bash
aws configure
```
E preencher:
AWS Access Key ID [None]: <sua-access-key>
AWS Secret Access Key [None]: <sua-secret-key>
Default region name [None]: sa-east-1
Default output format [None]: json

### 🌐 Etapa 3 – Acessar a API no EKS

#### 1. Acessar o painel do EKS na AWS

- Vá para [https://console.aws.amazon.com/eks](https://console.aws.amazon.com/eks)
- Selecione a região correta (ex: us-east-1 ou sa-east-1)
- Clique no nome do cluster criado (ex: `meu-eks`)

#### 2. Verificar se o cluster está ativo

- No painel do cluster, verifique se o status está como `ACTIVE`
- Confirme que os nós estão disponíveis na aba **Compute → Node Groups**

#### 3. Verificar os pods rodando via terminal

Certifique-se de que o `kubectl` está configurado para o cluster:

```bash
aws eks --region sa-east-1 update-kubeconfig --name meu-eks
```
para consuktar o nome do cluster: aws eks list-clusters --region sa-east-1

#### 4. Verifique os pods:
```bash
kubectl get pods --namespace=default
#Você deve ver algo como:
#NAME                                  READY   STATUS    RESTARTS   AGE
#fastfood-api-deployment-xxxxx         1/1     Running   0          5m
```
### 🌐 Etapa 4 – Acessar o endpoint público da API
- Vá para EC2 → Load Balancers
- Localize o Load Balancer criado pelo serviço
- Copie o DNS público
- Acesse no navegador:

http://a5b4751deaacc4376b4572bb470a3f1a-910775930.sa-east-1.elb.amazonaws.com/swagger/index.html

###🗄️ Etapa 5 – Validar o banco RDS
- Acesse o painel do RDS na AWS 
- Confirme que a instância está com status "available" 
Mostre:
 - Endpoint do banco 
 - Engine (PostgreSQL) 
 - Configurações básicas 


### 🌐 Acessar a aplicação 
Isso abrirá automaticamente o navegador com o endereço do serviço.
A URL normalmente é algo como: http://127.0.0.1:<porta>/swagger/index.html

### 📦 Opção 2: Execução Local com Docker Compose (Ambiente de Desenvolvimento)

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

### Fase 3 - Autenticação Externa + Function CPF

**JWT Authentication com IdP Externo:**
- **Produção**: Aceita JWT de IdP externo (Cognito/Google/Azure AD)
- **Desenvolvimento**: Sistema local de autenticação (DEV ONLY)
- Token com expiração configurável
- Middleware de autorização
- Endpoints públicos e protegidos
- Política `AdminOnly` para roles administrativas

**Nova Function Serverless:**
- **IdentifyByCPF**: AWS Lambda Function para consultar cliente por CPF
- Consome a mesma infraestrutura da API (DbContext, repositórios)
- Endpoint: `GET /identify?cpf=12345678900`
- Respostas: 200 (cliente encontrado), 400 (CPF inválido), 404 (não encontrado)

### Configuração de Desenvolvimento

**Variáveis de Ambiente:**
```bash
# Windows
set Auth__Authority=http://localhost/dev-issuer
set Auth__Audience=dev-client-id

# Linux/Mac
export Auth__Authority=http://localhost/dev-issuer
export Auth__Audience=dev-client-id
```

**Como rodar a API:**
```bash
dotnet run --project src/FastFood.Api
```

**Como rodar a Function:**
```bash
# AWS Lambda local
dotnet lambda local-run --function-handler FastFood.CpfFunction::FastFood.CpfFunction.Function::IdentifyByCpf

# Ou teste direto
cd src/FastFood.CpfFunction
dotnet run
```

**Exemplos de uso:**
```bash
# API com token de IdP externo
curl -H "Authorization: Bearer SEU_TOKEN_IDP" https://localhost:5001/api/v1/customers

# Function para identificar cliente
curl "http://localhost:3000/identify?cpf=12345678900"
```

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

**Desenvolvido para o Tech Challenge - Fase 2** 🚀
