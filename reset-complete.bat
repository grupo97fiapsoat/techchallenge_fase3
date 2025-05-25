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
echo %BOLD%%RED%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo %BOLD%%RED%║                        🔥 RESET COMPLETO DO AMBIENTE                          ║%RESET%
echo %BOLD%%RED%║                    Limpeza Total e Configuração do Zero                       ║%RESET%
echo %BOLD%%RED%═══════════════════════════════════════════════════════════════════════════════%RESET%
echo.

echo %BOLD%%YELLOW%⚠️  ATENÇÃO: Este script irá:%RESET%
echo %WHITE%   • Parar todos os containers Docker do projeto%RESET%
echo %WHITE%   • Remover containers, volumes e imagens%RESET%
echo %WHITE%   • Limpar cache e networks%RESET%
echo %WHITE%   • Configurar nova porta para SQL Server (1434)%RESET%
echo %WHITE%   • Recriar tudo do zero%RESET%
echo.

set /p "confirm=Deseja continuar? (S/N): "
if /i not "%confirm%"=="S" (
    echo %RED%Operação cancelada pelo usuário.%RESET%
    pause
    exit /b 1
)

echo.
echo %BOLD%%BLUE%[ETAPA 1] Parando containers ativos...%RESET%
docker-compose down --remove-orphans
if errorlevel 1 (
    echo %YELLOW%Aviso: Alguns containers podem não estar rodando.%RESET%
)

echo.
echo %BOLD%%BLUE%[ETAPA 2] Removendo containers relacionados...%RESET%
docker ps -aq --filter "name=techchallenge_fase1" > nul 2>&1 && (
    for /f %%i in ('docker ps -aq --filter "name=techchallenge_fase1"') do (
        echo Removendo container: %%i
        docker rm -f %%i > nul 2>&1
    )
) || echo %CYAN%Nenhum container encontrado.%RESET%

echo.
echo %BOLD%%BLUE%[ETAPA 3] Removendo volumes...%RESET%
docker volume ls -q --filter "name=techchallenge_fase1" > nul 2>&1 && (
    for /f %%i in ('docker volume ls -q --filter "name=techchallenge_fase1"') do (
        echo Removendo volume: %%i
        docker volume rm %%i > nul 2>&1
    )
) || echo %CYAN%Nenhum volume encontrado.%RESET%

echo.
echo %BOLD%%BLUE%[ETAPA 4] Removendo redes...%RESET%
docker network ls -q --filter "name=techchallenge_fase1" > nul 2>&1 && (
    for /f %%i in ('docker network ls -q --filter "name=techchallenge_fase1"') do (
        echo Removendo rede: %%i
        docker network rm %%i > nul 2>&1
    )
) || echo %CYAN%Nenhuma rede encontrada.%RESET%

echo.
echo %BOLD%%BLUE%[ETAPA 5] Removendo imagens do projeto...%RESET%
docker images -q --filter "reference=*fastfood*" > nul 2>&1 && (
    for /f %%i in ('docker images -q --filter "reference=*fastfood*"') do (
        echo Removendo imagem: %%i
        docker rmi -f %%i > nul 2>&1
    )
) || echo %CYAN%Nenhuma imagem encontrada.%RESET%

echo.
echo %BOLD%%BLUE%[ETAPA 6] Limpeza geral do Docker...%RESET%
docker system prune -f > nul 2>&1
echo %GREEN%Sistema Docker limpo.%RESET%

echo.
echo %BOLD%%BLUE%[ETAPA 7] Configurando nova porta (1434)...%RESET%

:: Backup do docker-compose.yml original
if exist docker-compose.yml.bak (
    echo %CYAN%Backup já existe, restaurando original...%RESET%
    copy docker-compose.yml.bak docker-compose.yml > nul
) else (
    echo %CYAN%Criando backup do docker-compose.yml...%RESET%
    copy docker-compose.yml docker-compose.yml.bak > nul
)

:: Modificar a porta no docker-compose.yml
powershell -Command "(Get-Content docker-compose.yml) -replace '1433:1433', '1434:1433' | Set-Content docker-compose.yml"

echo.
echo %BOLD%%BLUE%[ETAPA 8] Atualizando string de conexão (.env)...%RESET%

:: Atualizar a porta na string de conexão
if exist .env (
    powershell -Command "(Get-Content .env) -replace 'localhost,1433', 'localhost,1434' | Set-Content .env"
    echo %GREEN%String de conexão atualizada para porta 1434.%RESET%
) else (
    echo %YELLOW%Arquivo .env não encontrado. Será criado durante a inicialização.%RESET%
)

echo.
echo %BOLD%%GREEN%✅ RESET COMPLETO FINALIZADO!%RESET%
echo.
echo %BOLD%%CYAN%📋 NOVA CONFIGURAÇÃO:%RESET%
echo %WHITE%   • SQL Server rodará na porta: %CYAN%1434%RESET%
echo %WHITE%   • String de conexão: %CYAN%localhost,1434%RESET%
echo %WHITE%   • Usuário: %CYAN%sa%RESET%
echo %WHITE%   • Senha: %CYAN%FastFood2024%RESET%
echo %WHITE%   • Database: %CYAN%FastFoodDb%RESET%
echo.

echo %BOLD%%YELLOW%🚀 PRÓXIMOS PASSOS:%RESET%
echo %WHITE%   1. Execute: %CYAN%init.bat%RESET%
echo %WHITE%   2. Escolha a opção de instalação completa%RESET%
echo %WHITE%   3. Para SSMS, use a porta %CYAN%1434%RESET% ao conectar%RESET%
echo.

echo %BOLD%%BLUE%💡 VERIFICAR PORTA:%RESET%
echo %CYAN%netstat -ano | findstr :1434%RESET%
echo.

pause
exit /b 0
