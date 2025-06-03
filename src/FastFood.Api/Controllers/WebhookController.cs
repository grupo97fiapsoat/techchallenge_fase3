using FastFood.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FastFood.Api.Controllers
{   
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
        }       
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
        }      
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
