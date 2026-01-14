using Core.Domain.Entities;

namespace Core.Application.Ports.Input;

/// <summary>
/// Port de Entrada - Define o contrato para atualizar o status de um pedido
/// </summary>
public interface IUpdateOrderStatusUseCase
{
    Task<Order> ExecuteAsync(Guid orderId, OrderStatus newStatus);
}
