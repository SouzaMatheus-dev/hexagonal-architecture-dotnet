using Core.Application.Ports.Input;
using Core.Application.Ports.Output;
using Core.Application.UseCases;
using Adapters.Output.Persistence;
using Adapters.Output.External;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Configuração de Injeção de Dependência
/// Aqui conectamos os Ports (interfaces) com os Adapters (implementações)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar casos de uso (use cases)
        services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
        services.AddScoped<IGetOrderUseCase, GetOrderUseCase>();
        services.AddScoped<IUpdateOrderStatusUseCase, UpdateOrderStatusUseCase>();

        return services;
    }

    public static IServiceCollection AddOutputAdapters(this IServiceCollection services)
    {
        // Registrar adapters de saída
        // Fácil trocar InMemoryOrderRepository por EntityFrameworkOrderRepository, MongoOrderRepository, etc.
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();

        // Fácil trocar ConsoleNotificationService por EmailNotificationService, SmsNotificationService, etc.
        services.AddScoped<INotificationService, ConsoleNotificationService>();

        return services;
    }
}
