using FileDownloadBlazor.Extensions;
using HistoryBlazor.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using SptlServices.BuiltInZhouyi;
using SptlServices.GradedLocalStoraging;

namespace SptlWebsite;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        _ = builder.Services.AddFluentUIComponents(configuration =>
        {
            configuration.CollocatedJavaScriptQueryString = null;
        });

        _ = builder.Services.AddFileDownloadBlazor();
        _ = builder.Services.AddHistoryBlazor();
        _ = builder.Services.AddQRCodeDecoder();

        builder.Services.AddSptlLocalStorage("SharpTools");
        builder.Services.AddSptlBuiltInZhouyi(builder.HostEnvironment.BaseAddress);

        await builder.Build().RunAsync();
    }
}
