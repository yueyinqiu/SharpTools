using System.Collections.Immutable;

namespace SptlWebsite.Pages;

partial class BytesRepresentationsPage
{
    private BytesFormat inputFormatDontTouchMe = formats.Single(x => x.Name is "字节数组");
    private BytesFormat InputFormat
    {
        get => inputFormatDontTouchMe;
        set
        {
            inputFormatDontTouchMe = value;
            CacheInputBytes();
        }
    }
    private BytesFormat outputFormatDontTouchMe = formats.Single(x => x.Name is "Base64");
    private BytesFormat OutputFormat
    {
        get => outputFormatDontTouchMe;
        set
        {
            outputFormatDontTouchMe = value;
        }
    }

    private string inputDontTouchMe = 
        "[72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 33]";
    private string Input
    {
        get => inputDontTouchMe;
        set
        {
            inputDontTouchMe = value;
            CacheInputBytes();
        }
    }

    private (byte[]? bytes, Exception? ex) inputBytes = (
        [72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 33], 
        null);

    public void CacheInputBytes()
    {
        if (string.IsNullOrWhiteSpace(this.Input))
        {
            inputBytes = ([], null);
            return;
        }
        try
        {
            inputBytes = (InputFormat.ToBytes(this.Input), null);
        }
        catch (Exception ex)
        {
            inputBytes = (null, ex);
        }
    }

    private string Output
    {
        get
        {
            var (bytes, ex) = inputBytes;
            if (bytes is not null)
                return OutputFormat.FromBytes(bytes);
            else if (ex is not null)
                return $"转换失败：{Environment.NewLine}{ex.ToString()}";
            else
                return $"转换失败。";
        }
    }

    private sealed record BytesFormat(
        string Name, Func<string, byte[]> ToBytes, Func<byte[], string> FromBytes);

    private static readonly ImmutableArray<BytesFormat> formats =
        [
            new("Base64", Convert.FromBase64String, Convert.ToBase64String),
            new("Hex", Convert.FromHexString, Convert.ToHexString),
            new("字节数组",
                (s) =>
                {
                    s = s.Trim().Trim('[', ']');
                    var strings = s.Split(',');
                    if (strings.Length is 1 && string.IsNullOrWhiteSpace(strings[0]))
                        return [];
                    return strings.Select(x => byte.Parse(x)).ToArray();
                },
                (b) =>
                {
                    var result = string.Join(", ", b);
                    return $"[{result}]";
                }),
        ];

    private void Swap()
    {
        var (bytes, _) = inputBytes;
        if (bytes is null)
            return;
        this.Input = this.Output;
        var newOutputFormat = this.InputFormat;
        this.InputFormat = this.OutputFormat;
        this.OutputFormat = newOutputFormat;
    }
    private async Task ExportAsync()
    {
        var (bytes, _) = inputBytes;
        if (bytes is null)
            return;
        await Downloader.DownloadFromStream(bytes, "bytes.bin");
    }

    private sealed record Preferences(string? DisplayName);
    protected override async Task OnParametersSetAsync()
    {
    }
}