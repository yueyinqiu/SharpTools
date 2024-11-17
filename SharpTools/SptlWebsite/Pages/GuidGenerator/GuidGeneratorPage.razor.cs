using SptlServices.GradedLocalStoraging;
using System.Collections.Immutable;

namespace SptlWebsite.Pages.GuidGenerator;

public partial class GuidGeneratorPage
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
    private GuidFormat FormatInput
    {
        get;
        set
        {
            field = value;
            this.SavePreference();
        }
    } = formats.Single(x => x.Name == "oooooooo-oooo-oooo-oooo-oooooooooooo");

    internal sealed record Preferences(string FormatName, int Count);

    private ILocalStorageEntry<Preferences> PreferenceStorage =>
        this.LocalStorage.GetEntry<Preferences>("GuidGeneratorPage.Preferences", Importance.SimpleOptions);

    protected override void OnParametersSet()
    {
        if (this.PreferenceStorage.TryGet(out var preference) && preference is not null)
        {
            this.countInput = Math.Clamp(preference.Count, 1, int.MaxValue);
            this.FormatInput = formats.FirstOrDefault(
                x => x.Name == preference.FormatName,
                this.FormatInput);
        }
    }

    private void SavePreference()
    {
        this.PreferenceStorage.Set(new(this.FormatInput.Name, this.countInput));
    }

    private void ButtonClick()
    {
        this.outputs = Enumerable.Range(0, this.countInput)
            .Select(_ => Guid.NewGuid())
            .ToImmutableArray();

        this.SavePreference();
        this.StateHasChanged();
    }
}