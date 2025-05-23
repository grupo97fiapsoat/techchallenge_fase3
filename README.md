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
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) (opcional, apenas para desenvolvimento)

## 🛠️ Configuração

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto com as seguintes variáveis:

```env
DB_PASSWORD=YourStrongPassword123!
```

### Executando com Docker

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/fastfood.git
cd fastfood
```

2. Execute os containers:
```bash
docker-compose up -d
```

3. Verifique se os serviços estão rodando:
```bash
docker-compose ps
```

A API estará disponível em:
- http://localhost:5000 - HTTP
- https://localhost:5001 - HTTPS
- Swagger UI: http://localhost:5000/swagger

## 🏗️ Arquitetura

O projeto segue os princípios do Domain-Driven Design e Clean Architecture, organizado em camadas:

- **FastFood.Domain**: Entidades, agregados, value objects e regras de negócio
- **FastFood.Application**: Casos de uso, DTOs, comandos e queries (CQRS)
- **FastFood.Infrastructure**: Implementações de persistência, serviços externos
- **FastFood.Api**: Controllers e configuração da API

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

## 🗄️ Banco de Dados

- SQL Server 2022
- Migrations automáticas na inicialização
- Porta: 1433
- Volume persistente: fastfood-data

## ⚙️ Desenvolvimento

Para desenvolver localmente:

1. Instale o [.NET 7 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
2. Restaure os pacotes:
```bash
dotnet restore
```

3. Execute os testes:
```bash
dotnet test
```

4. Execute a API (requer SQL Server local ou via Docker):
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

- Documentação da API via Swagger: http://localhost:5000/swagger
- Arquivos de definição da API em `/docs/api/`
- Collection do Postman em `/docs/postman/`

## 🔐 Segurança

- HTTPS habilitado
- Validação de inputs
- Logging estruturado
- Tratamento de erros global
- Dados sensíveis em variáveis de ambiente

## 📜 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
- **Infrastructure**: 
  - Repositories (implementação)
  - Banco de Dados
  - Serviços Externos

### Padrões
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Value Objects
- Domain Events
- Fluent Validation
- Middleware de Erros Global
- Testes Unitários
