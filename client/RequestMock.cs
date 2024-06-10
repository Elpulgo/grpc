using client.Requests;
using Grpc.Core;

namespace client;

public class RequestMock : IHostedService
{
    private readonly ILogger<RequestMock> _logger;
    private readonly IStoreRequests _storeRequests;

    public RequestMock(
        ILogger<RequestMock> logger,
        IStoreRequests storeRequests)
    {
        _logger = logger;
        _storeRequests = storeRequests;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var calculationTask = DoRequestToServer(cancellationToken);
        return calculationTask.IsCompleted ? calculationTask : Task.CompletedTask;
    }

    private async Task DoRequestToServer(CancellationToken ct)
    {
        var errorCount = 0;
        var storeId = 0;
        
        while (!ct.IsCancellationRequested)
        {
            // We have hit max store ids on mock-server, reset request to start over
            errorCount = ResetStoreId(errorCount, ref storeId);
            
            try
            {
                storeId++;
                await _storeRequests.GetStoreInfo(storeId, ct);
            }
            catch (RpcException rpcE)
            {
                if (rpcE.Status.StatusCode == StatusCode.NotFound)
                {
                    errorCount++;
                }
                
                _logger.LogError(rpcE, $"Failed to execute DoRequestToServer worker task due to RpcException '{rpcE.Message}'");

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to execute DoRequestToServer worker task '{e.Message}'");
            }

            await Task.Delay((int)TimeSpan.FromMilliseconds(20).TotalMilliseconds, ct);
        }
    }

    private static int ResetStoreId(int errorCount, ref int storeId)
    {
        if (errorCount > 5)
        {
            errorCount = 0;
            storeId = 0;
        }

        return errorCount;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}