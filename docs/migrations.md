# Guia de Migrações do Banco de Dados - FastFood API

Este documento fornece instruções sobre como executar e gerenciar migrações do banco de dados para a FastFood API.

## Introdução

A aplicação FastFood API utiliza o Entity Framework Core para gerenciar o banco de dados SQL Server. O código já contém uma migração inicial que cria as tabelas necessárias para o funcionamento da aplicação.

## Pré-requisitos

- .NET SDK 8.0 ou superior
- Entity Framework Core CLI tools (`dotnet-ef`)
- SQL Server (local ou Docker)

## Instalação da Ferramenta dotnet-ef

Se você ainda não tem a ferramenta dotnet-ef instalada, execute o seguinte comando:

```
dotnet tool install --global dotnet-ef
```

## Opções para Executar Migrações

### 1. Usando Docker Compose

A maneira mais simples de executar as migrações é usando Docker Compose, que já está configurado no projeto:

```
docker-compose up -d db migrations
```

Este comando iniciará o banco de dados SQL Server e executará as migrações automaticamente.

### 2. Usando o Script run-migrations.bat (Windows)

Para executar as migrações diretamente no Windows, sem Docker:

1. Certifique-se de que o SQL Server está em execução e acessível
2. Atualize a string de conexão no arquivo `appsettings.json` do projeto FastFood.Api
3. Execute o script `run-migrations.bat` na raiz do projeto

### 3. Manualmente com o CLI do EF Core

Para executar as migrações manualmente usando o CLI do EF Core:

1. Navegue até o diretório do projeto FastFood.Api:
   ```
   cd src/FastFood.Api
   ```

2. Execute o comando de atualização do banco de dados:
   ```
   dotnet ef database update
   ```

## Criação de Novas Migrações

Se você fizer alterações nos modelos de domínio que exigem mudanças no banco de dados, precisará criar uma nova migração:

1. Navegue até o diretório do projeto FastFood.Api:
   ```
   cd src/FastFood.Api
   ```

2. Execute o comando para adicionar uma nova migração:
   ```
   dotnet ef migrations add NomeDaMigração
   ```

3. Execute a migração:
   ```
   dotnet ef database update
   ```

## Solução de Problemas

### Erros de Conexão com o Banco de Dados

- Verifique se o SQL Server está em execução
- Verifique se a string de conexão está correta no arquivo `appsettings.json`
- Se estiver usando Docker, verifique se o contêiner do SQL Server está em execução

### Erros durante a Execução da Migração

- Verifique se você tem permissões suficientes no banco de dados
- Execute `dotnet ef migrations script` para gerar o script SQL e verificar se há problemas
- Se os modelos foram alterados significativamente, pode ser necessário remover a migração existente e criar uma nova

## Estrutura do Banco de Dados

A migração inicial cria as seguintes tabelas:

- `Customers`: Armazena dados dos clientes
- `Products`: Armazena informações de produtos
- `Orders`: Armazena pedidos
- `OrderItems`: Armazena itens de pedidos

## Observações Adicionais

- A aplicação está configurada para aplicar migrações pendentes automaticamente durante o startup em ambiente de desenvolvimento
- No ambiente de produção, é recomendável executar as migrações como um passo separado antes da implantação da aplicação
