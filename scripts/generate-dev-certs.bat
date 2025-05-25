@echo off
REM Script para gerar certificados de desenvolvimento no Windows
REM Requer OpenSSL instalado (pode usar Git Bash ou WSL)

echo 🔐 Gerando certificados de desenvolvimento para HTTPS...

REM Criar diretório para certificados se não existir
if not exist ".\certs" mkdir ".\certs"

REM Verificar se OpenSSL está disponível
where openssl >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ❌ OpenSSL não encontrado!
    echo 💡 Instale o OpenSSL ou use o WSL Ubuntu para executar o script generate-dev-certs.sh
    echo 💡 Alternativa: Use o Git Bash que inclui OpenSSL
    pause
    exit /b 1
)

REM Gerar chave privada
openssl genrsa -out .\certs\fastfood-dev.key 2048

REM Gerar certificado autoassinado
openssl req -new -x509 -key .\certs\fastfood-dev.key -out .\certs\fastfood-dev.crt -days 365 -subj "/C=BR/ST=SP/L=SaoPaulo/O=FastFood/OU=Development/CN=localhost"

REM Gerar arquivo PFX para .NET
openssl pkcs12 -export -out .\certs\fastfood-dev.pfx -inkey .\certs\fastfood-dev.key -in .\certs\fastfood-dev.crt -password pass:fastfood123

echo ✅ Certificados gerados com sucesso!
echo 📁 Localização: .\certs\
echo 🔑 Senha do certificado PFX: fastfood123
echo.
echo ⚠️  IMPORTANTE: Este é um certificado autoassinado para DESENVOLVIMENTO apenas!
echo    Para produção, use certificados válidos de uma CA confiável.
pause
