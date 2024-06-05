using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace grpcpoc.Services;


public class StoreService : grpcpoc.StoreService.StoreServiceBase
{
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
        throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented"));
        return base.GetStoreInfo(request, context);
    }

    public override Task GetTransportSchemas(TransportSchemasRequest request, IServerStreamWriter<TransportSchema> responseStream,
        ServerCallContext context)
    {
        throw new RpcException(new Status(StatusCode.Unimplemented, "Not implemented"));
        return base.GetTransportSchemas(request, responseStream, context);
    }
}