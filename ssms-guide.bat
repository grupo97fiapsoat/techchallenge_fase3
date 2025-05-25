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
echo %BOLD%%CYAN%║               📊 GUIA: SQL SERVER MANAGEMENT STUDIO (SSMS)                   ║%RESET%
echo %BOLD%%CYAN%║                    Conexão Manual com FastFood Database                       ║%RESET%
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo.

echo %BOLD%%BLUE%📋 INFORMAÇÕES DE CONEXÃO:%RESET%
echo.
echo %WHITE%   Server Type:     %CYAN%Database Engine%RESET%
echo %WHITE%   Server Name:     %CYAN%localhost,1434%RESET% %YELLOW%(NOVA PORTA!)%RESET%
echo %WHITE%                    %GRAY%Alternativa: 127.0.0.1,1434%RESET%
echo %WHITE%   Authentication:  %CYAN%SQL Server Authentication%RESET%
echo %WHITE%   Login:          %CYAN%sa%RESET%
echo %WHITE%   Password:       %CYAN%FastFood2024%RESET%
echo %WHITE%   Database:       %CYAN%FastFoodDb%RESET%
echo.

echo %BOLD%%YELLOW%🔧 PASSO A PASSO:%RESET%
echo.

echo %BOLD%%WHITE%PASSO 1: Verificar se o SQL Server está rodando%RESET%
echo %CYAN%   1.1. Abra o prompt de comando ou PowerShell%RESET%
echo %CYAN%   1.2. Execute: docker-compose ps%RESET%
echo %CYAN%   1.3. Verifique se o container 'db' está com status 'Up'%RESET%
echo %WHITE%        Se não estiver rodando, execute: docker-compose up -d db%RESET%
echo.

echo %BOLD%%WHITE%PASSO 2: Abrir o SQL Server Management Studio%RESET%
echo %CYAN%   2.1. Procure por "SSMS" no menu Iniciar%RESET%
echo %CYAN%   2.2. Ou procure por "SQL Server Management Studio"%RESET%
echo %CYAN%   2.3. Clique para abrir o programa%RESET%
echo.

echo %BOLD%%WHITE%PASSO 3: Configurar a Conexão%RESET%
echo %CYAN%   3.1. Na tela "Connect to Server" que aparece automaticamente:%RESET%
echo.
echo %WHITE%        Server type: %YELLOW%Database Engine%RESET% %WHITE%(já selecionado)%RESET%
echo %WHITE%        Server name: %YELLOW%localhost,1434%RESET%
echo %WHITE%                     %GRAY%OU use: 127.0.0.1,1434%RESET%
echo %WHITE%                     %GRAY%OU use: .\SQLEXPRESS (se instalado localmente)%RESET%
echo.
echo %WHITE%        Authentication: %YELLOW%SQL Server Authentication%RESET%
echo %WHITE%                        %GRAY%(NÃO use Windows Authentication)%RESET%
echo.
echo %WHITE%        Login: %YELLOW%sa%RESET%
echo %WHITE%        Password: %YELLOW%FastFood2024%RESET%
echo.

echo %BOLD%%WHITE%PASSO 4: Opções Avançadas (Opcional)%RESET%
echo %CYAN%   4.1. Clique em "Options >>" para expandir%RESET%
echo %CYAN%   4.2. Na aba "Connection Properties":%RESET%
echo %WHITE%        Connect to database: %YELLOW%FastFoodDb%RESET%
echo %WHITE%        Network protocol: %YELLOW%TCP/IP%RESET%
echo %WHITE%        Connection timeout: %YELLOW%30%RESET%
echo %WHITE%        Execution timeout: %YELLOW%0%RESET%
echo.

echo %BOLD%%WHITE%PASSO 5: Conectar%RESET%
echo %CYAN%   5.1. Clique no botão "Connect"%RESET%
echo %CYAN%   5.2. Aguarde alguns segundos para a conexão%RESET%
echo %CYAN%   5.3. Se conectou com sucesso, você verá o Object Explorer%RESET%
echo.

echo %BOLD%%GREEN%✅ APÓS CONECTAR:%RESET%
echo.
echo %WHITE%• No Object Explorer, expanda:%RESET%
echo %CYAN%  └── localhost,1434 (SQL Server...)%RESET%
echo %CYAN%      └── Databases%RESET%
echo %CYAN%          └── FastFoodDb%RESET%
echo %CYAN%              ├── Tables%RESET%
echo %CYAN%              │   ├── dbo.Customers%RESET%
echo %CYAN%              │   ├── dbo.Orders%RESET%
echo %CYAN%              │   ├── dbo.Products%RESET%
echo %CYAN%              │   └── ...%RESET%
echo %CYAN%              ├── Views%RESET%
echo %CYAN%              ├── Stored Procedures%RESET%
echo %CYAN%              └── Functions%RESET%
echo.

echo %BOLD%%BLUE%🔍 COMANDOS ÚTEIS PARA TESTAR:%RESET%
echo.
echo %WHITE%-- Ver todas as tabelas%RESET%
echo %YELLOW%SELECT name FROM sys.tables ORDER BY name;%RESET%
echo.
echo %WHITE%-- Ver dados dos produtos%RESET%
echo %YELLOW%SELECT TOP 10 * FROM Products;%RESET%
echo.
echo %WHITE%-- Ver dados dos pedidos%RESET%
echo %YELLOW%SELECT TOP 10 * FROM Orders;%RESET%
echo.
echo %WHITE%-- Contar registros%RESET%
echo %YELLOW%SELECT %RESET%
echo %YELLOW%    'Products' as Tabela, COUNT(*) as Total FROM Products%RESET%
echo %YELLOW%UNION ALL%RESET%
echo %YELLOW%SELECT 'Orders', COUNT(*) FROM Orders%RESET%
echo %YELLOW%UNION ALL%RESET%
echo %YELLOW%SELECT 'Customers', COUNT(*) FROM Customers;%RESET%
echo.

:TROUBLESHOOTING
echo %BOLD%%RED%🛠️  TROUBLESHOOTING:%RESET%
echo.

echo %WHITE%❌ "A network-related or instance-specific error occurred"%RESET%
echo %CYAN%   → Verifique se o Docker está rodando: docker info%RESET%
echo %CYAN%   → Verifique se o container está up: docker-compose ps%RESET%
echo %CYAN%   → Reinicie o banco: docker-compose restart db%RESET%
echo %CYAN%   → Aguarde 30-60 segundos após reiniciar%RESET%
echo.

echo %WHITE%❌ "Login failed for user 'sa'"%RESET%
echo %CYAN%   → Confirme a senha: FastFood2024 (case-sensitive)%RESET%
echo %CYAN%   → Use SQL Server Authentication, não Windows Auth%RESET%
echo %CYAN%   → Verifique se não há espaços na senha%RESET%
echo.

echo %WHITE%❌ "Cannot connect to localhost,1434"%RESET%
echo %CYAN%   → Tente usar: 127.0.0.1,1434%RESET%
echo %CYAN%   → Ou apenas: localhost%RESET%
echo %CYAN%   → Verifique se a porta 1434 não está bloqueada%RESET%
echo %CYAN%   → Execute: netstat -an | findstr :1434%RESET%
echo.

echo %WHITE%❌ "Database 'FastFoodDb' does not exist"%RESET%
echo %CYAN%   → Execute as migrações: init.bat → opção 6%RESET%
echo %CYAN%   → Ou: docker-compose up migrations%RESET%
echo %CYAN%   → Conecte primeiro sem especificar database%RESET%
echo.

echo %WHITE%❌ "Timeout expired"%RESET%
echo %CYAN%   → Aumentar timeout de conexão para 60 segundos%RESET%
echo %CYAN%   → Verificar recursos do sistema: docker stats%RESET%
echo %CYAN%   → Reiniciar Docker Desktop se necessário%RESET%
echo.

echo %BOLD%%YELLOW%💡 DICAS IMPORTANTES:%RESET%
echo.
echo %WHITE%• Use sempre "localhost,1434" com a vírgula (não dois pontos)%RESET%
echo %WHITE%• O SQL Server pode demorar 30-60 segundos para aceitar conexões%RESET%
echo %WHITE%• Se o SSMS não conseguir conectar, teste com Azure Data Studio%RESET%
echo %WHITE%• Para conexões externas, use o IP da máquina ao invés de localhost%RESET%
echo %WHITE%• Se mudar a porta no docker-compose.yml, ajuste aqui também%RESET%
echo.

echo %BOLD%%GREEN%🔧 COMANDOS RÁPIDOS PARA VERIFICAÇÃO:%RESET%
echo.
echo %YELLOW%:: Verificar se o banco está rodando%RESET%
echo %CYAN%docker-compose exec db sqlcmd -S localhost -U sa -P "FastFood2024" -Q "SELECT @@VERSION"%RESET%
echo.
echo %YELLOW%:: Ver logs do SQL Server%RESET%
echo %CYAN%docker-compose logs db%RESET%
echo.
echo %YELLOW%:: Conectar via linha de comando%RESET%
echo %CYAN%docker-compose exec db sqlcmd -S localhost -U sa -P "FastFood2024"%RESET%
echo.
echo %YELLOW%:: Verificar status dos containers%RESET%
echo %CYAN%docker-compose ps%RESET%
echo.

echo %BOLD%%CYAN%📋 RESUMO RÁPIDO:%RESET%
echo %WHITE%Server: localhost,1434 | User: sa | Pass: FastFood2024 | DB: FastFoodDb%RESET%
echo.

pause
exit /b 0
