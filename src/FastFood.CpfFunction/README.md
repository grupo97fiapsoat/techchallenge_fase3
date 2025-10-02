# FastFood CPF Function

Esta é uma AWS Lambda Function para identificar clientes por CPF, consumindo a mesma infraestrutura da API principal.

## Funcionalidade

- **Endpoint**: `GET /identify?cpf=12345678900`
- **Validação**: CPF deve conter 11 dígitos numéricos
- **Respostas**:
  - `200 OK`: Cliente encontrado (JSON com dados do cliente)
  - `400 Bad Request`: CPF inválido ou não fornecido
  - `404 Not Found`: Cliente não encontrado
  - `500 Internal Server Error`: Erro interno

## Como executar localmente

### Pré-requisitos

1. .NET 8 SDK instalado
2. Banco de dados SQL Server rodando (mesmo da API)
3. Migrations aplicadas

### Execução

#### Opção 1: AWS Lambda Local (Recomendado)

```bash
# Compilar o projeto
dotnet build

# Executar localmente
dotnet lambda local-run --function-handler FastFood.CpfFunction::FastFood.CpfFunction.Function::IdentifyByCpf
```

#### Opção 2: Teste direto com dotnet run

```bash
# Navegar para o diretório da function
cd src/FastFood.CpfFunction

# Executar
dotnet run
```

### Configuração

A function usa as mesmas configurações da API:

- **ConnectionString**: Via variável de ambiente `ConnectionStrings__DefaultConnection` ou appsettings.json
- **Logging**: Configurado para console

### Variáveis de Ambiente (Produção)

```bash
# Windows
set ConnectionStrings__DefaultConnection="Server=seu-servidor;Database=FastFood;..."

# Linux/Mac
export ConnectionStrings__DefaultConnection="Server=seu-servidor;Database=FastFood;..."
```

### Exemplo de Uso

```bash
# Teste local
curl "http://localhost:3000/identify?cpf=12345678900"

# Resposta de sucesso
{
  "id": "guid-do-cliente",
  "name": "Nome do Cliente",
  "email": "cliente@email.com",
  "cpf": "12345678900",
  "createdAt": "2025-01-01T00:00:00Z",
  "updatedAt": "2025-01-01T00:00:00Z"
}
```

### Deploy para AWS

```bash
# Empacotar
dotnet lambda package --configuration Release

# Deploy (requer AWS CLI configurado)
dotnet lambda deploy-function FastFoodCpfFunction --function-handler FastFood.CpfFunction::FastFood.CpfFunction.Function::IdentifyByCpf
```

## Arquitetura

A function reutiliza:
- **FastFood.Application**: Queries e handlers
- **FastFood.Infrastructure**: DbContext e repositórios
- **MediatR**: Para processamento de queries
- **Entity Framework**: Para acesso ao banco de dados

Isso garante consistência com a API principal e facilita manutenção.