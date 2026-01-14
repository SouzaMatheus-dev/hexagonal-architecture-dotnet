using Core.Application.Ports.Input;
using Core.Application.Ports.Output;

namespace Core.Application.UseCases;

/// <summary>
/// Caso de Uso - Obtém um pedido pelo ID
/// </summary>
public class GetOrderUseCase : IGetOrderUseCase
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Core.Domain.Entities.Order?> ExecuteAsync(Guid orderId)
    {
        return await _orderRepository.GetByIdAsync(orderId);
    }
}
