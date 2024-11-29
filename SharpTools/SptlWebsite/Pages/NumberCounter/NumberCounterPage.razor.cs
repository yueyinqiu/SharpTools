using Microsoft.FluentUI.AspNetCore.Components;
using SixLabors.ImageSharp;
using SptlServices.GradedLocalStoraging;
using System.Collections.Immutable;

namespace SptlWebsite.Pages.NumberCounter;

public partial class NumberCounterPage
{
    private long value = 0;
    private string ValueDisplay => value is 0 ? "点击计数" : value.ToString();

    private long StepLength
    {
        get;
        set
        {
            field = value;
            this.SavePreference();
        }
    } = 1;

    private long valueToSkipTo = 0;

    private sealed record Preferences(long StepLength, long Value);

    private ILocalStorageEntry<Preferences> PreferenceStorage =>
        this.LocalStorage.GetEntry<Preferences>(
            "NumberCounterPage.Preferences", Importance.SimpleOptions);

    protected override void OnParametersSet()
    {
        if (this.PreferenceStorage.TryGet(out var preference) && preference is not null)
        {
            this.StepLength = preference.StepLength;
            this.valueToSkipTo = preference.Value;
        }
    }

    private void SavePreference()
    {
        this.PreferenceStorage.Set(new(this.StepLength, this.value));
    }

    private void Count()
    {
        value += StepLength;
        SavePreference();
    }
    private void Skip()
    {
        value = valueToSkipTo;
        SavePreference();
    }
}