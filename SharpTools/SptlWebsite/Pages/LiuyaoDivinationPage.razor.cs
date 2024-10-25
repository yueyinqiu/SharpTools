using System.Collections.Immutable;
using YiJingFramework.PrimitiveTypes;

namespace SptlWebsite.Pages;

partial class LiuyaoDivinationPage
{
    private record 单拆重交(Yinyang 阴阳, bool 动否)
    {
        public static 单拆重交 单 => new 单拆重交(Yinyang.Yang, false);
        public static 单拆重交 拆 => new 单拆重交(Yinyang.Yin, false);
        public static 单拆重交 重 => new 单拆重交(Yinyang.Yang, true);
        public static 单拆重交 交 => new 单拆重交(Yinyang.Yin, true);

        public override string ToString()
        {
            if (阴阳.IsYang)
                return 动否 ? "单" : "重";
            else
                return 动否 ? "拆" : "交";
        }
    }

    private ImmutableArray<单拆重交?> selectedLines = [null, null, null, null, null, null];
    private readonly ImmutableArray<单拆重交?> 空单拆重交 = [
        null,
        单拆重交.单, 
        单拆重交.拆, 
        单拆重交.重, 
        单拆重交.交
    ];
    private void SwitchSelectedLine(int line)
    {
        var current = 空单拆重交.IndexOf(selectedLines[line]);
        var next = (current + 1) % 空单拆重交.Length;
        selectedLines = selectedLines.SetItem(line, 空单拆重交[next]);
    }
}