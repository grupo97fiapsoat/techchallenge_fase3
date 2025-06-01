using FastFood.Domain.Orders.Services;
using MercadoPago.Config;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
                throw new InvalidOperationException("Mercado Pago Access Token não configurado");
            }

            MercadoPagoConfig.AccessToken = accessToken;
            _preferenceClient = new PreferenceClient();

            _logger.LogInformation("MercadoPago Payment Service inicializado");
        }        public async Task<string> GenerateQrCodeAsync(Guid orderId, decimal amount)
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
                        Name = "Cliente FastFood",
                        Email = "cliente@fastfood.com"
                    }
                };

                var preference = await _preferenceClient.CreateAsync(preferenceRequest);
                
                _logger.LogInformation("Preferência criada com sucesso. ID: {PreferenceId}", preference.Id);

                // No ambiente real, você retornaria preference.QrCode ou preference.SandboxInitPoint
                // Para QR Code, use preference.QrCode se disponível
                return preference.SandboxInitPoint ?? preference.InitPoint;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar QR Code para pedido {OrderId}", orderId);
                throw new InvalidOperationException($"Erro ao gerar QR Code: {ex.Message}", ex);
            }
        }        public Task<bool> ProcessPaymentAsync(Guid orderId, string qrCode)
        {
            try
            {
                _logger.LogInformation("Processando pagamento para pedido {OrderId} com QR Code: {QrCode}", orderId, qrCode);

                // Parse dos dados do webhook do Mercado Pago
                var webhookData = JsonSerializer.Deserialize<MercadoPagoWebhookData>(qrCode);
                
                if (webhookData == null)
                {
                    _logger.LogWarning("Dados de webhook inválidos recebidos para pedido {OrderId}", orderId);
                    return Task.FromResult(false);
                }

                // Verificar o status do pagamento
                var isApproved = webhookData.Action == "payment.updated" && 
                                webhookData.Data?.Id != null;

                if (isApproved)
                {
                    _logger.LogInformation("Pagamento aprovado para pedido {OrderId}. Payment ID: {PaymentId}", 
                        orderId, webhookData.Data?.Id);
                    return Task.FromResult(true);
                }
                else
                {
                    _logger.LogWarning("Pagamento não aprovado para pedido {OrderId}", orderId);
                    return Task.FromResult(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar pagamento para pedido {OrderId}", orderId);
                return Task.FromResult(false);
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
