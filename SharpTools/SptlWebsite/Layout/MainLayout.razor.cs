using Microsoft.FluentUI.AspNetCore.Components;
using SptlServices.GradedLocalStoraging;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace SptlWebsite.Layout;

public partial class MainLayout
{
    private static readonly ImmutableArray<Icon> alphabetIcons = [
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaABoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaBBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaCBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaDBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaEBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaFBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaGBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaHBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaIBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaJBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaKBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaLBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaMBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaNBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaOBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaPBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaQBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaRBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaSBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaTBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaUBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaVBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaWBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaXBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaYBoxOutline(IconVariant.Regular, IconSize.Size24),
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaZBoxOutline(IconVariant.Regular, IconSize.Size24)
    ];

    private sealed record ToolEntry(string Title, string Pinyin, string Href)
    {
        public Icon Icon => alphabetIcons[this.Pinyin[0] - 'a'];
    }
    private sealed record ToolGroup(string Title, string Pinyin, ImmutableArray<ToolEntry> Entries)
    {
        public Icon Icon => alphabetIcons[this.Pinyin[0] - 'a'];
    }

    private static readonly ImmutableArray<ToolGroup> Groups = [
        new ToolGroup("编程工具", "bian1 cheng2 gong1 ju4", [
            new ToolEntry("Guid 生成器", "guid sheng1 cheng2 qi4", "/GuidGenerator"),
            new ToolEntry("字节数组表示", "zi4 jie2 shu4 zu3 biao3 shi4", "/BytesRepresentations"),
            new ToolEntry("Qr 码扫描器", "qr ma3 sao3 miao2 qi4", "/QrCodeScanner"),
        ]),
        new ToolGroup("易学工具", "yi4 xue2 gong1 ju4", [
            new ToolEntry("六爻预测", "liu4 yao2 yu4 ce4", "/LiuyaoDivination"),
            new ToolEntry("一日一卦", "yi2 ri4 yi2 gua4", "/OneHexagramPerDay"),
        ]),
    ];

    static MainLayout()
    {
        var orderedGroups = new List<ToolGroup>();
        foreach (var group in Groups)
        {
            var ordered = group.Entries.OrderBy(x => x.Pinyin).ToImmutableArray();
            orderedGroups.Add(group with { Entries = ordered });
        }
        Groups = [.. orderedGroups.OrderBy(x => x.Pinyin)];
    }

    private ILocalStorageEntry<double> NavMenuRatioStorageEntry =>
        this.LocalStorage.GetEntry<double>("MainLayout.NavMenuRatio", 1000);

    private string panel1Size = "250px";

    // https://github.com/microsoft/fluentui-blazor/issues/2858
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SplitterResizedEventArgs))]
    private void SaveNavMenuRatio(SplitterResizedEventArgs e)
    {
        var ratio = (double)e.Panel1Size / (e.Panel1Size + e.Panel2Size);
        this.NavMenuRatioStorageEntry.Set(ratio);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (this.NavMenuRatioStorageEntry.TryGet(out var navMenuSize))
        {
            this.panel1Size = $"{navMenuSize * 100}%";
            this.StateHasChanged();
        }
    }
}