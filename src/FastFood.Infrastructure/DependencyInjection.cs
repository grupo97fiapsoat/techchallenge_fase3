using FastFood.Domain.Customers.Repositories;
using FastFood.Domain.Orders.Repositories;
using FastFood.Domain.Orders.Services;
using FastFood.Domain.Products.Repositories;
using FastFood.Infrastructure.Data;
using FastFood.Infrastructure.Repositories;
using FastFood.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastFood.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FastFoodDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
        // Registrar repositórios
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
    
        // Registrar serviços
        services.AddScoped<IPaymentService, FakePaymentService>(); // Adicionando serviço de pagamento
        services.AddScoped<INotificationService, EmailNotificationService>();
        // Descomente a linha abaixo para usar notificações por SMS em vez de email
        // services.AddScoped<INotificationService, SmsNotificationService>();

        return services;
    }
}
