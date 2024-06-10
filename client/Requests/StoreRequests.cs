using Grpc.Core;
using grpcpoc;

namespace client.Requests;

public interface IStoreRequests
{
    Task GetStoreInfo(int storeId, CancellationToken ct);
}

public class StoreRequests : IStoreRequests
{
    private readonly StoreService.StoreServiceClient _storeClient;
    private readonly ILogger<StoreRequests> _logger;

    public StoreRequests(
        StoreService.StoreServiceClient storeClient,
        ILogger<StoreRequests> logger)
    {
        _storeClient = storeClient;
        _logger = logger;
    }

    public async Task GetStoreInfo(int storeId, CancellationToken ct)
    {
        try
        {
            var response = await _storeClient.GetStoreInfoAsync(
                new StoreInfoRequest
                {
                    Id = storeId
                },
                new Metadata
                {
                    { "key", "value" }
                },
                deadline: null, // TODO: Learn about this
                ct);

            _logger.LogInformation(
                $"GetStoreInfo({storeId}) returned: Id: '{response.Id}', Balance: '{response.Balance}'");

            foreach (var pro in response.Products)
            {
                _logger.LogInformation(
                    $"Product: {pro.Name}, {pro.Price}, {pro.EanCode}, {pro.Location.City}, {pro.Location.PostalCode}, {pro.Location.Street}, {pro.Location.Latitude}/{pro.Location.Longitude}");
            }
        }
        catch (RpcException e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}