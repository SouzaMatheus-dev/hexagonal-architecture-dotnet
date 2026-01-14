namespace Core.Application.Ports.Output;

/// <summary>
/// Port de Saída - Define o contrato para serviços externos de notificação
/// Implementado por adapters de saída (Email, SMS, Push, etc.)
/// </summary>
public interface INotificationService
{
    Task SendOrderConfirmationAsync(string email, Guid orderId, decimal totalAmount);
    Task SendOrderCancellationAsync(string email, Guid orderId);
}
