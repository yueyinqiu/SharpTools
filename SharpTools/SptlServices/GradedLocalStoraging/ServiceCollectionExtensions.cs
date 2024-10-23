using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SptlServices.GradedLocalStoraging;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSptlLocalStorage(
        this IServiceCollection services, string rootKey)
    {
        return services
            .AddBlazoredLocalStorage()
            .AddScoped<IGradedLocalStorage>((services) =>
            {
                return new GradedLocalStorage(
                    services.GetRequiredService<ILogger<GradedLocalStorage>>(),
                    services.GetRequiredService<ISyncLocalStorageService>(),
                    rootKey);
            });
    }
}
