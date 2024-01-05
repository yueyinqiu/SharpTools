using Microsoft.AspNetCore.Components.Forms;
using SharpTools.Services.GradedLocalStoraging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.ImageSharp;
using ZXing.QrCode;

namespace SharpTools.ToolPages.Bytes;

public partial class Bytes
{
    private string? inputedString;
    private Display? selectedDisplay;
    private LocalStorageEntry<Preferences>? preferencesStorage;
    private LocalStorageEntry<Inputs>? inputsStorage;

    private sealed record Display(
        string Name, Func<string, byte[]> ToBytes, Func<byte[], string> FromBytes);

    private readonly ImmutableArray<Display> displays =
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

    private sealed record Preferences(string? DisplayName);
    private sealed record Inputs(string? Input, byte[]? Bytes);
    protected override async Task OnParametersSetAsync()
    {
        this.preferencesStorage = this.GradedLocalStorage.GetEntry<Preferences>("bytes", 1);
        this.inputsStorage = this.GradedLocalStorage.GetEntry<Inputs>("bytes.inputs", 0);

        // 同步运行会导致输出框的 AutoGrow 不能正常工作，不知道是什么原因。
        await Task.Yield();

        var preference = this.preferencesStorage.Get();
        if (preference is null)
        {
            this.selectedDisplay = this.displays.Single(x => x.Name == "Base64");
        }
        else
        {
            this.selectedDisplay = this.displays.FirstOrDefault(
                x => x.Name == preference.DisplayName,
                this.displays.Single(x => x.Name == "Base64"));
        }

        var inputs = this.inputsStorage.Get();
        if (inputs is null)
        {
            this.cachedBytes = Encoding.UTF8.GetBytes("Hello World!");
            this.inputedString = this.selectedDisplay.FromBytes(this.cachedBytes);
        }
        else
        {
            this.cachedBytes = inputs.Bytes;
            this.inputedString = inputs.Input;
        }
    }

    private byte[]? cachedBytes;

    [MemberNotNullWhen(true, nameof(cachedBytes))]
    private bool CacheBytes()
    {
        if (this.cachedBytes is not null)
            return true;

        Debug.Assert(this.inputedString is not null);
        Debug.Assert(this.selectedDisplay is not null);

        try
        {
            if (string.IsNullOrWhiteSpace(this.inputedString))
                this.cachedBytes = [];
            else
                this.cachedBytes = this.selectedDisplay.ToBytes(this.inputedString);
            return true;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to resolve the input string.");
            return false;
        }
    }
    private void OnSelected(Display value)
    {
        if (!this.CacheBytes())
        {
            _ = this.MudSnackbar.Add("转换失败，请检查输入内容是否匹配所选格式", MudBlazor.Severity.Error);
            return;
        }

        this.inputedString = value.FromBytes(this.cachedBytes);
        this.selectedDisplay = value;

        _ = this.preferencesStorage?.Set(new(value.Name));
        _ = this.inputsStorage?.Set(new(this.inputedString, this.cachedBytes));
    }
    private void OnInputted(string value)
    {
        this.cachedBytes = null;
        this.inputedString = value;

        Debug.Assert(this.selectedDisplay is not null);

        _ = this.preferencesStorage?.Set(new(this.selectedDisplay.Name));
        _ = this.inputsStorage?.Set(new(value, null));
    }
    private async Task OnFilesChanged(IBrowserFile file)
    {
        using var memory = new MemoryStream();
        try
        {
            using var stream = file.OpenReadStream(file.Size);
            await stream.CopyToAsync(memory);
        }
        catch(Exception ex)
        {
            this.Logger.LogError(ex, "Failed to load the file.");
            _ = this.MudSnackbar.Add("读取文件时出现了错误", MudBlazor.Severity.Error);
            return;
        }
        this.cachedBytes = memory.ToArray();
        this.OnSelected(this.displays.Single(x => x.Name == "Base64"));
    }

    private async Task OnExportClicked()
    {
        if (!this.CacheBytes())
        {
            _ = this.MudSnackbar.Add("导出失败，请检查输入内容是否匹配所选格式", MudBlazor.Severity.Error);
            return;
        }
        _ = await this.FileDownloader.DownloadFileAsync("bytes.bin", this.cachedBytes);
    }
    private async Task OnQrCodeFilesChanged(IBrowserFile file)
    {
        try
        {
            using var stream = file.OpenReadStream(file.Size);
            var image = await Image.LoadAsync<Rgba32>(stream);
            LuminanceSource luminanceSource = new ImageSharpLuminanceSource<Rgba32>(image);
            var bitmap = new BinaryBitmap(new HybridBinarizer(luminanceSource));
            this.cachedBytes = new QRCodeReader().decode(bitmap).RawBytes;
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, "Failed to decode the QR code.");
            _ = this.MudSnackbar.Add("扫描二维码时出现了错误", MudBlazor.Severity.Error);
            return;
        }
        this.OnSelected(this.displays.Single(x => x.Name == "Base64"));
    }
}
