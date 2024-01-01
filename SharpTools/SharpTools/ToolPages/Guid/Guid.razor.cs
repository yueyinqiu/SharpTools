using SharpTools.Services.GradedLocalStoraging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Numerics;

namespace SharpTools.ToolPages.Guid;

public partial class Guid
{
    private LocalStorageEntry<Preferences>? preferencesStorage;
    private string? output;
    private int? inputedCount;
    private GuidFormat? selectedFormat;
    private ImmutableArray<System.Guid>? currentGuids;

    private sealed record GuidFormat(string Name, Func<System.Guid, string> Converter);

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

    private sealed record Preferences(string FormatName, int Count, ImmutableArray<System.Guid> Guids);

    protected override async Task OnParametersSetAsync()
    {
        this.preferencesStorage = this.GradedLocalStorage.GetEntry<Preferences>("guid", 1);

        // 同步运行会导致输出框的 AutoGrow 不能正常工作，不知道是什么原因。
        await Task.Yield();

        var preference = this.preferencesStorage.Get();
        if (preference == null)
        {
            this.inputedCount = 1;
            this.selectedFormat = formats.Single(x => x.Name == "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
            this.DisplayNewGuids();
        }
        else
        {
            this.inputedCount = preference.Count;
            this.selectedFormat = formats.FirstOrDefault(
                x => x.Name == preference.FormatName,
                formats.Single(x => x.Name == "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"));
            this.currentGuids = preference.Guids;
            this.RedisplayCurrentGuids();
        }
    }

    private static ImmutableArray<System.Guid> NewGuids(int count)
    {
        var guids = Enumerable.Range(0, count)
            .Select(_ => System.Guid.NewGuid());
        return guids.ToImmutableArray();
    }

    private void RedisplayCurrentGuids(GuidFormat? newFormat = null)
    {
        Debug.Assert(this.currentGuids is not null);

        this.selectedFormat = newFormat ?? this.selectedFormat;
        Debug.Assert(this.selectedFormat is not null);

        var output = this.currentGuids.Select(this.selectedFormat.Converter);
        this.output = string.Join(Environment.NewLine, output);

        Debug.Assert(this.inputedCount.HasValue);
        this.preferencesStorage?.Set(
            new(this.selectedFormat.Name, this.inputedCount.Value, this.currentGuids.Value));
    }

    private void DisplayNewGuids()
    {
        Debug.Assert(this.inputedCount.HasValue);

        this.currentGuids = NewGuids(this.inputedCount.Value);
        this.RedisplayCurrentGuids();
    }

    private sealed class NoExceptionIntConverter : MudBlazor.Converter<int?, string>
    {
        public NoExceptionIntConverter()
        {
            this.SetFunc = (i) => i?.ToString() ?? "";
            this.GetFunc = (s) =>
            {
                const NumberStyles style = NumberStyles.Integer | NumberStyles.AllowThousands;
                if (BigInteger.TryParse(s, style, this.Culture, out var result))
                {
                    if (result < int.MinValue)
                        return int.MinValue;
                    else if (result > int.MaxValue)
                        return int.MaxValue;
                    else
                        return (int)result;
                }

                return 0;
            };
        }
    }
}
