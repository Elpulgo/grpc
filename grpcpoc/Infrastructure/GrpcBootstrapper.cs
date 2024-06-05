using System.Reflection;

namespace grpcpoc.Infrastructure;

internal static class GrpcBootstrapper
{
    private const string GrpcServiceMethodName = "MapGrpcService";
    private const string GrpcMethodBindingAttributeValue = "BindService";

    /// <summary>
    /// Equivalent to app.MapGrpcService<FooService>(); but in a generic way so we don't need to care when adding new GrpcServices 
    /// </summary>
    /// <param name="endpoints"></param>
    internal static void RegisterGrpcServices(IEndpointRouteBuilder endpoints)
    {
        var assemblyTypes = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .ToArray();

        var gRpcGeneratedTypes = assemblyTypes
            .Where(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(m => m.Name == GrpcMethodBindingAttributeValue && m.GetParameters().Length == 1))
            .ToList();

        foreach (var gRpcGenerateType in gRpcGeneratedTypes)
        {
            var implementationBaseClass = assemblyTypes
                .SingleOrDefault(t =>
                    t is { IsClass: true, IsAbstract: true } && t.Name == $"{gRpcGenerateType.Name}Base");

            if (implementationBaseClass is null)
            {
                continue;
            }

            var concreteImplementation = assemblyTypes
                .SingleOrDefault(w => w.IsSubclassOf(implementationBaseClass));

            if (concreteImplementation is null)
            {
                continue;
            }

            var mapGrpcServiceMethod = typeof(GrpcEndpointRouteBuilderExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m => m.Name == GrpcServiceMethodName && m.GetGenericArguments().Length == 1)
                .MakeGenericMethod(concreteImplementation);

            mapGrpcServiceMethod.Invoke(null, new object?[] { endpoints });
        }
    }
}