using Core.Domain.Entities;

namespace Core.Application.Ports.Input;

/// <summary>
/// Port de Entrada - Define o contrato para obter um pedido
/// </summary>
public interface IGetOrderUseCase
{
    Task<Order?> ExecuteAsync(Guid orderId);
}
