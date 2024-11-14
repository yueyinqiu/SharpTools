using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using SixLabors.ImageSharp;
using System.Collections.Immutable;

namespace SptlWebsite.Pages.MeihuaYishu;

public partial class MeihuaYishuPage
{
    private IJSInProcessObjectReference jsModule;
    protected override async Task OnParametersSetAsync()
    {
        jsModule = await this.JsRuntime.InvokeAsync<IJSInProcessObjectReference>(
            "import", "./Pages/MeihuaYishu/MeihuaYishuPage.razor.js");
    }
}