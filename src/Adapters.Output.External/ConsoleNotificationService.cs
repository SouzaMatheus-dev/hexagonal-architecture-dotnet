using Core.Application.Ports.Output;

namespace Adapters.Output.External;

/// <summary>
/// Adapter de Saída - Implementação de INotificationService usando Console
/// Pode ser facilmente substituído por EmailService, SmsService, PushNotificationService, etc.
/// </summary>
public class ConsoleNotificationService : INotificationService
{
    public Task SendOrderConfirmationAsync(string email, Guid orderId, decimal totalAmount)
    {
        Console.WriteLine($"[NOTIFICAÇÃO] Email enviado para {email}");
        Console.WriteLine($"             Pedido {orderId} confirmado no valor de R$ {totalAmount:F2}");
        return Task.CompletedTask;
    }

    public Task SendOrderCancellationAsync(string email, Guid orderId)
    {
        Console.WriteLine($"[NOTIFICAÇÃO] Email enviado para {email}");
        Console.WriteLine($"             Pedido {orderId} foi cancelado");
        return Task.CompletedTask;
    }
}
