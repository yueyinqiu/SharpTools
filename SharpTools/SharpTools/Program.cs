using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;

namespace SharpTools;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        _ = builder.Services.AddMudServices((c) =>
        {
            c.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopLeft;
        });
        _ = builder.Services.AddBlazoredLocalStorage();
        _ = builder.Services.AddBlazorDownloadFile();

        await builder.Build().RunAsync();
    }
}
