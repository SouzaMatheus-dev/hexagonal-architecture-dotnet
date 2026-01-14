using Core.Application.Ports.Input;
using Core.Application.Ports.Output;
using Core.Domain.Entities;

namespace Core.Application.UseCases;

/// <summary>
/// Caso de Uso - Atualiza o status de um pedido
/// </summary>
public class UpdateOrderStatusUseCase : IUpdateOrderStatusUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly INotificationService _notificationService;

    public UpdateOrderStatusUseCase(
        IOrderRepository orderRepository,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _notificationService = notificationService;
    }

    public async Task<Order> ExecuteAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order == null)
            throw new InvalidOperationException($"Pedido com ID {orderId} não encontrado");

        // Aplicar regras de negócio através dos métodos da entidade
        switch (newStatus)
        {
            case OrderStatus.Confirmed:
                order.Confirm();
                break;
            case OrderStatus.Cancelled:
                order.Cancel();
                await _notificationService.SendOrderCancellationAsync(order.CustomerEmail, order.Id);
                break;
            case OrderStatus.Delivered:
                order.MarkAsDelivered();
                break;
            default:
                throw new InvalidOperationException($"Status {newStatus} não é válido para atualização");
        }

        return await _orderRepository.SaveAsync(order);
    }
}
