using Core.Application.Ports.Input;
using Core.Domain.Entities;
using Grpc.Core;
using Adapters.Input.Grpc;

namespace Adapters.Input.Grpc.Services;

/// <summary>
/// Adapter de Entrada - gRPC Service
/// Este adapter converte requisições gRPC em chamadas para os casos de uso
/// Note que este adapter NÃO conhece a implementação dos repositórios ou serviços externos
/// Mesma lógica de negócio, apenas um protocolo diferente!
/// </summary>
public class OrderGrpcService : OrderService.OrderServiceBase
{
  private readonly ICreateOrderUseCase _createOrderUseCase;
  private readonly IGetOrderUseCase _getOrderUseCase;
  private readonly IUpdateOrderStatusUseCase _updateOrderStatusUseCase;

  public OrderGrpcService(
      ICreateOrderUseCase createOrderUseCase,
      IGetOrderUseCase getOrderUseCase,
      IUpdateOrderStatusUseCase updateOrderStatusUseCase)
  {
    _createOrderUseCase = createOrderUseCase;
    _getOrderUseCase = getOrderUseCase;
    _updateOrderStatusUseCase = updateOrderStatusUseCase;
  }

  public override async Task<OrderResponse> CreateOrder(
      CreateOrderRequest request,
      ServerCallContext context)
  {
    try
    {
      var command = new CreateOrderCommand(
          request.CustomerName,
          request.CustomerEmail,
          request.Items.Select(item => new OrderItemDto(
              Guid.Parse(item.ProductId),
              item.ProductName,
              (decimal)item.Price,
              item.Quantity
          )).ToList()
      );

      var order = await _createOrderUseCase.ExecuteAsync(command);

      return MapToGrpcResponse(order);
    }
    catch (ArgumentException ex)
    {
      throw new RpcException(
          new Status(StatusCode.InvalidArgument, ex.Message)
      );
    }
  }

  public override async Task<OrderResponse> GetOrder(
      GetOrderRequest request,
      ServerCallContext context)
  {
    if (!Guid.TryParse(request.OrderId, out var orderId))
    {
      throw new RpcException(
          new Status(StatusCode.InvalidArgument, "ID do pedido inválido")
      );
    }

    var order = await _getOrderUseCase.ExecuteAsync(orderId);

    if (order == null)
    {
      throw new RpcException(
          new Status(StatusCode.NotFound, "Pedido não encontrado")
      );
    }

    return MapToGrpcResponse(order);
  }

  public override async Task<OrderResponse> UpdateOrderStatus(
      UpdateOrderStatusRequest request,
      ServerCallContext context)
  {
    if (!Guid.TryParse(request.OrderId, out var orderId))
    {
      throw new RpcException(
          new Status(StatusCode.InvalidArgument, "ID do pedido inválido")
      );
    }

    try
    {
      var orderStatus = Enum.Parse<OrderStatus>(request.Status, ignoreCase: true);
      var order = await _updateOrderStatusUseCase.ExecuteAsync(orderId, orderStatus);

      return MapToGrpcResponse(order);
    }
    catch (InvalidOperationException ex)
    {
      throw new RpcException(
          new Status(StatusCode.FailedPrecondition, ex.Message)
      );
    }
    catch (ArgumentException)
    {
      throw new RpcException(
          new Status(StatusCode.InvalidArgument, "Status inválido")
      );
    }
  }

  private static OrderResponse MapToGrpcResponse(Order order)
  {
    var response = new OrderResponse
    {
      OrderId = order.Id.ToString(),
      CustomerName = order.CustomerName,
      CustomerEmail = order.CustomerEmail,
      TotalAmount = (double)order.TotalAmount,
      Status = order.Status.ToString(),
      CreatedAt = order.CreatedAt.ToString("O"),
      UpdatedAt = order.UpdatedAt?.ToString("O") ?? string.Empty
    };

    response.Items.AddRange(order.Items.Select(item => new OrderItemResponse
    {
      ProductId = item.ProductId.ToString(),
      ProductName = item.ProductName,
      Price = (double)item.Price,
      Quantity = item.Quantity
    }));

    return response;
  }
}
