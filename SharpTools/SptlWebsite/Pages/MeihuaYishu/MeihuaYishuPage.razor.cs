using Microsoft.JSInterop;
using OneHexagramPerDayCore;
using SptlServices.GradedLocalStoraging;

namespace SptlWebsite.Pages.MeihuaYishu;

public partial class MeihuaYishuPage
{
    private IJSInProcessObjectReference? jsModule;
    private ZhouyiStoreWithLineTitles zhouyi = new(new(null));
    protected override async Task OnParametersSetAsync()
    {
        this.jsModule = await this.JsRuntime.InvokeAsync<IJSInProcessObjectReference>(
            "import", "./Pages/MeihuaYishu/MeihuaYishuPage.razor.js");

        var zhouyiRaw = await this.BuiltInZhouyi.GetZhouyiAsync();
        this.zhouyi = new ZhouyiStoreWithLineTitles(zhouyiRaw);

        if (PreferenceStorage.TryGet(out var preferences) && preferences is not null)
        {
            this.script = preferences.Script;
            this.upperInput = preferences.Upper;
            this.lowerInput = preferences.Lower;
            this.changingInput = preferences.Changing;
        }
    }

    private sealed record Preferences(string Script, string Upper, string Lower, string Changing);
    private ILocalStorageEntry<Preferences> PreferenceStorage =>
        this.LocalStorage.GetEntry<Preferences>("MeihuaYishuPage.Preferences", 750);

    private void SavePreferences()
    {
        this.PreferenceStorage.Set(
            new Preferences(this.script, this.upperInput, this.lowerInput, this.changingInput));
    }
}