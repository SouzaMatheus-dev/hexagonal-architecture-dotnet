using Core.Application.Ports.Input;
using Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Adapters.Input.RestApi.Controllers;

/// <summary>
/// Adapter de Entrada - REST API Controller
/// Este adapter converte requisições HTTP/JSON em chamadas para os casos de uso
/// Note que este adapter NÃO conhece a implementação dos repositórios ou serviços externos
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ICreateOrderUseCase _createOrderUseCase;
    private readonly IGetOrderUseCase _getOrderUseCase;
    private readonly IUpdateOrderStatusUseCase _updateOrderStatusUseCase;

    public OrdersController(
        ICreateOrderUseCase createOrderUseCase,
        IGetOrderUseCase getOrderUseCase,
        IUpdateOrderStatusUseCase updateOrderStatusUseCase)
    {
        _createOrderUseCase = createOrderUseCase;
        _getOrderUseCase = getOrderUseCase;
        _updateOrderStatusUseCase = updateOrderStatusUseCase;
    }

    /// <summary>
    /// Cria um novo pedido via JSON
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var command = new CreateOrderCommand(
                request.CustomerName,
                request.CustomerEmail,
                request.Items.Select(item => new OrderItemDto(
                    item.ProductId,
                    item.ProductName,
                    item.Price,
                    item.Quantity
                )).ToList()
            );

            var order = await _createOrderUseCase.ExecuteAsync(command);

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.Id },
                MapToDto(order)
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtém um pedido pelo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        var order = await _getOrderUseCase.ExecuteAsync(id);

        if (order == null)
            return NotFound(new { message = "Pedido não encontrado" });

        return Ok(MapToDto(order));
    }

    /// <summary>
    /// Atualiza o status de um pedido
    /// </summary>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(
        Guid id,
        [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var orderStatus = Enum.Parse<OrderStatus>(request.Status, ignoreCase: true);
            var order = await _updateOrderStatusUseCase.ExecuteAsync(id, orderStatus);

            return Ok(MapToDto(order));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException)
        {
            return BadRequest(new { message = "Status inválido" });
        }
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(item => new OrderItemResponseDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}

// DTOs para requisições e respostas
public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class OrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; } = new();
}

public class OrderItemResponseDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
