using Microsoft.FluentUI.AspNetCore.Components;
using SixLabors.ImageSharp;
using System.Collections.Immutable;

namespace SptlWebsite.Pages;

partial class QrCodeScannerPage
{
    private ImmutableArray<string> outputs = ["这里会以 Base64 字符串的形式显示扫描结果。"];
    private string activeTab = "tab0";

    private void Input(IEnumerable<FluentInputFileEventArgs> files)
    {
        var file = files.Single();
        if (file.LocalFile is null)
        {
            outputs = [$"文件导入失败：{Environment.NewLine}{file.ErrorMessage}"];
            activeTab = "tab0";
            return;
        }

        Image image;
        try
        {
            image = Image.Load(file.LocalFile.FullName);
        }
        catch (Exception ex)
        {
            outputs = [$"图像解析失败：{Environment.NewLine}{ex}"];
            activeTab = "tab0";
            file.LocalFile.Delete();
            return;
        }

        byte[][]? bytes;
        try
        {
            bytes = Qr.ImageDecoder(image);
        }
        catch (Exception ex)
        {
            outputs = [$"Qr 码识别失败：{Environment.NewLine}{ex}"];
            activeTab = "tab0";
            file.LocalFile.Delete();
            return;
        }

        // 找不到的时候会返回 null 而不是零个元素
        if (bytes is null)
            outputs = [$"识别失败或图像中不存在 Qr 码。"];
        else
            outputs = bytes.Select(Convert.ToBase64String).ToImmutableArray();
        activeTab = "tab0";
        file.LocalFile.Delete();
    }
}