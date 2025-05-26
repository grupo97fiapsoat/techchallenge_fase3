# TASK.md - Controle de Tarefas

## Tarefas Pendentes

### ✅ CONCLUÍDO - Correção da Validação de CPF (26/05/2025)
**Descrição**: Corrigir problema na validação de CPF da API onde CPFs com 11 números estão sendo rejeitados. Problema identificado: duplicidade de validação entre FluentValidation e Value Object, conflito entre validação com/sem formatação.

**Problemas Identificados**:
- Validadores FluentValidation exigem exatamente 11 caracteres ANTES da normalização
- CPF formatado (123.456.789-01) tem 14 caracteres e falha na validação
- Duplicidade de validação entre CreateCustomerCommandValidator, GetCustomerByCpfQueryValidator e Value Object Cpf
- Regex CPF_FORMAT não está sendo usado corretamente

**Soluções Implementadas**:
- Removida validação de tamanho dos validadores FluentValidation
- Mantida apenas validação "NotEmpty" nos validadores
- Toda lógica de validação (normalização + cálculo de dígitos) centralizada no Value Object
- Value Object aceita CPF formatado e não formatado
- Regex aplicado antes da normalização no Value Object
- **CORREÇÃO ADICIONAL**: Simplificada regex CPF_FORMAT para aceitar formatação parcial
- Removida duplicação de normalização no CustomerRepository
- Corrigidas propriedades nullable no CreateCustomerDto

## Descobertos Durante o Trabalho

### 🚀 NOVA TAREFA - Adicionar Coluna QrCode na Tabela de Pedidos (26/05/2025)
**Descrição**: Persistir o QR Code gerado no banco de dados para resolver problema de perda de dados quando a aplicação reinicia.
**Problema Atual**: FakePaymentService armazena QR Codes em memória (Dictionary), perdendo dados ao reiniciar.
**Melhorias Propostas**:
- Adicionar coluna `QrCode` na entidade Order
- Migration para criar a nova coluna
- Atualizar ProcessCheckoutCommandHandler para salvar QR Code no pedido
- Atualizar ConfirmPaymentCommandHandler para buscar QR Code do banco
- Remover dependência do Dictionary em memória no FakePaymentService
- Adicionar validação de QR Code expirado (opcional)

### 🔄 EM PROGRESSO - Implementar Status "AwaitingPayment" (26/05/2025)
**Descrição**: Separar o fluxo de checkout para ter um status intermediário entre geração do QR Code e confirmação do pagamento.
**Problema Atual**: O checkout gera QR Code e imediatamente marca como "Paid", não refletindo a realidade de pagamentos via QR Code.
**Solução Proposta**: 
- Adicionar status `AwaitingPayment` (6)
- Checkout gera QR Code e muda status para `AwaitingPayment`
- Novo endpoint para confirmar pagamento (ou webhook) muda status para `Paid`
- Melhor separação de responsabilidades no fluxo de pagamento

### ✅ CONCLUÍDO - Regex muito restritiva (26/05/2025)
**Problema**: A regex original `@"^(\d{3}\.?\d{3}\.?\d{3}\-?\d{2}|\d{11})$"` era muito restritiva e não aceitava formatação parcial como "123.456.78901" ou "12345678-01".
**Solução**: Simplificada para `@"^[\d\.\-\s]{11,14}$"` que aceita 11-14 caracteres contendo apenas dígitos, pontos, hífen e espaços.

*Adicionar aqui novas subtarefas ou TODOs descobertos durante o desenvolvimento*
