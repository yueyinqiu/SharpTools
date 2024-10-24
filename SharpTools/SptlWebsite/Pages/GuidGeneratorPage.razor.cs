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
            new("oooooooooooooooooooooooooooooooo", (guid) => guid.ToString("N")),
            new("OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO", (guid) => guid.ToString("N").ToUpperInvariant()),
            new("oooooooo-oooo-oooo-oooo-oooooooooooo", (guid) => guid.ToString("D")),
            new("OOOOOOOO-OOOO-OOOO-OOOO-OOOOOOOOOOOO", (guid) => guid.ToString("D").ToUpperInvariant()),
            new("{oooooooo-oooo-oooo-oooo-oooooooooooo}", (guid) => guid.ToString("B")),
            new("{OOOOOOOO-OOOO-OOOO-OOOO-OOOOOOOOOOOO}", (guid) => guid.ToString("B").ToUpperInvariant()),
            new("(oooooooo-oooo-oooo-oooo-oooooooooooo)", (guid) => guid.ToString("P")),
            new("(OOOOOOOO-OOOO-OOOO-OOOO-OOOOOOOOOOOO)", (guid) => guid.ToString("P").ToUpperInvariant()),
            new("{0xoooooooo,0xoooo,0xoooo,{0xoo,0xoo,0xoo,0xoo,0xoo,0xoo,0xoo,0xoo}}", (guid) => guid.ToString("X")),
            new("{0xOOOOOOOO,0xOOOO,0xOOOO,{0xOO,0xOO,0xOO,0xOO,0xOO,0xOO,0xOO,0xOO}}", (guid) => guid.ToString("X").ToUpperInvariant()),
            new("字节数组小端序 Base64", (guid) => Convert.ToBase64String(guid.ToByteArray())),
            new("字节数组大端序 Base64", (guid) => Convert.ToBase64String(guid.ToByteArray(true))),
        ];

    private ImmutableArray<Guid> outputs = [];
    private int countInput = 1;
    private GuidFormat formatInputDontTouchMe = formats.Single(
        x => x.Name == "oooooooo-oooo-oooo-oooo-oooooooooooo");
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
            if (preference is null)
                return;
            this.countInput = Math.Clamp(preference.Count, 1, int.MaxValue);
            this.FormatInput = formats.FirstOrDefault(
                x => x.Name == preference.FormatName,
                formats.Single(x => x.Name == "oooooooo-oooo-oooo-oooo-oooooooooooo"));
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