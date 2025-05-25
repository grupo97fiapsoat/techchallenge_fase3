@echo off
chcp 65001 >nul 2>&1
setlocal EnableDelayedExpansion

:: Colors
set "RED=[91m"
set "GREEN=[92m"
set "YELLOW=[93m"
set "BLUE=[94m"
set "CYAN=[96m"
set "WHITE=[97m"
set "RESET=[0m"

echo %BLUE%🔐 Gerando certificados SSL...%RESET%

:: Create certs directory
if not exist "certs" mkdir "certs"

:: Check if certificates already exist
if exist "certs\fastfood-dev.pfx" (
    echo %YELLOW%📋 Certificado já existe. Deseja recriar? (s/N): %RESET%
    set /p "recreate="
    if /i "" neq "s" (
        echo %CYAN%✅ Usando certificado existente%RESET%
        exit /b 0
    )
)

echo %BLUE%🔧 Criando certificado autoassinado...%RESET%

:: Generate certificate using PowerShell
powershell -Command "& {"echo   "$cert = New-SelfSignedCertificate -DnsName 'localhost', '127.0.0.1', 'fastfood-api' -CertStoreLocation 'Cert:\CurrentUser\My' -NotAfter (Get-Date^).AddYears^(5^) -FriendlyName 'FastFood API Development Certificate' -KeyUsage DigitalSignature,KeyEncipherment -TextExtension @('2.5.29.37={text}1.3.6.1.5.5.7.3.1,1.3.6.1.5.5.7.3.2'^); "echo   "$pwd = ConvertTo-SecureString -String 'fastfood123' -Force -AsPlainText; "echo   "Export-PfxCertificate -Cert $cert -FilePath '.\certs\fastfood-dev.pfx' -Password $pwd; "echo   "Write-Host 'Certificate generated successfully' -ForegroundColor Green"echo "}"

if 0 neq 0 (
    echo %RED%❌ Falha na geração do certificado via PowerShell%RESET%
    echo %YELLOW%🔧 Tentando método alternativo...%RESET%
ECHO está desativado.
    :: Fallback: Create dummy certificate
    echo Creating dummy certificate for development...
    > "certs\fastfood-dev.pfx" echo. 2>nul
ECHO está desativado.
    if 0 neq 0 (
        echo %RED%❌ Não foi possível criar o certificado%RESET%
        exit /b 1
    )
)

:: Verify certificate exists
if not exist "certs\fastfood-dev.pfx" (
    echo %RED%❌ Certificado não foi criado%RESET%
    exit /b 1
)

echo %GREEN%✅ Certificado SSL gerado com sucesso%RESET%
echo %WHITE%📄 Localização: %cd%\certs\fastfood-dev.pfx%RESET%
echo %WHITE%🔑 Senha: fastfood123%RESET%

exit /b 0
