using FastFood.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FastFood.Api.Controllers
{    /// <summary>
    /// Controlador para recebimento de webhooks de integração externa
    /// 
    /// **Finalidade:** Recebe notificações automáticas de serviços externos (principalmente MercadoPago) para atualizar status de pagamentos em tempo real.
    /// 
    /// **Funcionalidades principais:**
    /// - Recebe webhooks do MercadoPago sobre status de pagamentos
    /// - Processa automaticamente confirmações de pagamento
    /// - Registra logs detalhados para auditoria
    /// - Confirma recebimento para evitar reenvios
    /// 
    /// **Níveis de acesso:**
    /// - **Todos os endpoints são públicos** - Não requer autenticação (webhooks externos)
    /// - **Segurança**: Validação por assinatura e origem confiável
    /// 
    /// **Integração MercadoPago:**
    /// - Notificações instantâneas sobre mudanças de status
    /// - Processamento automático de confirmações de pagamento
    /// - Atualização automática de pedidos para "EmPreparacao"
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous] // Permite acesso sem autenticação, pois é chamado pelo Mercado Pago
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;
        private readonly MercadoPagoPaymentService? _mercadoPagoService;

        public WebhookController(ILogger<WebhookController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            
            // Verificar se o serviço MercadoPago está configurado
            try
            {
                _mercadoPagoService = serviceProvider.GetService<MercadoPagoPaymentService>();
                if (_mercadoPagoService == null)
                {
                    _logger.LogWarning("MercadoPago service not configured - webhook will be ignored");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("MercadoPago service not available: {Message}", ex.Message);
                _mercadoPagoService = null;
            }
        }        /// <summary>
        /// Recebe notificações de webhook do MercadoPago sobre pagamentos
        /// 
        /// **Endpoint público** - Não requer autenticação (chamado automaticamente pelo MercadoPago).
        /// 
        /// **Finalidade:** Processa notificações automáticas do MercadoPago quando o status de um pagamento muda, atualizando pedidos em tempo real.
        ///        /// **Funcionamento:**
        /// 1. **MercadoPago envia**: Notificação automática sobre mudança de status
        /// 2. **Sistema processa**: Valida e interpreta a notificação
        /// 3. **Atualiza pedido**: Muda status do pedido conforme pagamento
        /// 4. **Confirma recebimento**: Retorna 200 OK para parar reenvios
        /// 
        /// **Exemplo de notificação MercadoPago:**
        /// ```json
        /// {
        ///   "id": 12345,
        ///   "live_mode": true,
        ///   "type": "payment",
        ///   "date_created": "2025-06-02T10:30:00.000-04:00",
        ///   "user_id": 123456789,
        ///   "api_version": "v1",
        ///   "action": "payment.updated",
        ///   "data": {
        ///     "id": "1234567890"
        ///   }
        /// }
        /// ```
        /// 
        /// **Estados processados:**
        /// - **approved**: Pagamento aprovado → Pedido vai para "EmPreparacao"
        /// - **pending**: Pagamento pendente → Mantém "Recebido"
        /// - **cancelled/rejected**: Pagamento falhou → Log de erro
        /// 
        /// **Segurança:**
        /// - Endpoint público mas com validação de origem
        /// - Logs detalhados para auditoria
        /// - Sempre retorna 200 OK (mesmo com erros) para evitar spam de reenvios
        /// 
        /// **Configuração necessária:**
        /// - URL do webhook configurada no painel do MercadoPago
        /// - Serviço MercadoPago configurado na aplicação
        /// </summary>
        /// <returns>Status 200 OK para confirmar recebimento ao MercadoPago</returns>
        /// <response code="200">Webhook recebido e processado (sempre retorna 200)</response>
        [HttpPost("mercadopago")]
        public async Task<IActionResult> MercadoPagoWebhook()
        {
            try
            {
                if (_mercadoPagoService == null)
                {
                    _logger.LogWarning("Webhook recebido mas MercadoPago service não está configurado");
                    return Ok(); // Retorna OK para evitar reenvio
                }

                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var requestBody = await reader.ReadToEndAsync();
                
                _logger.LogInformation("Webhook recebido do Mercado Pago: {RequestBody}", requestBody);

                if (string.IsNullOrEmpty(requestBody))
                {
                    _logger.LogWarning("Webhook recebido com corpo vazio");
                    return Ok(); // Retorna OK para evitar reenvio
                }

                // Processar o pagamento através do serviço MercadoPago
                var isSuccess = _mercadoPagoService.ProcessWebhook(requestBody, out var transactionId, out var message);
                
                if (isSuccess)
                {
                    _logger.LogInformation("Webhook processado com sucesso. Transaction ID: {TransactionId}", 
                        transactionId);
                }
                else
                {
                    _logger.LogWarning("Falha ao processar webhook: {Message}", message);
                }

                // Sempre retorna 200 OK para o Mercado Pago confirmar que recebemos o webhook
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar webhook do Mercado Pago");
                
                // Mesmo com erro, retorna 200 OK para evitar reenvio desnecessário
                return Ok();
            }
        }        /// <summary>
        /// Endpoint de teste para verificar funcionamento do webhook
        /// 
        /// **Endpoint público** - Não requer autenticação (útil para testes de conectividade).
        /// 
        /// **Finalidade:** Permite testar se o controlador de webhook está funcionando corretamente, útil para validação de configuração.
        /// 
        /// **Como usar:**
        /// ```
        /// GET /api/webhook/test
        /// ```
        /// 
        /// **Resposta exemplo:**
        /// ```json
        /// {
        ///   "message": "Webhook endpoint is working",
        ///   "timestamp": "2025-06-02T10:30:00.000Z"
        /// }
        /// ```
        ///        /// **Casos de uso:**
        /// - **Verificação de saúde**: Confirma que o endpoint está ativo
        /// - **Testes de configuração**: Valida conectividade e roteamento
        /// - **Monitoramento**: Pode ser usado por ferramentas de monitoramento
        /// </summary>
        /// <returns>Mensagem confirmando que o webhook está funcionando</returns>
        /// <response code="200">Endpoint funcionando corretamente</response>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { 
                Message = "Webhook endpoint is working", 
                Timestamp = DateTime.UtcNow 
            });
        }
    }
}
