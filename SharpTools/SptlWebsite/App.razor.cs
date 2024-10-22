
using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;

namespace SptlWebsite;

partial class App
{
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            // BaseFontSize.WithDefault("10px");
        }
    }
}