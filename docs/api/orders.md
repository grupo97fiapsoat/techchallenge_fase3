# API de Pedidos - FastFood

## Fluxo de Pedidos com Pagamento via QR Code

### Status dos Pedidos

O sistema de pedidos agora possui os seguintes status:

- **Pending** (0) - Pedido criado, aguardando checkout
- **AwaitingPayment** (6) - QR Code gerado, aguardando confirmação de pagamento
- **Paid** (5) - Pagamento confirmado
- **Processing** (1) - Pedido em preparo na cozinha
- **Ready** (2) - Pedido pronto para retirada
- **Completed** (3) - Pedido entregue ao cliente
- **Cancelled** (4) - Pedido cancelado

### Fluxo Principal

#### 1. Criar Pedido
```http
POST /api/v1/orders
```
- Cria um novo pedido com status `Pending`

#### 2. Gerar QR Code para Pagamento
```http
POST /api/v1/orders/{id}/checkout
```
- Gera QR Code para pagamento
- Atualiza status para `AwaitingPayment`
- Retorna QR Code para o cliente

#### 3. Confirmar Pagamento
```http
POST /api/v1/orders/{id}/confirm-payment
```
- Confirma o pagamento usando o QR Code
- Atualiza status para `Paid`
- Cliente pode usar apps de pagamento para pagar o QR Code

#### 4. Processar na Cozinha
```http
PUT /api/v1/orders/{id}/status
```
- Funcionário atualiza status para `Processing`
- Status muda para `Ready` quando terminar o preparo
- Status muda para `Completed` quando cliente retirar

### Transições de Status Válidas

```
Pending → AwaitingPayment  (checkout gera QR Code)
Pending → Cancelled        (cancelamento antes do pagamento)

AwaitingPayment → Paid      (confirmação de pagamento)
AwaitingPayment → Cancelled (cancelamento durante espera)

Paid → Processing          (envio para cozinha)

Processing → Ready         (pedido pronto)
Processing → Cancelled     (cancelamento durante preparo)
Processing → Pending       (problemas na cozinha)

Ready → Completed          (entrega ao cliente)

* → * (mesmo status - idempotência)
```

### Exemplos de Uso

#### Exemplo 1: Fluxo Completo de Sucesso

```bash
# 1. Criar pedido
POST /api/v1/orders
{
  "customerId": "123e4567-e89b-12d3-a456-426614174000",
  "items": [...]
}
# Status: Pending

# 2. Gerar QR Code
POST /api/v1/orders/123e4567-e89b-12d3-a456-426614174000/checkout
# Status: AwaitingPayment
# Retorna: QR Code

# 3. Cliente paga usando QR Code (simulado)
POST /api/v1/orders/123e4567-e89b-12d3-a456-426614174000/confirm-payment
{
  "qrCode": "QR_CODE_123_ABCDEF"
}
# Status: Paid

# 4. Funcionário envia para cozinha
PUT /api/v1/orders/123e4567-e89b-12d3-a456-426614174000/status
{
  "status": "Processing"
}
# Status: Processing

# 5. Pedido fica pronto
PUT /api/v1/orders/123e4567-e89b-12d3-a456-426614174000/status
{
  "status": "Ready"
}
# Status: Ready
# Notificação enviada ao cliente

# 6. Cliente retira pedido
PUT /api/v1/orders/123e4567-e89b-12d3-a456-426614174000/status
{
  "status": "Completed"
}
# Status: Completed
```

#### Exemplo 2: Cancelamento Durante Espera de Pagamento

```bash
# 1. Criar pedido
POST /api/v1/orders
# Status: Pending

# 2. Gerar QR Code
POST /api/v1/orders/{id}/checkout
# Status: AwaitingPayment

# 3. Cliente desiste
PUT /api/v1/orders/{id}/status
{
  "status": "Cancelled"
}
# Status: Cancelled
```

### Notificações

O sistema envia notificações automáticas:

- **Mudança de Status**: Para todas as mudanças de status
- **Pedido Pronto**: Notificação especial quando status vira `Ready`

### Retrocompatibilidade

O sistema mantém compatibilidade com o fluxo antigo onde era possível ir diretamente de `Pending` para `Paid`, mas o novo fluxo com `AwaitingPayment` é o recomendado para novos clientes.
