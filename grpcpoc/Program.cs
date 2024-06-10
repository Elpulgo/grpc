using grpcpoc.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLogging();

// Add services to the container.
builder.Services.AddGrpc();

// To be able to inspect and describe in dev mode via grpcurl for example
builder.Services.AddGrpcReflection();

var app = builder.Build();

IWebHostEnvironment env = app.Environment;

if (env.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.UseRouting();

#pragma warning disable ASP0014
app.UseEndpoints(GrpcBootstrapper.RegisterGrpcServices);
#pragma warning restore ASP0014


app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();