using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SharpTools.Tools;

public partial class Bytes
{
    private string? inputedString;
    private Display? selectedDisplay;

    private sealed record Display(
        string Name, Func<string, byte[]> ToBytes, Func<byte[], string> FromBytes);

    private readonly ImmutableArray<Display> displays =
        [
            new("Base64", Convert.FromBase64String, Convert.ToBase64String),
            new("Hex", Convert.FromHexString, Convert.ToHexString),
            new("UTF-8 文本", Encoding.UTF8.GetBytes, Encoding.UTF8.GetString),
            new("字节数组",
                (s) =>
                {
                    s = s.Trim().Trim('[', ']');
                    var strings = s.Split(',');
                    if (strings.Length is 1 && string.IsNullOrWhiteSpace(strings[0]))
                        return Array.Empty<byte>();
                    return strings.Select(x => byte.Parse(x)).ToArray();
                },
                (b) =>
                {
                    var result = string.Join(", ", b);
                    return $"[{result}]";
                }),
        ];

    private sealed record Preferences(string? DisplayName, string? Input);
    protected override async Task OnParametersSetAsync()
    {
        // 同步运行会导致输出框的 AutoGrow 不能正常工作，不知道是什么原因。
        await Task.Yield();

        try
        {
            var preference = LocalStorage.GetItem<Preferences>("sharptools.preferences.bytes");
            this.inputedString = preference.Input;
            this.selectedDisplay = displays.Single(x => x.Name == preference.DisplayName);
        }
        catch
        {
            this.selectedDisplay = displays.Single(x => x.Name == "Base64");
            var bytes = Encoding.UTF8.GetBytes("你好！");
            this.inputedString = this.selectedDisplay.FromBytes(bytes);
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
                this.cachedBytes = Array.Empty<byte>();
            else
                this.cachedBytes = selectedDisplay.ToBytes(this.inputedString);
            return true;
        }
        catch
        {
            return false;
        }
    }
    private void OnSelected(Display value)
    {
        if (!this.CacheBytes())
        {
            MudSnackbar.Add("转换失败，请检查输入内容是否匹配所选格式", MudBlazor.Severity.Error);
            return;
        }

        this.inputedString = value.FromBytes(this.cachedBytes);
        this.selectedDisplay = value;

        LocalStorage.SetItem(
            "sharptools.preferences.bytes",
            new Preferences(value.Name, this.inputedString));
    }
    private void OnInputted(string value)
    {
        this.cachedBytes = null;
        this.inputedString = value;

        Debug.Assert(this.selectedDisplay is not null);
        LocalStorage.SetItem(
            "sharptools.preferences.bytes",
            new Preferences(this.selectedDisplay.Name, value));
    }
    private async Task OnFilesChanged(IBrowserFile file)
    {
        using var memory = new MemoryStream();
        try
        {
            using var stream = file.OpenReadStream(file.Size);
            await stream.CopyToAsync(memory);
        }
        catch
        {
            MudSnackbar.Add("读取文件时出现了错误", MudBlazor.Severity.Error);
            return;
        }
        this.cachedBytes = memory.ToArray();
        OnSelected(displays.Single(x => x.Name == "Base64"));
    }
    private async Task OnExportClicked()
    {
        if (!CacheBytes())
        {
            MudSnackbar.Add("导出失败，请检查输入内容是否匹配所选格式", MudBlazor.Severity.Error);
            return;
        }
        await FileDownloader.DownloadFileAsync("bytes.bin", this.cachedBytes);
    }
}
