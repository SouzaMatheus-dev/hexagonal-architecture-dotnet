namespace Core.Domain.Entities;

/// <summary>
/// Item de um pedido - Value Object ou Entidade dentro do agregado
/// </summary>
public class OrderItem
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }

    private OrderItem() 
    { 
        // Construtor privado para EF Core
        ProductName = string.Empty;
    }

    public OrderItem(Guid productId, string productName, decimal price, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser maior que zero", nameof(quantity));

        if (price < 0)
            throw new ArgumentException("O preço não pode ser negativo", nameof(price));

        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
    }
}
