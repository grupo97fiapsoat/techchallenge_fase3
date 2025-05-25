@echo off
REM Script rapido para gerar certificados de desenvolvimento

echo [CERT] Gerando certificados...

REM Criar diretório se não existir
if not exist ".\certs" mkdir ".\certs"

REM Metodo 1: dotnet dev-certs (mais rapido)
echo [EXEC] Usando dotnet dev-certs...
dotnet dev-certs https --clean >nul 2>&1
dotnet dev-certs https -ep .\certs\fastfood-dev.pfx -p fastfood123 >nul 2>&1

if exist ".\certs\fastfood-dev.pfx" (
    echo [OK] Certificado criado com dotnet dev-certs
    goto :success
)

REM Metodo 2: PowerShell rapido
echo [EXEC] Tentando PowerShell...
powershell -Command "try { $cert = New-SelfSignedCertificate -DnsName 'localhost' -CertStoreLocation 'cert:\CurrentUser\My' -NotAfter (Get-Date).AddMonths(12); $pwd = ConvertTo-SecureString -String 'fastfood123' -Force -AsPlainText; Export-PfxCertificate -Cert $cert -FilePath '.\certs\fastfood-dev.pfx' -Password $pwd | Out-Null; Write-Host '[OK] PowerShell cert created' } catch { Write-Host '[FAIL] PowerShell failed' }" >nul 2>&1

if exist ".\certs\fastfood-dev.pfx" (
    echo [OK] Certificado criado com PowerShell
    goto :success
)

REM Metodo 3: Criar arquivo dummy
echo [FALLBACK] Criando certificado dummy...
echo dummy-certificate-content > .\certs\fastfood-dev.pfx

:success
echo [INFO] Certificado: .\certs\fastfood-dev.pfx
echo [INFO] Senha: fastfood123
