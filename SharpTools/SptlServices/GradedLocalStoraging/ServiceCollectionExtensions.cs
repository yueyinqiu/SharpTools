using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace SptlServices.GradedLocalStoraging;
public static class ServiceCollectionExtensions
{
    public static void AddSptlLocalStorage(this IServiceCollection services, string rootKey)
    {
        services.AddBlazoredLocalStorage()
            .TryAddScoped<IGradedLocalStorage>((services) =>
            {
                return new GradedLocalStorage(
                    services.GetRequiredService<ILogger<GradedLocalStorage>>(),
                    services.GetRequiredService<ISyncLocalStorageService>(),
                    rootKey);
            });
    }
}
