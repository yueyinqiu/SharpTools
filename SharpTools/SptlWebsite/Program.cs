using FileDownloader;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
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

        _ = builder.Services.AddFileDownloder();
        _ = builder.Services.AddQRCodeDecoder();

        _ = builder.Services.AddSptlLocalStorage("SharpTools");

        await builder.Build().RunAsync();
    }
}
