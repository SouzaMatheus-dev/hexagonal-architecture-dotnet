using Infrastructure;
using Adapters.Input.Grpc.Services;
using Adapters.Input.RestApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configurar serviços da aplicação
builder.Services.AddApplication();
builder.Services.AddOutputAdapters();

// Configurar REST API (JSON)
builder.Services.AddControllers()
    .AddApplicationPart(typeof(OrdersController).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new()
  {
    Title = "Hexagonal Architecture API",
    Version = "v1",
    Description = "Exemplo de Arquitetura Hexagonal com adapters REST (JSON) e gRPC"
  });
});

// Configurar gRPC
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseAuthorization();

// Mapear controllers REST
app.MapControllers();

// Mapear serviços gRPC
app.MapGrpcService<OrderGrpcService>();

// Endpoint raiz para verificação
app.MapGet("/", () =>
    Results.Ok(new
    {
      message = "Hexagonal Architecture Demo",
      swagger = "/swagger",
      endpoints = new[]
        {
            "GET /api/orders/{id}",
            "POST /api/orders",
            "PATCH /api/orders/{id}/status"
        }
    }))
    .WithName("Root")
    .ExcludeFromDescription(); // Não aparece no Swagger

app.Run();
