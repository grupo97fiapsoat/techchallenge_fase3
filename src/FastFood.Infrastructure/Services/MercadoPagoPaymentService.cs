using FastFood.Domain.Orders.Services;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MercadoPago.Client.Payment;
using MercadoPago.Http;

namespace FastFood.Infrastructure.Services
{
    public class MercadoPagoPaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MercadoPagoPaymentService> _logger;
        private readonly PreferenceClient _preferenceClient;

        public MercadoPagoPaymentService(IConfiguration configuration, ILogger<MercadoPagoPaymentService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Configurar o Mercado Pago
            var accessToken = _configuration["MercadoPago:AccessToken"];
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("MercadoPago:AccessToken não configurado. O serviço de pagamento não funcionará corretamente.");
            }
            else
            {
                MercadoPagoConfig.AccessToken = accessToken;
                _preferenceClient = new PreferenceClient();
                _logger.LogInformation("MercadoPago Payment Service inicializado");
            }
        }

        public async Task<bool> ProcessPaymentAsync(Guid orderId, string qrCode)
        {
            try
            {
                _logger.LogInformation("Verificando status do pagamento para pedido {OrderId}", orderId);

                // Extrair o ID da preferência do QR Code
                var preferenceId = ExtractPreferenceIdFromQrCode(qrCode);
                if (string.IsNullOrEmpty(preferenceId))
                {
                    _logger.LogWarning("Não foi possível extrair o ID da preferência do QR Code");
                    return false;
                }


                // Busca os pagamentos para esta preferência
                var paymentClient = new MercadoPago.Client.Payment.PaymentClient();
                
                // Primeiro, tenta buscar por external_reference (ID do pedido)
                // Busca pagamentos por external_reference (ID do pedido)
                var searchRequest = new MercadoPago.Client.SearchRequest
                {
                    Limit = 10,
                    Offset = 0,
                    Filters = new Dictionary<string, object>
                    {
                        ["external_reference"] = orderId.ToString()
                    }
                };

                _logger.LogInformation("Buscando pagamentos para o pedido {OrderId}", orderId);
                var searchResults = await paymentClient.SearchAsync(searchRequest);
                
                // Verifica se há algum pagamento aprovado
                var approvedPayment = searchResults.Results?
                    .FirstOrDefault(p => p.Status == "approved" || p.Status == "authorized");

                if (approvedPayment != null)
                {
                    _logger.LogInformation("Pagamento aprovado encontrado para o pedido {OrderId}. Payment ID: {PaymentId}", 
                        orderId, approvedPayment.Id);
                    return true;
                }


                _logger.LogWarning("Nenhum pagamento aprovado encontrado para o pedido {OrderId}", orderId);
                
                // Se não encontrou, tenta buscar diretamente pelo ID da preferência
                _logger.LogInformation("Tentando buscar pagamento diretamente pela preferência {PreferenceId}", preferenceId);
                
                try
                {
                    // Tenta obter a preferência para ver seus pagamentos
                    var preference = await _preferenceClient.GetAsync(preferenceId);
                    
                    // Verifica se há pagamentos associados a esta preferência
                    // Busca pagamentos diretamente pelo ID da preferência
                    var preferenceSearchRequest = new MercadoPago.Client.SearchRequest
                    {
                        Limit = 10,
                        Offset = 0,
                        Filters = new Dictionary<string, object>
                        {
                            ["preference_id"] = preferenceId
                        }
                    };
                    
                    var paymentsForPreference = await paymentClient.SearchAsync(preferenceSearchRequest);
                    
                    var approvedPaymentForPreference = paymentsForPreference.Results?
                        .FirstOrDefault(p => p.Status == "approved" || p.Status == "authorized");
                        
                    if (approvedPaymentForPreference != null)
                    {
                        _logger.LogInformation("Pagamento aprovado encontrado para a preferência {PreferenceId}. Payment ID: {PaymentId}", 
                            preferenceId, approvedPaymentForPreference.Id);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao buscar preferência {PreferenceId} no Mercado Pago", preferenceId);
                }
                
                // Para fins de teste, você pode descomentar a linha abaixo para simular um pagamento aprovado
                // return true;
                
                _logger.LogWarning("Nenhum pagamento aprovado encontrado para o pedido {OrderId} ou preferência {PreferenceId}", 
                    orderId, preferenceId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar status do pagamento para o pedido {OrderId}", orderId);
                return false;
            }
        }



        public async Task<(string QrCodeUrl, string PreferenceId)> GenerateQrCodeAsync(Guid orderId, decimal amount)
        {
            try
            {
                _logger.LogInformation("Gerando QR Code para pedido {OrderId} no valor de {Amount}", orderId, amount);

                var preferenceRequest = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>
                    {
                        new PreferenceItemRequest
                        {
                            Title = $"Pedido FastFood #{orderId}",
                            Description = $"Pagamento do pedido #{orderId}",
                            Quantity = 1,
                            CurrencyId = "BRL",
                            UnitPrice = amount
                        }
                    },
                    ExternalReference = orderId.ToString(),
                    BackUrls = new PreferenceBackUrlsRequest
                    {
                        Success = _configuration["MercadoPago:CallbackUrls:Success"],
                        Failure = _configuration["MercadoPago:CallbackUrls:Failure"],
                        Pending = _configuration["MercadoPago:CallbackUrls:Pending"]
                    },
                    AutoReturn = "approved",
                    NotificationUrl = _configuration["MercadoPago:NotificationUrl"],
                    BinaryMode = true, // Apenas aprovado ou rejeitado, sem pendente
                    ExpirationDateFrom = DateTime.Now,
                    ExpirationDateTo = DateTime.Now.AddMinutes(30), // Expira em 30 minutos
                    Payer = new PreferencePayerRequest
                    {
                        Name = "Cliente Teste",
                        Email = "test_user_123456@testuser.com" // Email de teste do Mercado Pago
                    }
                };

                var preference = await _preferenceClient.CreateAsync(preferenceRequest);
                
                _logger.LogInformation("Preferência criada com sucesso. ID: {PreferenceId}", preference.Id);

                // Retorna tanto a URL do QR Code quanto o ID da preferência
                string qrCodeUrl = preference.SandboxInitPoint ?? preference.InitPoint;
                return (qrCodeUrl, preference.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar QR Code para pedido {OrderId}", orderId);
                throw new InvalidOperationException($"Erro ao gerar QR Code: {ex.Message}", ex);
            }
        }        private string ExtractPreferenceIdFromQrCode(string qrCode)
        {
            try
            {
                // Extrai o ID da preferência da URL do QR Code
                // Exemplo: https://sandbox.mercadopago.com.br/checkout/v1/redirect?pref_id=123456789-abc123
                var uri = new Uri(qrCode);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                return query["pref_id"];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair ID da preferência do QR Code");
                return null;
            }
        }

        // Método auxiliar para processar webhooks (usado pelo WebhookController)
        public bool ProcessWebhook(string paymentData, out string transactionId, out string message)
        {
            transactionId = string.Empty;
            message = string.Empty;

            try
            {
                _logger.LogInformation("Processando dados de pagamento: {PaymentData}", paymentData);

                // Parse dos dados do webhook do Mercado Pago
                var webhookData = JsonSerializer.Deserialize<MercadoPagoWebhookData>(paymentData);
                
                if (webhookData == null)
                {
                    _logger.LogWarning("Dados de webhook inválidos recebidos");
                    message = "Dados de pagamento inválidos";
                    return false;
                }

                // Verificar o status do pagamento
                var isApproved = webhookData.Action == "payment.updated" && 
                                webhookData.Data?.Id != null;

                if (isApproved)
                {
                    _logger.LogInformation("Pagamento aprovado. Payment ID: {PaymentId}", webhookData.Data?.Id);
                    transactionId = webhookData.Data?.Id?.ToString() ?? string.Empty;
                    message = "Pagamento aprovado com sucesso";
                    return true;
                }
                else
                {
                    _logger.LogWarning("Pagamento não aprovado ou dados incompletos");
                    transactionId = webhookData.Data?.Id?.ToString() ?? string.Empty;
                    message = "Pagamento não foi aprovado";
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar pagamento");
                message = $"Erro interno ao processar pagamento: {ex.Message}";
                return false;
            }
        }
    }

    // Classes para deserializar os dados do webhook do Mercado Pago
    public class MercadoPagoWebhookData
    {
        public string? Action { get; set; }
        public string? ApiVersion { get; set; }
        public WebhookDataInfo? Data { get; set; }
        public DateTime DateCreated { get; set; }
        public long Id { get; set; }
        public bool LiveMode { get; set; }
        public string? Type { get; set; }
        public string? UserId { get; set; }
    }

    public class WebhookDataInfo
    {
        public long? Id { get; set; }
    }
}
