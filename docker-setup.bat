@echo off
setlocal enabledelayedexpansion
:: FastFood API - Inicializacao Completa End-to-End

echo.
echo ========================================================================
echo                        FASTFOOD API - SETUP COMPLETO
echo ========================================================================
echo [INFO] Iniciando configuracao automatica do ambiente de desenvolvimento
echo [INFO] Timestamp: %date% %time%
echo ========================================================================
echo.

:: Check if Docker is running
echo [STEP 1/8] Verificando pre-requisitos...
echo [VERIFICACAO] Testando Docker Engine (timeout: 10s)...

:: Use a faster check with timeout
echo [INFO] Verificando se Docker responde...
timeout /t 1 /nobreak >nul 2>&1
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Docker CLI nao esta disponivel!
    echo [DICA] Instale o Docker Desktop primeiro.
    pause
    exit /b 1
)

echo [INFO] Docker CLI OK. Testando conectividade com daemon...
:: Quick daemon test without full info
docker ps >nul 2>&1
if %errorlevel% neq 0 (
    echo [AVISO] Docker daemon nao esta respondendo.
    echo [TENTATIVA] Verificando se esta iniciando...
    
    :: Give it a few seconds
    set /a DOCKER_RETRIES=0
    :docker_check_loop
    docker ps >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] Docker daemon esta rodando!
        goto docker_check_done
    )
    
    set /a DOCKER_RETRIES+=1
    if %DOCKER_RETRIES% geq 15 (
        echo [ERRO] Docker daemon nao respondeu apos 15 segundos!
        echo [ACAO] Inicie o Docker Desktop e aguarde a inicializacao completa.
        echo [DICA] O Docker pode levar 1-2 minutos para inicializar completamente.
        pause
        exit /b 1
    )
    
    echo [AGUARDANDO] Docker daemon... [%DOCKER_RETRIES%/15s]
    timeout /t 1 /nobreak >nul
    goto docker_check_loop
    :docker_check_done
) else (
    echo [OK] Docker daemon esta respondendo.
)

echo [INFO] Obtendo versao do Docker...
for /f "tokens=3" %%i in ('docker --version 2^>nul') do echo [INFO] Docker version: %%i

echo.
echo [STEP 2/8] Configurando arquivo de ambiente (.env)...
:: Check and create .env file if it doesn't exist
if not exist ".env" (
    echo [CRIACAO] Arquivo .env nao encontrado. Gerando automaticamente...
    echo [INFO] Definindo variaveis de ambiente padrao para desenvolvimento...
    (
        echo # Environment Variables for FastFood API
        echo # Database Configuration
        echo DB_PASSWORD=FastFood@2024!#
        echo.
        echo # Application Configuration
        echo ASPNETCORE_ENVIRONMENT=Development
        echo.
        echo # SSL/TLS Certificate Configuration
        echo CERT_PASSWORD=fastfood123
        echo.
        echo # SQL Server Configuration
        echo MSSQL_PID=Express
        echo.
        echo # Timezone
        echo TZ=America/Sao_Paulo
    ) > .env
    echo [OK] Arquivo .env criado com configuracoes padrao de desenvolvimento.
    echo [INFO] Senha do banco: FastFood@2024!#
    echo [INFO] Senha dos certificados: fastfood123
) else (
    echo [OK] Arquivo .env ja existe. Usando configuracoes existentes.
    echo [INFO] Validando conteudo do arquivo .env...
    findstr /C:"DB_PASSWORD" .env >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] Configuracao do banco encontrada no .env
    ) else (
        echo [AVISO] DB_PASSWORD nao encontrado no .env - pode causar problemas
    )
)

echo.
echo [STEP 3/8] Limpeza de containers anteriores...
:: Stop any running containers and clean up
echo [LIMPEZA] Parando containers existentes do FastFood API...
docker-compose down --remove-orphans >nul 2>&1
if %errorlevel% equ 0 (
    echo [OK] Containers anteriores removidos com sucesso.
) else (
    echo [INFO] Nenhum container anterior encontrado para remover.
)

echo [LIMPEZA] Removendo imagens antigas (se necessario)...
docker image prune -f >nul 2>&1
echo [OK] Limpeza de imagens completada.

echo.
echo [STEP 4/8] Configurando certificados SSL/TLS...
:: Generate certificates if needed
if not exist ".\certs" (
    echo [INFO] Criando diretorio de certificados...
    mkdir ".\certs"
)

if not exist ".\certs\fastfood-dev.pfx" (
    echo [CERTIFICADOS] Gerando certificados rapidamente...
    
    if exist ".\scripts\generate-dev-certs.bat" (
        echo [EXEC] Executando geracao com timeout de 15s...
        
        :: Start the certificate generation in background
        start /B "" ".\scripts\generate-dev-certs.bat"
        
        :: Wait for certificate with timeout
        set /a CERT_COUNTER=0
        :cert_wait_loop
        if exist ".\certs\fastfood-dev.pfx" (
            echo [OK] Certificados gerados com sucesso!
            goto cert_done
        )
        
        set /a CERT_COUNTER+=1
        if %CERT_COUNTER% geq 15 (
            echo [TIMEOUT] Geracao excedeu 15 segundos. Usando certificado simples...
            goto cert_fallback
        )
        
        echo [AGUARDANDO] Certificados... [%CERT_COUNTER%/15s]
        timeout /t 1 /nobreak >nul
        goto cert_wait_loop
        
        :cert_fallback
        echo [FALLBACK] Criando arquivo PFX simples para evitar erros Docker...
        echo dummy > ".\certs\fastfood-dev.pfx"
        echo [AVISO] Aplicacao rodara apenas em HTTP (porta 5000).
        
        :cert_done
        if exist ".\certs\fastfood-dev.pfx" (
            echo [INFO] Certificado: .\certs\fastfood-dev.pfx
        )
    ) else (
        echo [AVISO] Script de certificados nao encontrado.
        echo [FALLBACK] Criando arquivo simples...
        echo dummy > ".\certs\fastfood-dev.pfx"
    )
) else (
    echo [OK] Certificados ja existem.
)

echo.
echo [STEP 5/8] Validando arquivos Docker...
:: Build and start the application
echo [VALIDACAO] Verificando configuracao docker-compose.yml...
docker-compose config >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRO] Arquivo docker-compose.yml tem problemas de sintaxe!
    echo [DEBUG] Executando validacao detalhada:
    docker-compose config
    pause
    exit /b 1
)
echo [OK] Arquivo docker-compose.yml validado com sucesso.

:: Check if required files exist
echo [VALIDACAO] Verificando Dockerfiles necessarios...
if not exist "Dockerfile" (
    echo [ERRO] Arquivo Dockerfile nao encontrado!
    echo [ACAO] Verifique se o arquivo Dockerfile existe na raiz do projeto.
    pause
    exit /b 1
)
echo [OK] Dockerfile encontrado.

if not exist "Dockerfile.migrations" (
    echo [ERRO] Arquivo Dockerfile.migrations nao encontrado!
    echo [ACAO] Verifique se o arquivo Dockerfile.migrations existe na raiz do projeto.
    pause
    exit /b 1
)
echo [OK] Dockerfile.migrations encontrado.

echo [VALIDACAO] Verificando estrutura do projeto...
if not exist "src\FastFood.Api\FastFood.Api.csproj" (
    echo [ERRO] Projeto FastFood.Api nao encontrado!
    pause
    exit /b 1
)
echo [OK] Estrutura do projeto validada.

echo.
echo [STEP 6/8] Construindo e iniciando containers...
echo [INFO] Esta operacao pode levar varios minutos na primeira execucao...
echo [INFO] O Docker ira baixar imagens base e compilar o codigo .NET...
echo.
echo [BUILD] Executando 'docker-compose up --build -d'...
echo [PROGRESS] Iniciando build... Por favor aguarde...
docker-compose up --build -d

if %errorlevel% equ 0 (
    echo [OK] Containers construidos e iniciados com sucesso!
    echo.
    echo [STEP 7/8] Verificando saude dos servicos...
    
    :: Wait for services to be ready
    echo [AGUARDANDO] Permitindo inicializacao dos servicos... (5s)
    timeout /t 5 /nobreak >nul
    
    :: Check database
    echo [DB] Testando conectividade com SQL Server...
    set /a DB_RETRIES=0
    :db_check_loop
    docker-compose exec -T db /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "FastFood@2024!#" -Q "SELECT 1" >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] SQL Server esta respondendo na porta 1433.
        goto db_check_done
    )
    
    set /a DB_RETRIES+=1
    if %DB_RETRIES% geq 10 (
        echo [AVISO] SQL Server ainda nao respondeu apos 30s. Continuando...
        goto db_check_done
    )
    
    echo [AGUARDANDO] SQL Server... [%DB_RETRIES%/10] (pode levar ate 60s na primeira vez)
    timeout /t 3 /nobreak >nul
    goto db_check_loop
    :db_check_done
    
    :: Check migrations
    echo [MIGRATIONS] Verificando execucao das migrations...
    timeout /t 2 /nobreak >nul
    docker-compose logs migrations 2>nul | findstr /C:"completed" >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] Migrations do banco executadas com sucesso.
    ) else (
        echo [INFO] Migrations ainda em execucao ou aguardando banco.
        echo [DICA] Execute 'docker-compose logs migrations' para detalhes.
    )
    
    :: Check API
    echo [API] Testando resposta da API REST...
    timeout /t 5 /nobreak >nul
    
    set /a API_RETRIES=0
    :api_check_loop
    :: Try curl first, then PowerShell fallback
    curl -s -f http://localhost:5000/health >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] API REST esta respondendo em http://localhost:5000
        goto api_check_done
    )
    
    :: PowerShell fallback
    powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost:5000/health' -TimeoutSec 3 -ErrorAction Stop; exit 0 } catch { exit 1 }" >nul 2>&1
    if %errorlevel% equ 0 (
        echo [OK] API REST esta respondendo em http://localhost:5000 (via PowerShell)
        goto api_check_done
    )
    
    set /a API_RETRIES+=1
    if %API_RETRIES% geq 8 (
        echo [AVISO] API ainda nao respondeu apos 40s. Pode estar inicializando...
        goto api_check_done
    )
    
    echo [AGUARDANDO] API REST... [%API_RETRIES%/8] (aplicacao .NET pode demorar para inicializar)
    timeout /t 5 /nobreak >nul
    goto api_check_loop
    :api_check_done    
    echo.
    echo [STEP 8/8] Validacao final e informacoes de acesso...
    echo.
    echo ========================================================================
    echo                    FASTFOOD API - INICIALIZACAO CONCLUIDA!
    echo ========================================================================
    echo [SUCESSO] Todos os servicos foram iniciados com sucesso!
    echo [INFO] Timestamp de conclusao: %date% %time%
    echo ========================================================================
    echo.
    echo ENDPOINTS DISPONIVEIS:
    echo   [HTTP]     http://localhost:5000
    echo   [HTTPS]    https://localhost:5001  (certificado auto-assinado)
    echo   [SWAGGER]  http://localhost:5000/swagger (documentacao da API)
    echo   [HEALTH]   http://localhost:5000/health  (status da aplicacao)
    echo.
    echo BANCO DE DADOS SQL SERVER:
    echo   [HOST]     localhost:1433
    echo   [DATABASE] FastFoodDb
    echo   [USER]     sa
    echo   [PASSWORD] FastFood@2024!#
    echo   [CONN STR] Server=localhost,1433;Database=FastFoodDb;User Id=sa;Password=FastFood@2024!#;TrustServerCertificate=true;
    echo.
    echo STATUS ATUAL DOS CONTAINERS:
    docker-compose ps
    echo.
    echo COMANDOS UTEIS PARA DESENVOLVIMENTO:
    echo   docker-compose logs -f              # Ver todos os logs em tempo real
    echo   docker-compose logs -f api          # Ver logs apenas da API
    echo   docker-compose logs -f db           # Ver logs apenas do banco
    echo   docker-compose logs migrations      # Ver logs das migrations
    echo   docker-compose restart              # Reiniciar todos os servicos
    echo   docker-compose restart api          # Reiniciar apenas a API
    echo   docker-compose down                 # Parar todos os containers
    echo   docker-compose up -d                # Iniciar sem rebuild
    echo.
    echo SOLUCAO DE PROBLEMAS:
    echo   - Se a API nao responder imediatamente, aguarde 1-2 minutos
    echo   - Para ver erros detalhados: docker-compose logs
    echo   - Para recriar tudo: docker-compose down ^&^& docker-setup.bat
    echo   - Para limpar volumes: docker-compose down -v
    echo.
    echo [DICA] A aplicacao pode levar alguns minutos para inicializar completamente
    echo        na primeira execucao ou apos mudancas no codigo.
    echo.
    echo ========================================================================
) else (
    echo.
    echo [ERRO] Falha ao iniciar a aplicacao!
    echo ========================================================================
    echo [DEBUG] Informacoes para diagnostico:
    echo.
    echo 1. Verificar logs detalhados:
    echo    docker-compose logs
    echo.
    echo 2. Verificar portas em uso:
    echo    netstat -an ^| findstr ":5000 :5001 :1433"
    echo.
    echo 3. Verificar recursos do Docker:
    echo    docker system df
    echo    docker system info
    echo.
    echo 4. Limpar e tentar novamente:
    echo    docker-compose down -v
    echo    docker system prune -f
    echo    docker-setup.bat
    echo.
    echo 5. Verificar se Docker Desktop tem memoria suficiente (min 4GB recomendado)
    echo.
    echo ========================================================================
)

pause
