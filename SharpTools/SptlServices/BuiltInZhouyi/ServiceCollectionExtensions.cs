using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SptlServices.BuiltInZhouyi;
public static class ServiceCollectionExtensions
{
    public static void AddSptlBuiltInZhouyi(this IServiceCollection services, string baseAddress)
    {
        services.TryAddScoped<IBuiltInZhouyiAccessor>((_) =>
        {
            return new BuiltInZhouyiAccessor(baseAddress);
        });
    }
}
