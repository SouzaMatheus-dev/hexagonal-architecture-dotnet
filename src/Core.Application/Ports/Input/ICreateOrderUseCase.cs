using Core.Domain.Entities;

namespace Core.Application.Ports.Input;

/// <summary>
/// Port de Entrada (Input Port) - Define o contrato para criar um pedido
/// Esta interface é usada pelos adapters de entrada (REST, gRPC, etc.)
/// </summary>
public interface ICreateOrderUseCase
{
    Task<Order> ExecuteAsync(CreateOrderCommand command);
}

public record CreateOrderCommand(
    string CustomerName,
    string CustomerEmail,
    List<OrderItemDto> Items
);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity
);
