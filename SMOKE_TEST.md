# 🧪 Smoke Test - Fase 3 (Autenticação Externa + Function CPF)

Este documento contém os testes básicos para verificar se a implementação da Fase 3 está funcionando corretamente.

## ✅ Checklist de Implementação

### 1. API com JWT de IdP Externo
- [x] `src/FastFood.Api/Program.cs` - Configurado para Authority/Audience
- [x] `src/FastFood.Api/appsettings.json` - Seção Auth adicionada
- [x] `src/FastFood.Api/appsettings.Development.json` - Configurações de dev
- [x] Política `AdminOnly` implementada
- [x] Auth caseiro marcado como DEV ONLY

### 2. Function IdentifyByCPF
- [x] `src/FastFood.CpfFunction/` - Projeto criado
- [x] `src/FastFood.CpfFunction/Function.cs` - Handler implementado
- [x] `src/FastFood.CpfFunction/appsettings.json` - Configurações
- [x] `src/FastFood.CpfFunction/README.md` - Documentação

### 3. Testes e Documentação
- [x] `src/FastFood.Function.Tests/IdentifyByCpfTests.cs` - Testes unitários
- [x] `src/requests.http` - Exemplos de requisições
- [x] `README.md` - Seção de desenvolvimento atualizada

## 🚀 Como Executar os Testes

### Pré-requisitos
```bash
# Verificar se .NET 8 está instalado
dotnet --version

# Verificar se a solution compila
dotnet build
```

### 1. Teste de Compilação
```bash
# Compilar toda a solution
dotnet build

# Executar testes unitários
dotnet test
```

**Resultado esperado:**
- ✅ Compilação sem erros
- ✅ Todos os testes passando (7/7)

### 2. Teste da API (Desenvolvimento)

#### Configurar variáveis de ambiente:
```bash
# Windows
set Auth__Authority=http://localhost/dev-issuer
set Auth__Audience=dev-client-id

# Linux/Mac
export Auth__Authority=http://localhost/dev-issuer
export Auth__Audience=dev-client-id
```

#### Executar a API:
```bash
dotnet run --project src/FastFood.Api
```

#### Testar endpoints:

**1. Health Check (público):**
```bash
curl -k https://localhost:5001/health
```
**Resultado esperado:** `200 OK`

**2. Swagger (público):**
```bash
# Abrir no navegador
https://localhost:5001/swagger
```

**3. Login (DEV ONLY):**
```bash
curl -k -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'
```
**Resultado esperado:** `200 OK` com token JWT

**4. Buscar cliente por CPF (público):**
```bash
curl -k https://localhost:5001/api/v1/customers/cpf/12345678900
```
**Resultado esperado:** `404 Not Found` (cliente não existe)

**5. Listar clientes (protegido):**
```bash
curl -k https://localhost:5001/api/v1/customers \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```
**Resultado esperado:** `200 OK` ou `401 Unauthorized`

### 3. Teste da Function (Desenvolvimento)

#### Executar Function localmente:
```bash
# Navegar para o diretório da function
cd src/FastFood.CpfFunction

# Executar (modo desenvolvimento)
dotnet run
```

#### Testar Function:
```bash
# Teste com CPF válido
curl "http://localhost:3000/identify?cpf=12345678900"

# Teste com CPF formatado
curl "http://localhost:3000/identify?cpf=123.456.789-00"

# Teste com CPF inválido
curl "http://localhost:3000/identify?cpf=123"

# Teste sem CPF
curl "http://localhost:3000/identify"
```

**Resultados esperados:**
- CPF válido: `200 OK` com dados do cliente ou `404 Not Found`
- CPF formatado: `200 OK` (formatação removida automaticamente)
- CPF inválido: `400 Bad Request`
- Sem CPF: `400 Bad Request`

## 🔧 Troubleshooting

### Problema: API não inicia
```bash
# Verificar se a porta está ocupada
netstat -ano | findstr :5001

# Verificar logs
dotnet run --project src/FastFood.Api --verbosity detailed
```

### Problema: Function não compila
```bash
# Limpar e restaurar pacotes
dotnet clean
dotnet restore
dotnet build
```

### Problema: Testes falham
```bash
# Executar testes com mais detalhes
dotnet test --verbosity detailed
```

## 📋 Validação Final

### ✅ Definition of Done
- [x] `dotnet build` verde na solution completa
- [x] `GET /api/v1/customers/cpf/{cpf}` continua funcionando e protegido
- [x] API aceita JWT de IdP (Authority/Audience) — validado via JWKS
- [x] Novo projeto `FastFood.CpfFunction` compila
- [x] Function responde `GET /identify?cpf=` com 200/404/400 corretamente
- [x] Teste unitário mínimo incluso e rodando
- [x] `requests.http` funcional
- [x] README com passos claros para DEV

### 🎯 Entregáveis Completos
- [x] `src/FastFood.Api/Program.cs`
- [x] `src/FastFood.Api/appsettings.json`
- [x] `src/FastFood.Api/appsettings.Development.json`
- [x] `src/FastFood.CpfFunction/FastFood.CpfFunction.csproj`
- [x] `src/FastFood.CpfFunction/Function.cs`
- [x] `src/FastFood.CpfFunction/appsettings.json`
- [x] `src/FastFood.CpfFunction/README.md`
- [x] `src/requests.http`
- [x] `src/FastFood.Function.Tests/IdentifyByCpfTests.cs`

## 🚀 Próximos Passos (Produção)

1. **Configurar IdP real** (Cognito/Google/Azure AD)
2. **Definir variáveis de ambiente** em produção
3. **Deploy da Function** para AWS Lambda
4. **Configurar API Gateway** para a Function
5. **Testes de integração** com IdP real

---

**✅ Fase 3 implementada com sucesso!**
