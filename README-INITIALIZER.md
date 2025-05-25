# 🚀 FastFood API - Inicializador Moderno v2.0

## 📋 Visão Geral

Este projeto inclui um **sistema completo de inicialização end-to-end** para a FastFood API, desenvolvido com foco na **experiência do usuário**, **automação** e **facilidade de uso**.

## ⚡ Quick Start (Recomendado)

1. **Execute o launcher**:
   ```bash
   launcher.bat
   ```

2. **Escolha a opção 1** (Instalação Express)

3. **Aguarde 5-10 minutos** para a instalação completa

4. **Acesse**: https://localhost:5001/swagger

## 🛠️ Scripts Disponíveis

| Script | Função | Descrição |
|--------|--------|-----------|
| `launcher.bat` | 🚀 **Launcher Principal** | Menu principal com todas as opções |
| `init.bat` | ⚙️ **Inicializador Completo** | Instalação step-by-step detalhada |
| `monitor.bat` | 📊 **Monitor em Tempo Real** | Status dos serviços em tempo real |
| `help.bat` | 📚 **Documentação** | Guia completo e troubleshooting |

## 🔧 Instalação Detalhada

### Pré-requisitos
- ✅ Docker Desktop instalado e rodando
- ✅ PowerShell disponível
- ✅ Portas 1433, 5000, 5001 livres

### Método 1: Instalação Express (Recomendado)
```bash
# Execute o launcher
launcher.bat

# Escolha: 1️⃣ Instalação Express
# Aguarde a conclusão automática
```

### Método 2: Instalação Step-by-Step
```bash
# Execute o inicializador
init.bat

# Siga o menu:
# 1. Verificar Pré-requisitos
# 2. Configurar Ambiente  
# 3. Gerar Certificados SSL
# 4. Inicializar Docker
# 5. Executar Migrações
# 6. Verificar Status
```

### Método 3: Manual (Avançado)
```bash
# 1. Configurar ambiente
cd init
call check-prereq.bat
call setup-env.bat
call gen-certs.bat

# 2. Inicializar Docker
call docker-init.bat
call run-migrations.bat

# 3. Verificar
call verify-install.bat
```

## 🌐 Endpoints da API

| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/health` | GET | Health check |
| `/api/produtos` | GET | Listar produtos |
| `/api/produtos` | POST | Criar produto |
| `/api/produtos/{id}` | GET | Obter produto |
| `/api/produtos/{id}` | PUT | Atualizar produto |
| `/api/produtos/{id}` | DELETE | Excluir produto |
| `/api/pedidos` | GET | Listar pedidos |
| `/api/pedidos` | POST | Criar pedido |
| `/api/pedidos/{id}` | GET | Obter pedido |
| `/api/pedidos/{id}/status` | PUT | Atualizar status |

## 🔗 URLs de Acesso

- **Swagger UI**: https://localhost:5001/swagger
- **API HTTPS**: https://localhost:5001
- **API HTTP**: http://localhost:5000
- **Health Check**: http://localhost:5000/health

## 🗄️ Banco de Dados

| Configuração | Valor |
|--------------|-------|
| **Servidor** | localhost:1433 |
| **Database** | FastFoodDb |
| **Usuário** | sa |
| **Senha** | FastFood2024 |
| **Provider** | SQL Server 2022 |

## 📊 Monitoramento

### Monitor em Tempo Real
```bash
monitor.bat
```

### Comandos Úteis
```bash
# Status dos containers
docker-compose ps

# Logs da API
docker-compose logs -f api

# Logs do banco
docker-compose logs -f db

# Reiniciar serviços
docker-compose restart

# Parar tudo
docker-compose down

# Iniciar tudo
docker-compose up -d
```

## 🔐 Certificados SSL

- **Localização**: `./certs/fastfood-dev.pfx`
- **Senha**: `fastfood123`
- **Domínios**: localhost, 127.0.0.1, fastfood-api
- **Validade**: 5 anos

### Regenerar Certificados
```bash
init.bat
# Escolha opção 4: Gerar Certificados SSL
```

## 🔧 Troubleshooting

### Problemas Comuns

#### "Docker não está rodando"
```bash
# Solução: Inicie o Docker Desktop
# Verifique: docker info
```

#### "Porta já está em uso"
```bash
# Verificar porta
netstat -ano | findstr :5000

# Parar containers
docker-compose down

# Matar processo (se necessário)
taskkill /PID [PID] /F
```

#### "API não responde"
```bash
# Verificar logs
docker-compose logs api

# Reiniciar API
docker-compose restart api

# Aguardar compilação (1-2 minutos)
```

#### "Erro de certificado SSL"
```bash
# Regenerar certificado
init.bat → opção 4

# Ou usar HTTP
http://localhost:5000
```

### Comandos de Diagnóstico
```bash
# Status completo
monitor.bat

# Verificação do sistema
launcher.bat → opção 6

# Logs detalhados
help.bat → seção Troubleshooting
```

## 🚀 Funcionalidades Avançadas

### Reset e Limpeza
```bash
# Reset completo
launcher.bat → opção 8 → opção 3

# Ou via init.bat
init.bat → opção 9
```

### Backup e Restore
```bash
# Volume dos dados
docker volume inspect fastfood-data

# Backup manual
docker-compose exec db sqlcmd -Q "BACKUP DATABASE..."
```

### Logs Avançados
```bash
# Ver logs de instalação
dir logs\*.log

# Logs em tempo real
launcher.bat → opção 7
```

## 🏗️ Arquitetura

### Clean Architecture (DDD)
```
├── FastFood.Api (Presentation)
│   ├── Controllers
│   ├── Middlewares
│   └── Configuration
│
├── FastFood.Application (Application)
│   ├── Commands (CQRS Write)
│   ├── Queries (CQRS Read)
│   ├── DTOs
│   └── Validators
│
├── FastFood.Domain (Domain)
│   ├── Entities
│   ├── Value Objects
│   └── Repositories
│
└── FastFood.Infrastructure (Infrastructure)
    ├── Data Context
    ├── Repositories
    └── Migrations
```

### Containerização
- **API Container**: Aplicação .NET 8.0
- **DB Container**: SQL Server 2022
- **Migration Container**: EF Core Migrations

## 🎯 Novidades v2.0

### ✨ Interface Moderna
- Menu visual com cores e emojis
- Progress indicators em tempo real
- Feedback detalhado de cada etapa

### 🔧 Automação Completa
- Verificação automática de pré-requisitos
- Geração automática de certificados SSL
- Configuração automática do ambiente
- Inicialização automática do Docker

### 📊 Monitoramento Avançado
- Monitor de status em tempo real
- Logs estruturados com timestamps
- Diagnósticos automáticos
- Comandos de recuperação

### 🛡️ Robustez
- Sistema de fallback em múltiplas camadas
- Validação de cada etapa
- Recovery automático de erros
- Rollback completo se necessário

### 📚 Documentação Integrada
- Help system completo
- FAQ interativo
- Troubleshooting guiado
- Comandos úteis contextuais

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch: `git checkout -b feature/nova-funcionalidade`
3. Commit: `git commit -m 'Add nova funcionalidade'`
4. Push: `git push origin feature/nova-funcionalidade`
5. Pull Request

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para detalhes.

---

**🎉 FastFood API v2.0 - Desenvolvido com ❤️ para máxima facilidade de uso!**
