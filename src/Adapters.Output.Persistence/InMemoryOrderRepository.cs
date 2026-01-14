using Core.Application.Ports.Output;
using Core.Domain.Entities;

namespace Adapters.Output.Persistence;

/// <summary>
/// Adapter de Saída - Implementação de IOrderRepository usando armazenamento em memória
/// Pode ser facilmente substituído por EF Core, MongoDB, PostgreSQL, etc.
/// </summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private static readonly Dictionary<Guid, Order> _orders = new();

    public Task<Order> SaveAsync(Order order)
    {
        _orders[order.Id] = order;
        return Task.FromResult(order);
    }

    public Task<Order?> GetByIdAsync(Guid id)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task<List<Order>> GetAllAsync()
    {
        return Task.FromResult(_orders.Values.ToList());
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        return Task.FromResult(_orders.Remove(id));
    }
}
