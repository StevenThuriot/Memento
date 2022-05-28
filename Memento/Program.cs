var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<Memento.Services.MementoService>();
app.MapGet("/", () => "Memento gRPC v1.0");

app.Run();