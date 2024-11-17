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

        if (this.PreferenceStorage.TryGet(out var preferences) && preferences is not null)
        {
            this.upperInput = preferences.Upper;
            this.lowerInput = preferences.Lower;
            this.changingInput = preferences.Changing;
        }

        if (this.ScriptStorage.TryGet(out var script) && script is not null)
        {
            this.script = script;
        }
    }

    private sealed record Preferences(string Upper, string Lower, string Changing);
    private ILocalStorageEntry<string> ScriptStorage =>
        this.LocalStorage.GetEntry<string>("MeihuaYishuPage.Script", Importance.ComplexScripts);
    private ILocalStorageEntry<Preferences> PreferenceStorage =>
        this.LocalStorage.GetEntry<Preferences>("MeihuaYishuPage.Preferences", Importance.SimpleOptions);

    private void SavePreferences()
    {
        this.ScriptStorage.Set(this.script);
        this.PreferenceStorage.Set(
            new Preferences(this.upperInput, this.lowerInput, this.changingInput));
    }
}