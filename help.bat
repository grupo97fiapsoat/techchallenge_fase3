@echo off
chcp 65001 >nul 2>&1
setlocal EnableDelayedExpansion

:: Colors
set "RED=[91m"
set "GREEN=[92m"
set "YELLOW=[93m"
set "BLUE=[94m"
set "MAGENTA=[95m"
set "CYAN=[96m"
set "WHITE=[97m"
set "RESET=[0m"
set "BOLD=[1m"

cls
echo.
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo %BOLD%%CYAN%║                        📚 FASTFOOD API - DOCUMENTAÇÃO                         ║%RESET%
echo %BOLD%%CYAN%║                              Guia Completo de Uso                             ║%RESET%
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo.

:HELP_MENU
echo %BOLD%%YELLOW%📋 MENU DE AJUDA:%RESET%
echo.
echo %WHITE%   1️⃣  Visão Geral do Projeto%RESET%
echo %WHITE%   2️⃣  Guia de Instalação%RESET%
echo %WHITE%   3️⃣  Comandos Úteis%RESET%
echo %WHITE%   4️⃣  Troubleshooting%RESET%
echo %WHITE%   5️⃣  Arquitetura do Sistema%RESET%
echo %WHITE%   6️⃣  APIs e Endpoints%RESET%
echo %WHITE%   7️⃣  Banco de Dados%RESET%
echo %WHITE%   8️⃣  Certificados SSL%RESET%
echo %WHITE%   9️⃣  FAQ - Perguntas Frequentes%RESET%
echo %WHITE%   0️⃣  Voltar%RESET%
echo.
set /p "choice=%CYAN%🎯 Escolha uma opção: %RESET%"

if "%choice%"=="1" goto OVERVIEW
if "%choice%"=="2" goto INSTALL_GUIDE
if "%choice%"=="3" goto USEFUL_COMMANDS
if "%choice%"=="4" goto TROUBLESHOOTING
if "%choice%"=="5" goto ARCHITECTURE
if "%choice%"=="6" goto APIS
if "%choice%"=="7" goto DATABASE
if "%choice%"=="8" goto CERTIFICATES
if "%choice%"=="9" goto FAQ
if "%choice%"=="0" exit /b 0
goto HELP_MENU

:OVERVIEW
cls
echo %BOLD%%BLUE%📖 VISÃO GERAL DO PROJETO%RESET%
echo.
echo %WHITE%🍔 FastFood API é uma aplicação completa para gerenciamento de fast food,%RESET%
echo %WHITE%   desenvolvida com .NET 8.0 e arquitetura limpa (Clean Architecture).%RESET%
echo.
echo %BOLD%%CYAN%🏗️  Tecnologias Utilizadas:%RESET%
echo %WHITE%   • .NET 8.0 Web API%RESET%
echo %WHITE%   • Entity Framework Core%RESET%
echo %WHITE%   • SQL Server 2022%RESET%
echo %WHITE%   • Docker & Docker Compose%RESET%
echo %WHITE%   • AutoMapper%RESET%
echo %WHITE%   • FluentValidation%RESET%
echo %WHITE%   • MediatR (CQRS Pattern)%RESET%
echo %WHITE%   • Swagger/OpenAPI%RESET%
echo.
echo %BOLD%%CYAN%📁 Estrutura do Projeto:%RESET%
echo %WHITE%   • FastFood.Api        - Camada de apresentação%RESET%
echo %WHITE%   • FastFood.Application - Camada de aplicação (CQRS)%RESET%
echo %WHITE%   • FastFood.Domain     - Camada de domínio%RESET%
echo %WHITE%   • FastFood.Infrastructure - Camada de dados%RESET%
echo.
pause
goto HELP_MENU

:INSTALL_GUIDE
cls
echo %BOLD%%BLUE%🚀 GUIA DE INSTALAÇÃO%RESET%
echo.
echo %BOLD%%CYAN%📋 Pré-requisitos:%RESET%
echo %WHITE%   1. Docker Desktop instalado e rodando%RESET%
echo %WHITE%   2. Docker Compose disponível%RESET%
echo %WHITE%   3. PowerShell (para geração de certificados)%RESET%
echo %WHITE%   4. Portas 1433, 5000 e 5001 livres%RESET%
echo.
echo %BOLD%%CYAN%⚡ Instalação Rápida:%RESET%
echo %WHITE%   1. Execute: init.bat%RESET%
echo %WHITE%   2. Escolha opção "1" (Instalação Completa)%RESET%
echo %WHITE%   3. Aguarde a conclusão (5-10 minutos)%RESET%
echo %WHITE%   4. Acesse: https://localhost:5001/swagger%RESET%
echo.
echo %BOLD%%CYAN%🔧 Instalação Manual:%RESET%
echo %WHITE%   1. init.bat → opção "2" (Verificar Pré-requisitos)%RESET%
echo %WHITE%   2. init.bat → opção "3" (Configurar Ambiente)%RESET%
echo %WHITE%   3. init.bat → opção "4" (Gerar Certificados)%RESET%
echo %WHITE%   4. init.bat → opção "5" (Inicializar Docker)%RESET%
echo %WHITE%   5. init.bat → opção "6" (Executar Migrações)%RESET%
echo %WHITE%   6. init.bat → opção "7" (Verificar Status)%RESET%
echo.
pause
goto HELP_MENU

:USEFUL_COMMANDS
cls
echo %BOLD%%BLUE%💻 COMANDOS ÚTEIS%RESET%
echo.
echo %BOLD%%CYAN%🐳 Docker Compose:%RESET%
echo %YELLOW%   docker-compose up -d              %RESET%- Iniciar todos os serviços
echo %YELLOW%   docker-compose down               %RESET%- Parar todos os serviços
echo %YELLOW%   docker-compose logs -f api        %RESET%- Ver logs da API em tempo real
echo %YELLOW%   docker-compose ps                 %RESET%- Status dos containers
echo %YELLOW%   docker-compose restart api        %RESET%- Reiniciar apenas a API
echo %YELLOW%   docker-compose build --no-cache   %RESET%- Rebuildar imagens
echo.
echo %BOLD%%CYAN%🗄️  Banco de Dados:%RESET%
echo %YELLOW%   docker-compose exec db sqlcmd -S localhost -U sa -P "FastFood2024"%RESET%
echo %WHITE%     → Conectar ao SQL Server via linha de comando%RESET%
echo.
echo %BOLD%%CYAN%🔍 Monitoramento:%RESET%
echo %YELLOW%   monitor.bat                       %RESET%- Monitor de status em tempo real
echo %YELLOW%   docker stats                      %RESET%- Uso de recursos dos containers
echo %YELLOW%   docker system df                  %RESET%- Uso de espaço em disco
echo.
echo %BOLD%%CYAN%🧹 Limpeza:%RESET%
echo %YELLOW%   docker system prune -f            %RESET%- Limpar recursos não utilizados
echo %YELLOW%   docker-compose down -v            %RESET%- Remover volumes
echo %YELLOW%   init.bat → opção "9"              %RESET%- Reset completo
echo.
pause
goto HELP_MENU

:TROUBLESHOOTING
cls
echo %BOLD%%BLUE%🔧 TROUBLESHOOTING%RESET%
echo.
echo %BOLD%%RED%❌ Problemas Comuns:%RESET%
echo.
echo %WHITE%🔸 "Docker não está rodando"%RESET%
echo %CYAN%   → Inicie o Docker Desktop%RESET%
echo %CYAN%   → Verifique se o serviço está ativo%RESET%
echo.
echo %WHITE%🔸 "Porta já está em uso"%RESET%
echo %CYAN%   → netstat -ano | findstr :5000    (verificar porta 5000)%RESET%
echo %CYAN%   → taskkill /PID [PID] /F          (matar processo)%RESET%
echo %CYAN%   → docker-compose down             (parar containers)%RESET%
echo.
echo %WHITE%🔸 "Falha na conexão com banco"%RESET%
echo %CYAN%   → Aguarde 30-60 segundos após iniciar%RESET%
echo %CYAN%   → Verifique senha: FastFood2024%RESET%
echo %CYAN%   → docker-compose restart db%RESET%
echo.
echo %WHITE%🔸 "API não responde"%RESET%
echo %CYAN%   → Aguarde compilação (1-2 minutos)%RESET%
echo %CYAN%   → docker-compose logs api%RESET%
echo %CYAN%   → Verificar certificados SSL%RESET%
echo.
echo %WHITE%🔸 "Erro de certificado SSL"%RESET%
echo %CYAN%   → Execute: init.bat → opção "4"%RESET%
echo %CYAN%   → Aceite o certificado no navegador%RESET%
echo %CYAN%   → Use http://localhost:5000 como alternativa%RESET%
echo.
echo %BOLD%%GREEN%✅ Comandos de Diagnóstico:%RESET%
echo %YELLOW%   docker-compose ps                 %RESET%- Status dos containers
echo %YELLOW%   docker-compose logs api           %RESET%- Logs da API
echo %YELLOW%   monitor.bat                       %RESET%- Status em tempo real
echo %YELLOW%   init.bat → opção "7"              %RESET%- Verificação completa
echo.
pause
goto HELP_MENU

:ARCHITECTURE
cls
echo %BOLD%%BLUE%🏗️  ARQUITETURA DO SISTEMA%RESET%
echo.
echo %BOLD%%CYAN%📋 Clean Architecture (DDD):%RESET%
echo.
echo %WHITE%┌─ FastFood.Api (Presentation Layer)%RESET%
echo %WHITE%│  ├─ Controllers%RESET%
echo %WHITE%│  ├─ Middlewares%RESET%
echo %WHITE%│  └─ Program.cs / Startup%RESET%
echo %WHITE%│%RESET%
echo %WHITE%├─ FastFood.Application (Application Layer)%RESET%
echo %WHITE%│  ├─ Commands (CQRS Write)%RESET%
echo %WHITE%│  ├─ Queries (CQRS Read)%RESET%
echo %WHITE%│  ├─ DTOs%RESET%
echo %WHITE%│  ├─ Validators%RESET%
echo %WHITE%│  └─ Handlers%RESET%
echo %WHITE%│%RESET%
echo %WHITE%├─ FastFood.Domain (Domain Layer)%RESET%
echo %WHITE%│  ├─ Entities%RESET%
echo %WHITE%│  ├─ Value Objects%RESET%
echo %WHITE%│  ├─ Repositories (Interfaces)%RESET%
echo %WHITE%│  └─ Domain Services%RESET%
echo %WHITE%│%RESET%
echo %WHITE%└─ FastFood.Infrastructure (Infrastructure Layer)%RESET%
echo %WHITE%   ├─ Data Context (EF Core)%RESET%
echo %WHITE%   ├─ Repositories (Implementation)%RESET%
echo %WHITE%   ├─ Migrations%RESET%
echo %WHITE%   └─ External Services%RESET%
echo.
echo %BOLD%%CYAN%🐳 Containerização:%RESET%
echo %WHITE%   • API Container    - Aplicação .NET%RESET%
echo %WHITE%   • DB Container     - SQL Server 2022%RESET%
echo %WHITE%   • Migration Container - EF Migrations%RESET%
echo.
pause
goto HELP_MENU

:APIS
cls
echo %BOLD%%BLUE%🌐 APIs E ENDPOINTS%RESET%
echo.
echo %BOLD%%CYAN%📍 URLs Base:%RESET%
echo %WHITE%   • HTTP:  http://localhost:5000%RESET%
echo %WHITE%   • HTTPS: https://localhost:5001%RESET%
echo %WHITE%   • Swagger: https://localhost:5001/swagger%RESET%
echo.
echo %BOLD%%CYAN%🔧 Endpoints Principais:%RESET%
echo %WHITE%   GET    /health                    - Health check%RESET%
echo %WHITE%   GET    /api/produtos              - Listar produtos%RESET%
echo %WHITE%   POST   /api/produtos              - Criar produto%RESET%
echo %WHITE%   GET    /api/produtos/{id}         - Obter produto%RESET%
echo %WHITE%   PUT    /api/produtos/{id}         - Atualizar produto%RESET%
echo %WHITE%   DELETE /api/produtos/{id}         - Excluir produto%RESET%
echo.
echo %WHITE%   GET    /api/pedidos               - Listar pedidos%RESET%
echo %WHITE%   POST   /api/pedidos               - Criar pedido%RESET%
echo %WHITE%   GET    /api/pedidos/{id}          - Obter pedido%RESET%
echo %WHITE%   PUT    /api/pedidos/{id}/status   - Atualizar status%RESET%
echo.
echo %BOLD%%CYAN%🧪 Teste de API:%RESET%
echo %YELLOW%   curl http://localhost:5000/health%RESET%
echo %YELLOW%   curl https://localhost:5001/api/produtos%RESET%
echo %WHITE%   💡 Use Postman, Insomnia ou Swagger UI para testes completos%RESET%
echo.
echo %BOLD%%CYAN%📊 Status Codes:%RESET%
echo %WHITE%   200 - OK                  400 - Bad Request%RESET%
echo %WHITE%   201 - Created             401 - Unauthorized%RESET%
echo %WHITE%   204 - No Content          404 - Not Found%RESET%
echo %WHITE%   500 - Internal Server Error%RESET%
echo.
pause
goto HELP_MENU

:DATABASE
cls
echo %BOLD%%BLUE%🗄️  BANCO DE DADOS%RESET%
echo.
echo %BOLD%%CYAN%📋 Informações de Conexão:%RESET%
echo %WHITE%   • Servidor: localhost:1433%RESET%
echo %WHITE%   • Database: FastFoodDb%RESET%
echo %WHITE%   • Usuário:  sa%RESET%
echo %WHITE%   • Senha:    FastFood2024%RESET%
echo %WHITE%   • Provider: SQL Server 2022%RESET%
echo.
echo %BOLD%%CYAN%🗃️  Estrutura Principal:%RESET%
echo %WHITE%   • Produtos    - Catálogo de produtos%RESET%
echo %WHITE%   • Categorias  - Categorias de produtos%RESET%
echo %WHITE%   • Pedidos     - Pedidos dos clientes%RESET%
echo %WHITE%   • ItemPedido  - Itens de cada pedido%RESET%
echo %WHITE%   • Clientes    - Informações dos clientes%RESET%
echo.
echo %BOLD%%CYAN%🔧 Comandos SQL Úteis:%RESET%
echo %YELLOW%   -- Conectar ao banco%RESET%
echo %YELLOW%   docker-compose exec db sqlcmd -S localhost -U sa -P "FastFood2024"%RESET%
echo.
echo %YELLOW%   -- Verificar tabelas%RESET%
echo %YELLOW%   SELECT name FROM sys.tables;%RESET%
echo.
echo %YELLOW%   -- Contar registros%RESET%
echo %YELLOW%   SELECT 'Produtos' as Tabela, COUNT(*) as Registros FROM Produtos%RESET%
echo %YELLOW%   UNION ALL%RESET%
echo %YELLOW%   SELECT 'Pedidos', COUNT(*) FROM Pedidos;%RESET%
echo.
echo %BOLD%%CYAN%🔄 Migrações:%RESET%
echo %WHITE%   • Executar: init.bat → opção "6"%RESET%
echo %WHITE%   • Manual: docker-compose up migrations%RESET%
echo %WHITE%   • Logs: docker-compose logs migrations%RESET%
echo.
echo %BOLD%%CYAN%💾 Backup/Restore:%RESET%
echo %WHITE%   • Volume: fastfood-data%RESET%
echo %WHITE%   • Localização: /var/opt/mssql (no container)%RESET%
echo %WHITE%   • Backup: docker-compose exec db sqlcmd -Q "BACKUP DATABASE...""%RESET%
echo.
pause
goto HELP_MENU

:CERTIFICATES
cls
echo %BOLD%%BLUE%🔐 CERTIFICADOS SSL%RESET%
echo.
echo %BOLD%%CYAN%📁 Localização:%RESET%
echo %WHITE%   • Arquivo: ./certs/fastfood-dev.pfx%RESET%
echo %WHITE%   • Senha: fastfood123%RESET%
echo %WHITE%   • Formato: PKCS#12 (.pfx)%RESET%
echo.
echo %BOLD%%CYAN%🔧 Geração:%RESET%
echo %WHITE%   • Automática: init.bat → opção "4"%RESET%
echo %WHITE%   • Via PowerShell: New-SelfSignedCertificate%RESET%
echo %WHITE%   • Domínios: localhost, 127.0.0.1, fastfood-api%RESET%
echo %WHITE%   • Validade: 5 anos%RESET%
echo.
echo %BOLD%%CYAN%🌐 Configuração no Navegador:%RESET%
echo %WHITE%   1. Acesse https://localhost:5001%RESET%
echo %WHITE%   2. Clique em "Avançado" no aviso de segurança%RESET%
echo %WHITE%   3. Clique em "Prosseguir para localhost (não seguro)"%RESET%
echo %WHITE%   4. Ou instale o certificado como confiável:%RESET%
echo %WHITE%      → Windows: Painel de Controle → Certificados%RESET%
echo %WHITE%      → Chrome: Configurações → Privacidade → Certificados%RESET%
echo.
echo %BOLD%%CYAN%🔍 Verificação:%RESET%
echo %YELLOW%   # Verificar se existe%RESET%
echo %YELLOW%   dir certs\fastfood-dev.pfx%RESET%
echo.
echo %YELLOW%   # Testar HTTPS%RESET%
echo %YELLOW%   curl -k https://localhost:5001/health%RESET%
echo.
echo %YELLOW%   # Regenerar se necessário%RESET%
echo %YELLOW%   init.bat → opção "4"%RESET%
echo.
echo %BOLD%%CYAN%⚠️  Troubleshooting SSL:%RESET%
echo %WHITE%   • "ERR_CERT_AUTHORITY_INVALID" → Normal para certificado autoassinado%RESET%
echo %WHITE%   • "ERR_CONNECTION_REFUSED" → API não está rodando%RESET%
echo %WHITE%   • "ERR_CERT_COMMON_NAME_INVALID" → Use localhost, não 127.0.0.1%RESET%
echo.
pause
goto HELP_MENU

:FAQ
cls
echo %BOLD%%BLUE%❓ FAQ - PERGUNTAS FREQUENTES%RESET%
echo.
echo %BOLD%%CYAN%Q: Quanto tempo demora a instalação completa?%RESET%
echo %WHITE%A: Entre 5-10 minutos, dependendo da velocidade da internet%RESET%
echo %WHITE%   para download das imagens Docker.%RESET%
echo.
echo %BOLD%%CYAN%Q: Posso usar sem Docker?%RESET%
echo %WHITE%A: Tecnicamente sim, mas é altamente recomendado usar Docker%RESET%
echo %WHITE%   para consistência e facilidade de configuração.%RESET%
echo.
echo %BOLD%%CYAN%Q: Como acessar o banco de dados externamente?%RESET%
echo %WHITE%A: Use qualquer cliente SQL Server:%RESET%
echo %WHITE%   Server: localhost,1433%RESET%
echo %WHITE%   User: sa / Password: FastFood2024%RESET%
echo.
echo %BOLD%%CYAN%Q: A API funciona no HTTP também?%RESET%
echo %WHITE%A: Sim! Use http://localhost:5000 se tiver problemas com HTTPS.%RESET%
echo.
echo %BOLD%%CYAN%Q: Como ver os logs detalhados?%RESET%
echo %WHITE%A: docker-compose logs -f api%RESET%
echo %WHITE%   Ou use monitor.bat para visualização em tempo real.%RESET%
echo.
echo %BOLD%%CYAN%Q: Posso mudar as portas?%RESET%
echo %WHITE%A: Sim, edite o arquivo docker-compose.yml%RESET%
echo %WHITE%   e ajuste as portas na seção 'ports'.%RESET%
echo.
echo %BOLD%%CYAN%Q: Como fazer backup dos dados?%RESET%
echo %WHITE%A: Os dados estão no volume 'fastfood-data'.%RESET%
echo %WHITE%   Use: docker volume inspect fastfood-data%RESET%
echo.
echo %BOLD%%CYAN%Q: API está lenta, o que fazer?%RESET%
echo %WHITE%A: 1. Verifique recursos: docker stats%RESET%
echo %WHITE%   2. Reinicie containers: docker-compose restart%RESET%
echo %WHITE%   3. Limpe cache: docker system prune%RESET%
echo.
echo %BOLD%%CYAN%Q: Como atualizar para nova versão?%RESET%
echo %WHITE%A: 1. git pull%RESET%
echo %WHITE%   2. docker-compose build --no-cache%RESET%
echo %WHITE%   3. docker-compose up -d%RESET%
echo.
echo %BOLD%%CYAN%Q: Erro "connection refused", o que fazer?%RESET%
echo %WHITE%A: 1. Verifique se Docker está rodando%RESET%
echo %WHITE%   2. Execute: docker-compose ps%RESET%
echo %WHITE%   3. Restart: docker-compose restart api%RESET%
echo.
pause
goto HELP_MENU
