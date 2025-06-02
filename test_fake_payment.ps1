# Script para testar o sistema de pagamento fake
Write-Host "=== TESTE DO SISTEMA DE PAGAMENTO FAKE ===" -ForegroundColor Green

# Simulação do problema
Write-Host "`n1. PROBLEMA IDENTIFICADO:" -ForegroundColor Yellow
Write-Host "- O FakePaymentService.GenerateQrCodeAsync() gera um QR code com GUID aleatório" -ForegroundColor White
Write-Host "- Formato: 'https://sandbox.mercadopago.com.br/checkout/v1/redirect?pref_id=FAKE-{Guid.NewGuid()}'" -ForegroundColor White
Write-Host "- Cada chamada gera um GUID diferente, mesmo para o mesmo pedido" -ForegroundColor Red

Write-Host "`n2. FLUXO ATUAL (PROBLEMÁTICO):" -ForegroundColor Yellow
$orderId = [System.Guid]::NewGuid()
Write-Host "- OrderId: $orderId" -ForegroundColor White

# Simula primeira chamada (checkout)
$guid1 = [System.Guid]::NewGuid()
$qrCode1 = "https://sandbox.mercadopago.com.br/checkout/v1/redirect?pref_id=FAKE-$guid1"
Write-Host "- Checkout - QR Code gerado: $qrCode1" -ForegroundColor Green
Write-Host "- QR Code salvo no banco: $qrCode1" -ForegroundColor Green

# Simula segunda chamada (se fosse chamado novamente)
$guid2 = [System.Guid]::NewGuid()
$qrCode2 = "https://sandbox.mercadopago.com.br/checkout/v1/redirect?pref_id=FAKE-$guid2"
Write-Host "- Se chamado novamente - QR Code seria: $qrCode2" -ForegroundColor Red

Write-Host "`n3. VALIDAÇÃO:" -ForegroundColor Yellow
Write-Host "- QR Code no banco: $qrCode1" -ForegroundColor White
Write-Host "- QR Code enviado pelo usuário: $qrCode1" -ForegroundColor White
$isEqual = $qrCode1 -eq $qrCode1
Write-Host "- São iguais? $isEqual" -ForegroundColor $(if($isEqual) { "Green" } else { "Red" })

Write-Host "`n4. SOLUÇÃO NECESSÁRIA:" -ForegroundColor Yellow
Write-Host "- O sistema deve funcionar corretamente quando o usuário envia o QR code correto" -ForegroundColor White
Write-Host "- O problema pode estar em:" -ForegroundColor White
Write-Host "  1. QR code não sendo salvo corretamente no banco" -ForegroundColor White
Write-Host "  2. QR code não sendo recuperado corretamente do banco" -ForegroundColor White
Write-Host "  3. Problema de encoding/comparação de strings" -ForegroundColor White
Write-Host "  4. Transação não sendo commitada no banco" -ForegroundColor White

Write-Host "`n=== LOGS DETALHADOS ADICIONADOS ===" -ForegroundColor Green
Write-Host "Foram adicionados logs detalhados no FakePaymentService.ProcessPaymentAsync() para debug" -ForegroundColor White
