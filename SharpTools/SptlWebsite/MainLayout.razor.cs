using Microsoft.FluentUI.AspNetCore.Components;
using SptlServices.GradedLocalStoraging;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace SptlWebsite;

public partial class MainLayout
{
    private static readonly ImmutableArray<ToolGroup> Groups = [
        new ToolGroup("编程工具", "bian1 cheng2 gong1 ju4", [
            new ToolEntry("Guid 生成器", "guid sheng1 cheng2 qi4", "/GuidGenerator"),
            new ToolEntry("字节数组表示", "zi4 jie2 shu4 zu3 biao3 shi4", "/BytesRepresentations"),
            new ToolEntry("Qr 码扫描器", "qr ma3 sao3 miao2 qi4", "/QrCodeScanner"),
            new ToolEntry("拼音转换器", "pin1 yin1 zhuan3 huan4 qi4", "/PinyinConverter"),
        ]),
        new ToolGroup("易学工具", "yi4 xue2 gong1 ju4", [
            new ToolEntry("六爻预测", "liu4 yao2 yu4 ce4", "/LiuyaoDivination"),
            new ToolEntry("一日一卦", "yi1 ri4 yi1 gua4", "/OneHexagramPerDay"),
            new ToolEntry("梅花易数", "mei2 hua1 yi4 shu4", "/MeihuaYishu"),
        ]),
    ];

    private static readonly ImmutableArray<Icon> alphabetIcons = [
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaABoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaBBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaCBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaDBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaEBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaFBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaGBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaHBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaIBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaJBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaKBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaLBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaMBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaNBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaOBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaPBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaQBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaRBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaSBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaTBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaUBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaVBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaWBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaXBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaYBoxOutline,
        FluentUiBlazorMdiSvgIcons.MdiSvg.AlphaZBoxOutline,
    ];

    private sealed record ToolEntry(string Title, string Pinyin, string Href)
    {
        public Icon Icon => alphabetIcons[this.Pinyin[0] - 'a'];
    }
    private sealed record ToolGroup(string Title, string Pinyin, ImmutableArray<ToolEntry> Entries)
    {
        public Icon Icon => alphabetIcons[this.Pinyin[0] - 'a'];
    }

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

    private bool showNavMeau = true;
}