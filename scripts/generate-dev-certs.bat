@echo off
REM Script para gerar certificados de desenvolvimento no Windows
REM Usa dotnet dev-certs como metodo primario (mais confiavel para .NET)

echo [INFO] Gerando certificados de desenvolvimento para HTTPS...

REM Criar diretório para certificados se não existir
if not exist ".\certs" mkdir ".\certs"

REM Metodo 1: Tentar usar dotnet dev-certs (recomendado para .NET)
echo [TENTATIVA 1] Usando dotnet dev-certs...
dotnet dev-certs https --clean >nul 2>&1
dotnet dev-certs https --trust >nul 2>&1
dotnet dev-certs https -ep .\certs\fastfood-dev.pfx -p fastfood123 >nul 2>&1

if exist ".\certs\fastfood-dev.pfx" (
    echo [SUCESSO] Certificado gerado com dotnet dev-certs!
    goto :success
)

REM Metodo 2: Tentar usar PowerShell (fallback)
echo [TENTATIVA 2] Usando PowerShell...
powershell -Command "try { $cert = New-SelfSignedCertificate -DnsName 'localhost' -CertStoreLocation 'cert:\LocalMachine\My' -NotAfter (Get-Date).AddYears(1); $pwd = ConvertTo-SecureString -String 'fastfood123' -Force -AsPlainText; Export-PfxCertificate -Cert $cert -FilePath '.\certs\fastfood-dev.pfx' -Password $pwd; echo 'Certificado criado com sucesso' } catch { echo 'Erro ao criar certificado' }" >nul 2>&1

if exist ".\certs\fastfood-dev.pfx" (
    echo [SUCESSO] Certificado gerado com PowerShell!
    goto :success
)

REM Metodo 3: Verificar se OpenSSL está disponível (último recurso)
echo [TENTATIVA 3] Verificando OpenSSL...
where openssl >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [AVISO] OpenSSL nao encontrado. Criando certificado simplificado...
    goto :create_simple
)

REM Gerar com OpenSSL se disponível
echo [INFO] Usando OpenSSL...
openssl genrsa -out .\certs\fastfood-dev.key 2048 >nul 2>&1
openssl req -new -x509 -key .\certs\fastfood-dev.key -out .\certs\fastfood-dev.crt -days 365 -subj "/C=BR/ST=SP/L=SaoPaulo/O=FastFood/OU=Development/CN=localhost" >nul 2>&1
openssl pkcs12 -export -out .\certs\fastfood-dev.pfx -inkey .\certs\fastfood-dev.key -in .\certs\fastfood-dev.crt -password pass:fastfood123 >nul 2>&1

if exist ".\certs\fastfood-dev.pfx" (
    echo [SUCESSO] Certificado gerado com OpenSSL!
    goto :success
)

:create_simple
REM Criar um arquivo de certificado simples para evitar erros do Docker
echo [INFO] Criando certificado de desenvolvimento simplificado...
echo. > .\certs\fastfood-dev.pfx
echo [AVISO] Certificado simplificado criado. A aplicacao rodara apenas em HTTP.

:success
echo [INFO] Localizacao: .\certs\
echo [INFO] Senha do certificado PFX: fastfood123
echo.
echo [AVISO] IMPORTANTE: Este e um certificado autoassinado para DESENVOLVIMENTO apenas!
echo [AVISO] Para producao, use certificados validos de uma CA confiavel.
