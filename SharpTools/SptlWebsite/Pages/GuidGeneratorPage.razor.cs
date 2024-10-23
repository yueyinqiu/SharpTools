using SptlServices.GradedLocalStoraging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SptlWebsite.Pages;

partial class GuidGeneratorPage
{
    private sealed record GuidFormat(string Name, Func<Guid, string> Converter);

    private static readonly ImmutableArray<GuidFormat>
        formats = [
            new("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", (guid) => guid.ToString("N")),
            new("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", (guid) => guid.ToString("N").ToUpperInvariant()),
            new("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx", (guid) => guid.ToString("D")),
            new("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX", (guid) => guid.ToString("D").ToUpperInvariant()),
            new("{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}", (guid) => guid.ToString("B")),
            new("{XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX}", (guid) => guid.ToString("B").ToUpperInvariant()),
            new("(xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)", (guid) => guid.ToString("P")),
            new("(XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX)", (guid) => guid.ToString("P").ToUpperInvariant()),
            new("{xxxxxxxxxx,xxxxxx,xxxxxx,{xxxx,xxxx,xxxx,xxxx,xxxx,xxxx,xxxx,xxxx}}", (guid) => guid.ToString("X")),
            new("{XXXXXXXXXX,XXXXXX,XXXXXX,{XXXX,XXXX,XXXX,XXXX,XXXX,XXXX,XXXX,XXXX}}", (guid) => guid.ToString("X").ToUpperInvariant()),
            new("字节数组小端序 Base64", (guid) => Convert.ToBase64String(guid.ToByteArray())),
            new("字节数组大端序 Base64", (guid) => Convert.ToBase64String(guid.ToByteArray(true))),
        ];

    private ImmutableArray<Guid> outputs = [];
    private int countInput = 1;
    private GuidFormat formatInputDontTouchMe = formats.Single(
        x => x.Name == "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
    private GuidFormat FormatInput
    {
        get
        {
            return this.formatInputDontTouchMe;
        }
        set
        {
            this.formatInputDontTouchMe = value;
            this.SavePreference();
        }
    }

    internal sealed record Preferences(string FormatName, int Count);

    [JsonSerializable(typeof(Preferences))]
    partial class GuidGeneratorPageSerializerContext : JsonSerializerContext { }

    private ILocalStorageEntry<Preferences> PreferenceStorage =>
        this.LocalStorage.GetEntry(
            "GuidGeneratorPage.Preferences", 500,
            GuidGeneratorPageSerializerContext.Default.Preferences);

    protected override void OnParametersSet()
    {
        if (this.PreferenceStorage.TryGet(out var preference))
        {
            Debug.Assert(preference is not null);
            this.countInput = preference.Count;
            this.FormatInput = formats.FirstOrDefault(
                x => x.Name == preference.FormatName,
                formats.Single(x => x.Name == "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"));
        }
    }

    private void SavePreference()
    {
        this.PreferenceStorage.Set(new(this.FormatInput.Name, countInput));
    }

    private void ButtonClick()
    {
        this.outputs = Enumerable.Range(0, countInput)
            .Select(_ => Guid.NewGuid())
            .ToImmutableArray();

        this.SavePreference();
        this.StateHasChanged();
    }
}