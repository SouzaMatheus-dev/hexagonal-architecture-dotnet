using Core.Domain.Entities;

namespace Core.Application.Ports.Output;

/// <summary>
/// Port de Saída (Output Port) - Define o contrato para persistência de pedidos
/// Implementado pelos adapters de saída (In-Memory, EF Core, MongoDB, etc.)
/// </summary>
public interface IOrderRepository
{
    Task<Order> SaveAsync(Order order);
    Task<Order?> GetByIdAsync(Guid id);
    Task<List<Order>> GetAllAsync();
    Task<bool> DeleteAsync(Guid id);
}
