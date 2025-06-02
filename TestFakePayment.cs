using System;
using System.Threading.Tasks;

namespace FastFood.Tests
{
    // Teste simples para demonstrar o problema do FakePaymentService
    public class FakePaymentServiceTest
    {
        public static async Task TestQrCodeGeneration()
        {
            Console.WriteLine("=== TESTE DO FAKE PAYMENT SERVICE ===");
            
            var orderId = Guid.NewGuid();
            var amount = 25.50m;
            
            Console.WriteLine($"OrderId: {orderId}");
            Console.WriteLine($"Amount: {amount}");
            
            // Simula primeira chamada (como no checkout)
            var qrCode1 = await GenerateQrCodeAsync(orderId, amount);
            Console.WriteLine($"QR Code 1: {qrCode1}");
            
            // Simula segunda chamada (se fosse chamado novamente)
            var qrCode2 = await GenerateQrCodeAsync(orderId, amount);
            Console.WriteLine($"QR Code 2: {qrCode2}");
            
            Console.WriteLine($"São iguais? {qrCode1 == qrCode2}");
            
            // Teste de comparação
            Console.WriteLine($"\nTeste de validação:");
            Console.WriteLine($"QR Code salvo no banco: {qrCode1}");
            Console.WriteLine($"QR Code enviado pelo usuário: {qrCode1}");
            Console.WriteLine($"Validação seria: {qrCode1 == qrCode1}");
        }
        
        private static async Task<string> GenerateQrCodeAsync(Guid orderId, decimal amount)
        {
            // Simula uma latência de rede
            await Task.Delay(100);

            // Gera um QR Code fake no formato igual ao FakePaymentService
            var qrCode = $"https://sandbox.mercadopago.com.br/checkout/v1/redirect?pref_id=FAKE-{Guid.NewGuid()}";
            return qrCode;
        }
    }
}
