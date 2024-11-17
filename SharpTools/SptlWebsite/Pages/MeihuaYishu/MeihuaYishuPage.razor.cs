using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using OneHexagramPerDayCore;
using SixLabors.ImageSharp;
using System.Collections.Immutable;

namespace SptlWebsite.Pages.MeihuaYishu;

public partial class MeihuaYishuPage
{
    private IJSInProcessObjectReference jsModule;
    private ZhouyiStoreWithLineTitles zhouyi = new(new(null));
    protected override async Task OnParametersSetAsync()
    {
        jsModule = await this.JsRuntime.InvokeAsync<IJSInProcessObjectReference>(
            "import", "./Pages/MeihuaYishu/MeihuaYishuPage.razor.js");

        var zhouyiRaw = await this.BuiltInZhouyi.GetZhouyiAsync();
        this.zhouyi = new ZhouyiStoreWithLineTitles(zhouyiRaw);
    }
}