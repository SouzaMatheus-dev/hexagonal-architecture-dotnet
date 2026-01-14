# Exemplos de Uso

## Testando a API REST (JSON)

### 1. Criar um Pedido

**Requisição:**

```bash
POST https://localhost:5001/api/orders
Content-Type: application/json

{
  "customerName": "João Silva",
  "customerEmail": "joao@example.com",
  "items": [
    {
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "productName": "Notebook Dell XPS",
      "price": 4500.00,
      "quantity": 1
    },
    {
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "productName": "Mouse Logitech",
      "price": 89.90,
      "quantity": 2
    }
  ]
}
```

**Resposta (201 Created):**

```json
{
  "id": "a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d",
  "customerName": "João Silva",
  "customerEmail": "joao@example.com",
  "totalAmount": 4679.8,
  "status": "Pending",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null,
  "items": [
    {
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "productName": "Notebook Dell XPS",
      "price": 4500.0,
      "quantity": 1
    },
    {
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "productName": "Mouse Logitech",
      "price": 89.9,
      "quantity": 2
    }
  ]
}
```

### 2. Obter um Pedido

**Requisição:**

```bash
GET https://localhost:5001/api/orders/{id}
```

**Resposta (200 OK):**

```json
{
  "id": "a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d",
  "customerName": "João Silva",
  "customerEmail": "joao@example.com",
  "totalAmount": 4679.80,
  "status": "Pending",
  ...
}
```

### 3. Atualizar Status do Pedido

**Requisição:**

```bash
PATCH https://localhost:5001/api/orders/{id}/status
Content-Type: application/json

{
  "status": "Confirmed"
}
```

**Status disponíveis:**

- `Pending`
- `Confirmed`
- `Cancelled`
- `Delivered`

## Usando cURL

### Criar Pedido

```bash
curl -X POST "https://localhost:5001/api/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "customerName": "Maria Santos",
    "customerEmail": "maria@example.com",
    "items": [
      {
        "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "productName": "Smartphone",
        "price": 1200.00,
        "quantity": 1
      }
    ]
  }'
```

### Obter Pedido

```bash
curl -X GET "https://localhost:5001/api/orders/{order-id}"
```

### Confirmar Pedido

```bash
curl -X PATCH "https://localhost:5001/api/orders/{order-id}/status" \
  -H "Content-Type: application/json" \
  -d '{"status": "Confirmed"}'
```

## Testando gRPC

### Usando Postman

1. Abra o Postman (versão 9.0 ou superior)
2. Crie uma nova requisição gRPC (New → gRPC Request)
3. Importe o arquivo `.proto`: `src/Adapters.Input.Grpc/Protos/order.proto`
4. Configure o servidor: `localhost:5001` (HTTPS recomendado)
5. Aceite o certificado auto-assinado quando solicitado
6. Selecione o método desejado:
   - `CreateOrder`
   - `GetOrder`
   - `UpdateOrderStatus`

### Exemplo de Requisição CreateOrder (gRPC)

```json
{
  "customer_name": "Carlos Oliveira",
  "customer_email": "carlos@example.com",
  "items": [
    {
      "product_id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "product_name": "Tablet",
      "price": 800.0,
      "quantity": 1
    }
  ]
}
```

**Nota**: O Postman aceita tanto `snake_case` (customer_name) quanto `PascalCase` (CustomerName) nos campos.

## Testando Validações de Negócio

### Teste 1: Tentar confirmar pedido que não está pendente

1. Crie um pedido (status: Pending)
2. Confirme o pedido (status: Confirmed)
3. Tente confirmar novamente (deve falhar)

**Resposta esperada:**

```json
{
  "message": "Apenas pedidos pendentes podem ser confirmados"
}
```

### Teste 2: Tentar cancelar pedido entregue

1. Crie um pedido
2. Confirme o pedido
3. Marque como entregue (status: Delivered)
4. Tente cancelar (deve falhar)

**Resposta esperada:**

```json
{
  "message": "Pedidos entregues não podem ser cancelados"
}
```

## Fluxo Completo de um Pedido

```
1. Cliente → POST /api/orders
   Status: Pending

2. Cliente → PATCH /api/orders/{id}/status (Confirmed)
   Status: Confirmed
   → Notificação enviada (console/email)

3. Cliente → PATCH /api/orders/{id}/status (Delivered)
   Status: Delivered

Ou alternativamente:

2. Cliente → PATCH /api/orders/{id}/status (Cancelled)
   Status: Cancelled
   → Notificação de cancelamento enviada
```

## Observações Importantes

- O repositório atual é **In-Memory**, então os dados são perdidos ao reiniciar a aplicação
- As notificações são enviadas para o **Console** (veja a saída do terminal)
- Ambos os adapters (REST e gRPC) usam a **mesma lógica de negócio**
- Você pode usar **qualquer adapter** sem alterar o core
