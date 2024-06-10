using client;
using client.Requests;
using Grpc.Net.ClientFactory;
using grpcpoc;

var builder = WebApplication.CreateBuilder(args);

var grpcOptions = builder.Configuration.GetSection(GrpcOptions.Grpc)
    .Get<GrpcOptions>();

builder.Services.AddLogging();
builder.Services.AddGrpc();
builder.Services.AddTransient<IStoreRequests, StoreRequests>();
builder.Services.AddTransient<ClientLoggingInterceptor>();
builder.Services.AddHostedService<RequestMock>();


var token = "foo";

builder.Services
    .AddGrpcClient<StoreService.StoreServiceClient>(o => { o.Address = new Uri(grpcOptions.ServerUrlSecure); })
    .AddInterceptor<ClientLoggingInterceptor>(InterceptorScope.Client)
    .AddCallCredentials((context, metadata) =>
    {
        if (!string.IsNullOrEmpty(token))
        {
            metadata.Add("Authorization", $"Bearer {token}");
        }

        return Task.CompletedTask;
    });
    // .ConfigureChannel(channel =>
    //     channel.UnsafeUseInsecureChannelCallCredentials = true); // Unsafe, to be able to use HTTP
    
    // Use dotnet dotnet dev-certs https --trust, if not working, use above to just try out. 

//
// app.MapGet("/",
//     () =>
//         "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

var app = builder.Build();


app.Run();