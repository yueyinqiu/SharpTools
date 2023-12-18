using MudBlazor;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics;

namespace SharpTools.Tools;

public partial class Guid
{
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
            new("字节数组小端序", (guid) => Convert.ToBase64String(guid.ToByteArray())),
            new("字节数组大端序", (guid) => Convert.ToBase64String(guid.ToByteArray(true))),
        ];

    protected override async Task OnParametersSetAsync()
    {
        this.inputedCount = 1;
        this.selectedFormat = formats[2];

        // 如果直接同步运行，会导致输出框的 AutoGrow 不能正常工作，
        // Task.Yield() 一下就可以了，不知道是什么原因。
        await Task.Yield();
        this.DisplayNewGuids();
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

        var output = currentGuids.Select(this.selectedFormat.Converter);
        this.output = string.Join(Environment.NewLine, output);
    }

    private void DisplayNewGuids()
    {
        Debug.Assert(this.inputedCount.HasValue);

        this.currentGuids = NewGuids(inputedCount.Value);
        RedisplayCurrentGuids();
    }
}
