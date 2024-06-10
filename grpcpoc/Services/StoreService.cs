using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace grpcpoc.Services;

public class StoreService : grpcpoc.StoreService.StoreServiceBase
{
    private readonly ILogger<StoreService> _logger;
    private readonly Dictionary<int, StoreInfo> _storeInfos = new();

    public StoreService(ILogger<StoreService> logger)
    {
        _logger = logger;
        BuildStoreInfos();
    }

    public override Task<Empty> UpdateBalance(UpdateBalanceRequest request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented"));
        return base.UpdateBalance(request, context);
    }

    public override Task<Empty> AddProducts(AddProductsRequest request, ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented"));
        return base.AddProducts(request, context);
    }

    public override Task<StoreInfo> GetStoreInfo(StoreInfoRequest request, ServerCallContext context)
    {
        var token = context.RequestHeaders.Get("Authorization");
        _logger.LogInformation($"{DateTime.Now.TimeOfDay}: Request from client with token: '{token}'");
        
        if(_storeInfos.TryGetValue(request.Id, out var storeInfo))
        {
            return Task.FromResult(storeInfo);
        }
        
        throw new RpcException(new Status(StatusCode.NotFound, $"StoreInfo with id {request.Id} was not found"));
    }

    public override Task GetTransportSchemas(TransportSchemasRequest request,
        IServerStreamWriter<TransportSchema> responseStream,
        ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented"));
        return base.GetTransportSchemas(request, responseStream, context);
    }

    private void BuildStoreInfos()
    {
        for (int i = 0; i < 10_000; i++)
        {
            _storeInfos.Add(i, new StoreInfo
            {
                Balance = new Balance
                {
                    Count = i * 3,
                    Id = i,
                },
                Id = i,
                Products =
                {
                    Enumerable.Range(0, 10).Select((s, j) => new Product
                    {
                        Name = $"Product-{i}-{j}",
                        Location = new Location
                        {
                            City = "Citttty",
                            Latitude = i + j * 2,
                            Longitude = i + 4 * j,
                            Street = "Streety",
                            StreetNumber = j,
                            PostalCode = "244 31"
                        },
                        Price = i * j,
                        EanCode = (i * j).ToString("X")
                    })
                }
            });
        }
    }
}