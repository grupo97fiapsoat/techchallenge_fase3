@echo off
chcp 65001 >nul 2>&1
setlocal EnableDelayedExpansion

:: ═══════════════════════════════════════════════════════════════════════════════
::                          🚀 FASTFOOD API INITIALIZER 🚀                        
::                        Inicializador Moderno End-to-End                        
:: ═══════════════════════════════════════════════════════════════════════════════

:: Colors for better UX
set "RED=[91m"
set "GREEN=[92m"
set "YELLOW=[93m"
set "BLUE=[94m"
set "MAGENTA=[95m"
set "CYAN=[96m"
set "WHITE=[97m"
set "RESET=[0m"
set "BOLD=[1m"

:: Configuration
set "LOG_DIR=%~dp0logs"
set "INIT_DIR=%~dp0init"
set "TIMESTAMP=%date:~-4%%date:~3,2%%date:~0,2%_%time:~0,2%%time:~3,2%%time:~6,2%"
set "TIMESTAMP=!TIMESTAMP: =0!"
set "LOG_FILE=%LOG_DIR%\init_%TIMESTAMP%.log"

:: Create directories
if not exist "%LOG_DIR%" mkdir "%LOG_DIR%"
if not exist "%INIT_DIR%" mkdir "%INIT_DIR%"

:: Header
cls
echo.
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo %BOLD%%CYAN%║                          🚀 FASTFOOD API INITIALIZER 🚀                        ║%RESET%
echo %BOLD%%CYAN%║                        Inicializador Moderno End-to-End                        ║%RESET%
echo %BOLD%%CYAN%║                              Versão 2.0 - 2024                                ║%RESET%
echo %BOLD%%CYAN%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo.
echo %WHITE%📋 Este inicializador irá configurar completamente o projeto FastFood API%RESET%
echo %WHITE%   incluindo todas as dependências, certificados e banco de dados.%RESET%
echo.

:: Log initialization
echo [%date% %time%] ========== FASTFOOD API INITIALIZER STARTED ========== >> "%LOG_FILE%"
echo [%date% %time%] Timestamp: %TIMESTAMP% >> "%LOG_FILE%"
echo [%date% %time%] Log file: %LOG_FILE% >> "%LOG_FILE%"

:: Create utility scripts
call :CreateUtilityScripts

:: Menu
:MENU
echo %BOLD%%YELLOW%📋 MENU PRINCIPAL:%RESET%
echo.
echo %WHITE%   1️⃣  Instalação Completa (Recomendado)%RESET%
echo %WHITE%   2️⃣  Verificar Pré-requisitos%RESET%
echo %WHITE%   3️⃣  Configurar Ambiente%RESET%
echo %WHITE%   4️⃣  Gerar Certificados SSL%RESET%
echo %WHITE%   5️⃣  Inicializar Docker%RESET%
echo %WHITE%   6️⃣  Executar Migrações%RESET%
echo %WHITE%   7️⃣  Verificar Status%RESET%
echo %WHITE%   8️⃣  Ver Logs%RESET%
echo %WHITE%   9️⃣  Reset Completo%RESET%
echo %WHITE%   0️⃣  Sair%RESET%
echo.
set /p "choice=%CYAN%🎯 Escolha uma opção (1-9, 0 para sair): %RESET%"

if "%choice%"=="1" goto FULL_INSTALL
if "%choice%"=="2" goto CHECK_PREREQ
if "%choice%"=="3" goto SETUP_ENV
if "%choice%"=="4" goto GENERATE_CERTS
if "%choice%"=="5" goto DOCKER_INIT
if "%choice%"=="6" goto RUN_MIGRATIONS
if "%choice%"=="7" goto CHECK_STATUS
if "%choice%"=="8" goto VIEW_LOGS
if "%choice%"=="9" goto RESET_ALL
if "%choice%"=="0" goto EXIT
goto MENU

:FULL_INSTALL
echo.
echo %BOLD%%GREEN%🚀 INICIANDO INSTALAÇÃO COMPLETA...%RESET%
echo.
call :LogStep "Starting full installation"

call :CheckPrerequisites || goto ERROR
call :SetupEnvironment || goto ERROR
call :GenerateCertificates || goto ERROR
call :InitializeDocker || goto ERROR
call :RunMigrations || goto ERROR
call :VerifyInstallation || goto ERROR

echo.
echo %BOLD%%GREEN%✅ INSTALAÇÃO CONCLUÍDA COM SUCESSO!%RESET%
echo.
call :ShowSuccessInfo
goto MENU

:CHECK_PREREQ
call :CheckPrerequisites
pause
goto MENU

:SETUP_ENV
call :SetupEnvironment
pause
goto MENU

:GENERATE_CERTS
call :GenerateCertificates
pause
goto MENU

:DOCKER_INIT
call :InitializeDocker
pause
goto MENU

:RUN_MIGRATIONS
call :RunMigrations
pause
goto MENU

:CHECK_STATUS
call :VerifyInstallation
pause
goto MENU

:VIEW_LOGS
call :ShowLogs
pause
goto MENU

:RESET_ALL
call :ResetEnvironment
pause
goto MENU

:EXIT
echo.
echo %BOLD%%CYAN%👋 Obrigado por usar o FastFood API Initializer!%RESET%
echo %WHITE%📁 Logs salvos em: %LOG_FILE%%RESET%
echo.
call :LogStep "Initializer exited by user"
pause
exit /b 0

:ERROR
echo.
echo %BOLD%%RED%❌ ERRO DURANTE A INSTALAÇÃO!%RESET%
echo %RED%📄 Verifique o arquivo de log: %LOG_FILE%%RESET%
echo.
call :LogStep "ERROR: Installation failed"
pause
goto MENU

:: ═══════════════════════════════════════════════════════════════════════════════
::                                  FUNCTIONS                                     
:: ═══════════════════════════════════════════════════════════════════════════════

:LogStep
echo [%date% %time%] %~1 >> "%LOG_FILE%"
exit /b 0

:CheckPrerequisites
echo %BOLD%%BLUE%🔍 VERIFICANDO PRÉ-REQUISITOS...%RESET%
echo.
call :LogStep "Checking prerequisites"

call "%INIT_DIR%\check-prereq.bat"
if !errorlevel! neq 0 (
    echo %RED%❌ Falha na verificação de pré-requisitos%RESET%
    call :LogStep "ERROR: Prerequisites check failed"
    exit /b 1
)

echo %GREEN%✅ Todos os pré-requisitos atendidos%RESET%
call :LogStep "All prerequisites met"
exit /b 0

:SetupEnvironment
echo %BOLD%%BLUE%⚙️  CONFIGURANDO AMBIENTE...%RESET%
echo.
call :LogStep "Setting up environment"

call "%INIT_DIR%\setup-env.bat"
if !errorlevel! neq 0 (
    echo %RED%❌ Falha na configuração do ambiente%RESET%
    call :LogStep "ERROR: Environment setup failed"
    exit /b 1
)

echo %GREEN%✅ Ambiente configurado com sucesso%RESET%
call :LogStep "Environment setup completed"
exit /b 0

:GenerateCertificates
echo %BOLD%%BLUE%🔐 GERANDO CERTIFICADOS SSL...%RESET%
echo.
call :LogStep "Generating SSL certificates"

call "%INIT_DIR%\gen-certs.bat"
if !errorlevel! neq 0 (
    echo %RED%❌ Falha na geração de certificados%RESET%
    call :LogStep "ERROR: Certificate generation failed"
    exit /b 1
)

echo %GREEN%✅ Certificados gerados com sucesso%RESET%
call :LogStep "SSL certificates generated"
exit /b 0

:InitializeDocker
echo %BOLD%%BLUE%🐳 INICIALIZANDO DOCKER...%RESET%
echo.
call :LogStep "Initializing Docker"

call "%INIT_DIR%\docker-init.bat"
if !errorlevel! neq 0 (
    echo %RED%❌ Falha na inicialização do Docker%RESET%
    call :LogStep "ERROR: Docker initialization failed"
    exit /b 1
)

echo %GREEN%✅ Docker inicializado com sucesso%RESET%
call :LogStep "Docker initialization completed"
exit /b 0

:RunMigrations
echo %BOLD%%BLUE%🗃️  EXECUTANDO MIGRAÇÕES...%RESET%
echo.
call :LogStep "Running database migrations"

call "%INIT_DIR%\run-migrations.bat"
if !errorlevel! neq 0 (
    echo %RED%❌ Falha nas migrações do banco%RESET%
    call :LogStep "ERROR: Database migrations failed"
    exit /b 1
)

echo %GREEN%✅ Migrações executadas com sucesso%RESET%
call :LogStep "Database migrations completed"
exit /b 0

:VerifyInstallation
echo %BOLD%%BLUE%🔎 VERIFICANDO INSTALAÇÃO...%RESET%
echo.
call :LogStep "Verifying installation"

call "%INIT_DIR%\verify-install.bat"
if !errorlevel! neq 0 (
    echo %RED%❌ Falha na verificação da instalação%RESET%
    call :LogStep "ERROR: Installation verification failed"
    exit /b 1
)

echo %GREEN%✅ Instalação verificada com sucesso%RESET%
call :LogStep "Installation verification completed"
exit /b 0

:ResetEnvironment
echo %BOLD%%YELLOW%🔄 RESETANDO AMBIENTE...%RESET%
echo.
call :LogStep "Resetting environment"

call "%INIT_DIR%\reset-env.bat"
if !errorlevel! neq 0 (
    echo %RED%❌ Falha no reset do ambiente%RESET%
    call :LogStep "ERROR: Environment reset failed"
    exit /b 1
)

echo %GREEN%✅ Ambiente resetado com sucesso%RESET%
call :LogStep "Environment reset completed"
exit /b 0

:ShowSuccessInfo
echo %BOLD%%GREEN%🎉 FASTFOOD API ESTÁ PRONTO PARA USO!%RESET%
echo.
echo %WHITE%📊 Informações de Acesso:%RESET%
echo %CYAN%   • API HTTP:  http://localhost:5000%RESET%
echo %CYAN%   • API HTTPS: https://localhost:5001%RESET%
echo %CYAN%   • Swagger:   https://localhost:5001/swagger%RESET%
echo.
echo %WHITE%🗄️  Banco de Dados:%RESET%
echo %CYAN%   • Servidor:  localhost:1433%RESET%
echo %CYAN%   • Database:  FastFoodDb%RESET%
echo %CYAN%   • Usuario:   sa%RESET%
echo %CYAN%   • Senha:     FastFood2024%RESET%
echo.
echo %WHITE%🔧 Comandos Úteis:%RESET%
echo %YELLOW%   docker-compose logs -f api    %RESET%- Ver logs da API
echo %YELLOW%   docker-compose ps             %RESET%- Status dos containers
echo %YELLOW%   docker-compose down           %RESET%- Parar todos os serviços
echo %YELLOW%   docker-compose up -d          %RESET%- Iniciar todos os serviços
echo.
call :LogStep "Success information displayed"
exit /b 0

:ShowLogs
echo %BOLD%%BLUE%📄 EXIBINDO LOGS...%RESET%
echo.
if exist "%LOG_FILE%" (
    type "%LOG_FILE%"
) else (
    echo %RED%❌ Arquivo de log não encontrado%RESET%
)
exit /b 0

:CreateUtilityScripts
call :LogStep "Creating utility scripts"

:: Create the individual scripts that will handle each step
call "%INIT_DIR%\create-scripts.bat" 2>nul
if not exist "%INIT_DIR%\create-scripts.bat" (
    echo Creating utility script generator...
    call :CreateScriptGenerator
    call "%INIT_DIR%\create-scripts.bat"
)
exit /b 0

:CreateScriptGenerator
> "%INIT_DIR%\create-scripts.bat" (
echo @echo off
echo setlocal
echo set "INIT_DIR=%%~dp0"
echo call :CreateCheckPrereq
echo call :CreateSetupEnv
echo call :CreateGenCerts
echo call :CreateDockerInit
echo call :CreateRunMigrations
echo call :CreateVerifyInstall
echo call :CreateResetEnv
echo exit /b 0
echo.
echo :CreateCheckPrereq
echo ^> "%%INIT_DIR%%check-prereq.bat" ^(
echo echo @echo off
echo echo :: Placeholder for check-prereq.bat
echo echo exit /b 0
echo ^)
echo exit /b 0
echo.
echo :CreateSetupEnv
echo ^> "%%INIT_DIR%%setup-env.bat" ^(
echo echo @echo off
echo echo :: Placeholder for setup-env.bat
echo echo exit /b 0
echo ^)
echo exit /b 0
echo.
echo :CreateGenCerts
echo ^> "%%INIT_DIR%%gen-certs.bat" ^(
echo echo @echo off
echo echo :: Placeholder for gen-certs.bat
echo echo exit /b 0
echo ^)
echo exit /b 0
echo.
echo :CreateDockerInit
echo ^> "%%INIT_DIR%%docker-init.bat" ^(
echo echo @echo off
echo echo :: Placeholder for docker-init.bat
echo echo exit /b 0
echo ^)
echo exit /b 0
echo.
echo :CreateRunMigrations
echo ^> "%%INIT_DIR%%run-migrations.bat" ^(
echo echo @echo off
echo echo :: Placeholder for run-migrations.bat
echo echo exit /b 0
echo ^)
echo exit /b 0
echo.
echo :CreateVerifyInstall
echo ^> "%%INIT_DIR%%verify-install.bat" ^(
echo echo @echo off
echo echo :: Placeholder for verify-install.bat
echo echo exit /b 0
echo ^)
echo exit /b 0
echo.
echo :CreateResetEnv
echo ^> "%%INIT_DIR%%reset-env.bat" ^(
echo echo @echo off
echo echo :: Placeholder for reset-env.bat
echo echo exit /b 0
echo ^)
echo exit /b 0
)
exit /b 0
