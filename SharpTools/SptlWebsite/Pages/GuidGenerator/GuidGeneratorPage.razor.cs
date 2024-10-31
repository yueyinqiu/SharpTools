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
    private GuidFormat formatInputDontTouchMe = formats.Single(
        x => x.Name == "oooooooo-oooo-oooo-oooo-oooooooooooo");
    private GuidFormat FormatInput
    {
        get
        {
            return formatInputDontTouchMe;
        }
        set
        {
            formatInputDontTouchMe = value;
            SavePreference();
        }
    }

    internal sealed record Preferences(string FormatName, int Count);

    private ILocalStorageEntry<Preferences> PreferenceStorage =>
        this.LocalStorage.GetEntry<Preferences>("GuidGeneratorPage.Preferences", 500);

    protected override void OnParametersSet()
    {
        if (PreferenceStorage.TryGet(out var preference))
        {
            if (preference is null)
                return;
            countInput = Math.Clamp(preference.Count, 1, int.MaxValue);
            FormatInput = formats.FirstOrDefault(
                x => x.Name == preference.FormatName,
                formats.Single(x => x.Name == "oooooooo-oooo-oooo-oooo-oooooooooooo"));
        }
    }

    private void SavePreference()
    {
        PreferenceStorage.Set(new(FormatInput.Name, countInput));
    }

    private void ButtonClick()
    {
        outputs = Enumerable.Range(0, countInput)
            .Select(_ => Guid.NewGuid())
            .ToImmutableArray();

        SavePreference();
        this.StateHasChanged();
    }
}