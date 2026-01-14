using Core.Application.Ports.Input;
using Core.Application.Ports.Output;
using Core.Domain.Entities;

namespace Core.Application.UseCases;

/// <summary>
/// Caso de Uso - Implementa a lógica de negócio para criar um pedido
/// Este é o coração da aplicação, independente de como será chamado (REST, gRPC, etc.)
/// </summary>
public class CreateOrderUseCase : ICreateOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly INotificationService _notificationService;

    public CreateOrderUseCase(
        IOrderRepository orderRepository,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _notificationService = notificationService;
    }

    public async Task<Order> ExecuteAsync(CreateOrderCommand command)
    {
        // Validações de negócio
        if (string.IsNullOrWhiteSpace(command.CustomerName))
            throw new ArgumentException("Nome do cliente é obrigatório", nameof(command));

        if (string.IsNullOrWhiteSpace(command.CustomerEmail))
            throw new ArgumentException("Email do cliente é obrigatório", nameof(command));

        if (command.Items == null || !command.Items.Any())
            throw new ArgumentException("O pedido deve conter pelo menos um item", nameof(command));

        // Criar entidades de domínio
        var orderItems = command.Items.Select(item =>
            new OrderItem(item.ProductId, item.ProductName, item.Price, item.Quantity)
        ).ToList();

        var order = new Order(command.CustomerName, command.CustomerEmail, orderItems);

        // Salvar através do port (sem conhecer a implementação)
        var savedOrder = await _orderRepository.SaveAsync(order);

        // Enviar notificação através do port (sem conhecer a implementação)
        await _notificationService.SendOrderConfirmationAsync(
            savedOrder.CustomerEmail,
            savedOrder.Id,
            savedOrder.TotalAmount
        );

        return savedOrder;
    }
}
