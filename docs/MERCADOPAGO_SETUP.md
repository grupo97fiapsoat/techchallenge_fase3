# Configuração do Mercado Pago

Este projeto inclui integração com o **Mercado Pago** para processamento real de pagamentos, substituindo o sistema fake de desenvolvimento.

## 🔧 Configuração

### 1. Obter Credenciais do Mercado Pago

1. Acesse [developers.mercadopago.com](https://developers.mercadopago.com/)
2. Faça login na sua conta Mercado Pago
3. Acesse **"Suas aplicações"** → **"Criar aplicação"**
4. Preencha os dados da aplicação
5. Copie o **Access Token** (use o de teste para desenvolvimento)

### 2. Configurar no appsettings.json

Atualize as configurações no arquivo `src/FastFood.Api/appsettings.json`:

```json
{
  "MercadoPago": {
    "AccessToken": "TEST-4522852742974267-060115-f8d54a09ccaabcc5d21e2f8e66b02630-214883812",
    "PublicKey": "TEST-a8287905-8ae0-4102-a20c-3db7b063fbee",
    "NotificationUrl": "https://seudominio.com/api/webhook/mercadopago",
    "CallbackUrls": {
      "Success": "https://seudominio.com/payment/success",
      "Failure": "https://seudominio.com/payment/failure",
      "Pending": "https://seudominio.com/payment/pending"
    },
    "Environment": "sandbox"
  },
  "UseFakePayment": false
}
```

### 3. URLs Importantes

- **Webhook URL**: `https://seudominio.com/api/webhook/mercadopago`
  - Endpoint que recebe notificações de pagamento do Mercado Pago
  - Configure esta URL no painel do Mercado Pago

- **URLs de Retorno**:
  - Success: Para onde o cliente é redirecionado após pagamento aprovado
  - Failure: Para onde o cliente é redirecionado após pagamento rejeitado
  - Pending: Para onde o cliente é redirecionado quando pagamento fica pendente

## 🔄 Alternar entre Fake e Real

### Modo Desenvolvimento (Fake Payment)
```json
{
  "UseFakePayment": true
}
```

### Modo Produção (Mercado Pago Real)
```json
{
  "UseFakePayment": false,
  "MercadoPago": {
    "AccessToken": "PROD-ACCESS_TOKEN_AQUI",
    "Environment": "production"
  }
}
```

## 🛠️ Como Funciona

### 1. Geração de QR Code
- API chama `IPaymentService.GenerateQrCodeAsync()`
- Se fake: retorna QR Code simulado
- Se real: cria preferência no Mercado Pago e retorna link de pagamento

### 2. Processamento de Pagamento
- **Fake**: Simula aprovação automática
- **Real**: Recebe webhook do Mercado Pago com status real do pagamento

### 3. Webhook do Mercado Pago
- Endpoint: `POST /api/webhook/mercadopago`
- Recebe notificações em tempo real sobre mudanças no status do pagamento
- Atualiza automaticamente o status do pedido

## 📋 Endpoints da API

### Pagamentos
- `POST /api/orders/{id}/checkout` - Gera QR Code para pagamento
- `POST /api/webhook/mercadopago` - Recebe notificações do Mercado Pago
- `GET /api/webhook/test` - Testa se webhook está funcionando

## 🧪 Testes

### Teste com Cartões de Teste (Sandbox)
O Mercado Pago fornece cartões de teste para diferentes cenários:

**Aprovado**:
- Visa: 4509 9535 6623 3704
- Mastercard: 5031 7557 3453 0604

**Rejeitado**:
- Visa: 4000 0000 0000 0002
- Qualquer CVV: 123
- Qualquer data futura

### Verificar Funcionamento
1. Faça um pedido pela API
2. Use o link retornado no QR Code
3. Pague com cartão de teste
4. Verifique se o webhook foi chamado nos logs
5. Confirme se o status do pedido foi atualizado

## 🔐 Segurança

### Validação de Webhook
O webhook sempre retorna 200 OK para evitar reenvios desnecessários do Mercado Pago, mas processa os dados internamente.

### Configuração HTTPS
Em produção, certifique-se de que:
- Webhook URL usa HTTPS
- Certificado SSL válido
- URLs de callback usam HTTPS

## 📊 Monitoramento

### Logs
O serviço registra logs detalhados:
- Criação de preferências
- Recebimento de webhooks
- Processamento de pagamentos
- Erros e exceções

### Verificar Status
Use o painel do Mercado Pago para monitorar:
- Pagamentos processados
- Status das transações
- Webhooks enviados
- Taxas de aprovação

## 🚀 Deploy

### Variáveis de Ambiente
Configure as seguintes variáveis no ambiente de produção:

```bash
MERCADOPAGO__ACCESSTOKEN=seu_token_produção
MERCADOPAGO__NOTIFICATIONURL=https://api.seudominio.com/api/webhook/mercadopago
USEFAKEPAYMENT=false
```

### Docker
O projeto já está configurado com Docker. Certifique-se de definir as variáveis de ambiente no `docker-compose.yml` ou via configuração do orquestrador.

## 📞 Suporte

- [Documentação Oficial Mercado Pago](https://developers.mercadopago.com/)
- [SDK .NET do Mercado Pago](https://github.com/mercadopago/sdk-dotnet)
- [Suporte Técnico](https://developers.mercadopago.com/support/center)
