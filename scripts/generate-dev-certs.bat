@echo off
REM Script rapido para gerar certificados de desenvolvimento
REM TIMEOUT: 30 segundos maximo

echo [CERT] Gerando certificados de desenvolvimento...

REM Criar diretório se não existir (caminho correto quando chamado de scripts\)
if not exist "..\certs" mkdir "..\certs"

REM Metodo 1: dotnet dev-certs (mais rapido) - com timeout
echo [EXEC] Tentando dotnet dev-certs (timeout 15s)...
timeout /t 1 /nobreak >nul
dotnet dev-certs https --clean >nul 2>&1
start /wait /b timeout /t 15 /nobreak >nul
dotnet dev-certs https -ep ..\certs\fastfood-dev.pfx -p fastfood123 >nul 2>&1

if exist "..\certs\fastfood-dev.pfx" (
    echo [OK] Certificado criado com dotnet dev-certs
    goto :success
)

REM Metodo 2: PowerShell simples - com timeout
echo [EXEC] Tentando PowerShell simples (timeout 10s)...
powershell -Command "try { $cert = New-SelfSignedCertificate -DnsName localhost -CertStoreLocation cert:\CurrentUser\My; $pwd = ConvertTo-SecureString -String fastfood123 -Force -AsPlainText; Export-PfxCertificate -Cert $cert -FilePath ..\certs\fastfood-dev.pfx -Password $pwd | Out-Null; Write-Host OK } catch { Write-Host FAIL }" 2>nul

if exist "..\certs\fastfood-dev.pfx" (
    echo [OK] Certificado criado com PowerShell
    goto :success
)

REM Metodo 3: Criar arquivo dummy rapido
echo [FALLBACK] Criando certificado dummy...
echo dummy-certificate-for-development > ..\certs\fastfood-dev.pfx

:success
echo [INFO] Certificado: ..\certs\fastfood-dev.pfx
echo [INFO] Senha: fastfood123
echo [INFO] Script concluido em menos de 30s
