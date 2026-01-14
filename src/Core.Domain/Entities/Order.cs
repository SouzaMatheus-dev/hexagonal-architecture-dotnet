namespace Core.Domain.Entities;

/// <summary>
/// Entidade de domínio - Pedido
/// Representa o conceito central do negócio, sem dependências externas
/// </summary>
public class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public string CustomerEmail { get; private set; }
    public decimal TotalAmount { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public List<OrderItem> Items { get; private set; }

    private Order() 
    { 
        // Construtor privado para EF Core
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        Items = new List<OrderItem>();
    }

    public Order(string customerName, string customerEmail, List<OrderItem> items)
    {
        Id = Guid.NewGuid();
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        Items = items ?? new List<OrderItem>();
        TotalAmount = Items.Sum(item => item.Price * item.Quantity);
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Apenas pedidos pendentes podem ser confirmados");

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Pedidos entregues não podem ser cancelados");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Apenas pedidos confirmados podem ser marcados como entregues");

        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Delivered = 3
}
