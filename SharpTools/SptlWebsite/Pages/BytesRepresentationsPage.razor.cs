using Microsoft.FluentUI.AspNetCore.Components;
using SptlServices.GradedLocalStoraging;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json.Serialization;

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
            SavePreference();
        }
    }
    private BytesFormat outputFormatDontTouchMe = formats.Single(x => x.Name is "Base64");
    private BytesFormat OutputFormat
    {
        get => outputFormatDontTouchMe;
        set
        {
            outputFormatDontTouchMe = value;
            SavePreference();
        }
    }

    private static readonly ImmutableArray<byte> helloWorld =
        [72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 33];

    private string inputDontTouchMe = formats.Single(x => x.Name is "字节数组")
        .FromBytes([.. helloWorld]);
    private string Input
    {
        get => inputDontTouchMe;
        set
        {
            inputDontTouchMe = value;
            CacheInputBytes();
        }
    }

    private (byte[]? bytes, Exception? ex) inputBytes = ([.. helloWorld], null);

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
    private void Import(IEnumerable<FluentInputFileEventArgs> files)
    {
        var file = files.Single();
        if (file.LocalFile is null)
        {
            this.Input = $"文件导入失败：{file.ErrorMessage}";
            return;
        }
        var bytes = File.ReadAllBytes(file.LocalFile.FullName);
        this.Input = this.InputFormat.FromBytes(bytes);
        file.LocalFile.Delete();
    }

    private sealed record Preferences(string? InputFormat, string? OutputFormat);

    [JsonSerializable(typeof(Preferences))]
    partial class BytesRepresentationsPageSerializerContext : JsonSerializerContext { }

    private ILocalStorageEntry<Preferences> PreferenceStorage =>
        this.LocalStorage.GetEntry(
            "BytesRepresentationsPage.Preferences", 500,
            BytesRepresentationsPageSerializerContext.Default.Preferences);

    protected override void OnParametersSet()
    {
        if (this.PreferenceStorage.TryGet(out var preference))
        {
            this.InputFormat = formats.SingleOrDefault(
                x => x.Name == preference?.InputFormat,
                formats.Single(x => x.Name is "字节数组"));
            this.OutputFormat = formats.SingleOrDefault(
                x => x.Name == preference?.OutputFormat,
                formats.Single(x => x.Name is "Base64"));
            this.Input = this.InputFormat.FromBytes([.. helloWorld]);
        }
    }

    private void SavePreference()
    {
        this.PreferenceStorage.Set(new(this.InputFormat.Name, this.OutputFormat.Name));
    }
}