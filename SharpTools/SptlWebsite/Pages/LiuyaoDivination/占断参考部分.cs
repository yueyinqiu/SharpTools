using System.Text;
using YiJingFramework.EntityRelations.EntityCharacteristics.Extensions;
using YiJingFramework.EntityRelations.EntityStrings.Conversions;
using YiJingFramework.EntityRelations.EntityStrings.Extensions;
using YiJingFramework.EntityRelations.GuaCharacters.Extensions;
using YiJingFramework.EntityRelations.GuaHexagramBagongs;
using YiJingFramework.EntityRelations.GuaHexagramBagongs.Extensions;
using YiJingFramework.EntityRelations.TianganNianYuesAndRishis.Extensions;
using YiJingFramework.Nongli.Extensions;
using YiJingFramework.Nongli.Lunar;
using YiJingFramework.Nongli.Solar;
using YiJingFramework.PrimitiveTypes;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;
using static SptlWebsite.Components.InlineNongliLunarDateTimePicker;
using static SptlWebsite.Components.InlineNongliSolarDateTimePicker;

namespace SptlWebsite.Pages.LiuyaoDivination;

public partial class LiuyaoDivinationPage
{
    private string reference = "点击各卦或各爻可显示占断参考。";

    private void 复原占断参考()
    {
        this.reference = "点击各卦或各爻可显示占断参考。";
    }

    private string YaoTitle(int index0Based, Yinyang yinyang)
    {
        var 九六 = yinyang.IsYang ? '九' : '六';
        return index0Based switch
        {
            0 => $"初{九六}",
            1 => $"{九六}二",
            2 => $"{九六}三",
            3 => $"{九六}四",
            4 => $"{九六}五",
            _ => $"上{九六}",
        };
    }

    private void 显示占断参考本卦()
    {
        if (this.本卦 is null)
        {
            this.reference = "请先点击左侧问号选择单拆重交起出卦来。";
            return;
        }

        var builder = new StringBuilder();

        var 卦 = this.本卦;
        {
            var (上卦, 下卦) = 卦.SplitToTrigrams(this.zhouyi);
            if (上卦.Painting == 下卦.Painting)
            {
                _ = builder.AppendLine($"本卦 {卦.Name}为{上卦.Nature}");
            }
            else
            {
                _ = builder.AppendLine($"本卦 {上卦.Nature}{下卦.Nature}{卦.Name}");
            }
        }
        _ = builder.AppendLine();

        {
            _ = builder.AppendLine($"卦象：{卦.Painting.ToUnicodeChar()}");
            var (八宫, 世应) = 卦.Painting.Bagong();
            var 世应文本 = 世应 switch
            {
                Shiying.Yishi => "一世卦",
                Shiying.Ershi => "二世卦",
                Shiying.Sanshi => "三世卦",
                Shiying.Sishi => "四世卦",
                Shiying.Wushi => "五世卦",
                Shiying.Youhun => "游魂卦",
                Shiying.Guihun => "归魂卦",
                _ => "八纯卦",
            };
            _ = builder.AppendLine($"八宫：{this.zhouyi.GetTrigram(八宫.Gong).Name}宫{世应文本}");
        }
        _ = builder.AppendLine();

        {
            _ = builder.AppendLine($"卦辞：{卦.Text}");
            _ = builder.AppendLine($"大象：{卦.Xiang}");
            _ = builder.AppendLine($"彖传：{卦.Tuan}");
            if (卦.Painting == new GuaHexagram(
                Yinyang.Yang, Yinyang.Yang, Yinyang.Yang,
                Yinyang.Yang, Yinyang.Yang, Yinyang.Yang))
            {
                _ = builder.AppendLine($"用九：{卦.Yong.YaoText}");
                _ = builder.AppendLine($"小象：{卦.Yong.Xiang}");
                _ = builder.AppendLine($"文言：");
                _ = builder.AppendLine($"{卦.Wenyan}");
            }
            if (卦.Painting == new GuaHexagram(
                Yinyang.Yin, Yinyang.Yin, Yinyang.Yin,
                Yinyang.Yin, Yinyang.Yin, Yinyang.Yin))
            {
                _ = builder.AppendLine($"用六：{卦.Yong.YaoText}");
                _ = builder.AppendLine($"小象：{卦.Yong.Xiang}");
                _ = builder.AppendLine($"文言：{卦.Wenyan}");
            }
        }
        _ = builder.AppendLine();

        this.reference = builder.ToString();
    }
    private void 显示占断参考之卦()
    {
        if (this.之卦 is null)
        {
            this.reference = "请先点击左侧问号选择单拆重交起出卦来。";
            return;
        }

        var builder = new StringBuilder();

        var 卦 = this.之卦;
        {
            var (上卦, 下卦) = 卦.SplitToTrigrams(this.zhouyi);
            if (上卦.Painting == 下卦.Painting)
            {
                _ = builder.AppendLine($"之卦 {卦.Name}为{上卦.Nature}");
            }
            else
            {
                _ = builder.AppendLine($"之卦 {上卦.Nature}{下卦.Nature}{卦.Name}");
            }
        }
        _ = builder.AppendLine();

        {
            _ = builder.AppendLine($"卦象：{卦.Painting.ToUnicodeChar()}");
            var (八宫, 世应) = 卦.Painting.Bagong();
            var 世应文本 = 世应 switch
            {
                Shiying.Yishi => "一世卦",
                Shiying.Ershi => "二世卦",
                Shiying.Sanshi => "三世卦",
                Shiying.Sishi => "四世卦",
                Shiying.Wushi => "五世卦",
                Shiying.Youhun => "游魂卦",
                Shiying.Guihun => "归魂卦",
                _ => "八纯卦",
            };
            _ = builder.AppendLine($"八宫：{this.zhouyi.GetTrigram(八宫.Gong).Name}宫{世应文本}");
        }
        _ = builder.AppendLine();

        {
            _ = builder.AppendLine($"卦辞：{卦.Text}");
            _ = builder.AppendLine($"大象：{卦.Xiang}");
            _ = builder.AppendLine($"彖传：{卦.Tuan}");
            if (卦.Painting == new GuaHexagram(
                Yinyang.Yang, Yinyang.Yang, Yinyang.Yang,
                Yinyang.Yang, Yinyang.Yang, Yinyang.Yang))
            {
                _ = builder.AppendLine($"用九：{卦.Yong.YaoText}");
                _ = builder.AppendLine($"小象：{卦.Yong.Xiang}");
                _ = builder.AppendLine($"文言：");
                _ = builder.AppendLine($"{卦.Wenyan}");
            }
            if (卦.Painting == new GuaHexagram(
                Yinyang.Yin, Yinyang.Yin, Yinyang.Yin,
                Yinyang.Yin, Yinyang.Yin, Yinyang.Yin))
            {
                _ = builder.AppendLine($"用六：{卦.Yong.YaoText}");
                _ = builder.AppendLine($"小象：{卦.Yong.Xiang}");
                _ = builder.AppendLine($"文言：{卦.Wenyan}");
            }
        }
        _ = builder.AppendLine();

        this.reference = builder.ToString();
    }
    private void 显示占断参考本卦爻(int 爻)
    {
        if (this.本卦 is null)
        {
            this.reference = "请先点击左侧问号选择单拆重交起出卦来。";
            return;
        }

        var builder = new StringBuilder();

        _ = builder.AppendLine(
            $"本卦{this.YaoTitle(爻, this.本卦.Painting[爻])} " +
            $"{this.本卦爻六亲[爻]?.ToString(WuxingRelationToStringConversions.Liuqin)}" +
            $"{this.本卦爻天干[爻]:C}" +
            $"{this.本卦爻地支[爻]:C}" +
            $"{this.本卦爻五行[爻]:C}");
        _ = builder.AppendLine();

        var 爻周易 = this.本卦.EnumerateYaos().ElementAt(爻);
        _ = builder.AppendLine($"爻辞：{爻周易.YaoText}");
        _ = builder.AppendLine($"小象：{爻周易.Xiang}");
        _ = builder.AppendLine();

        this.reference = builder.ToString();
    }
    private void 显示占断参考伏神爻(int 爻)
    {
        if (this.本卦 is null)
        {
            this.reference = "请先点击左侧问号选择单拆重交起出卦来。";
            return;
        }

        var builder = new StringBuilder();

        _ = builder.AppendLine(
            $"本卦{this.YaoTitle(爻, this.本卦.Painting[爻])}伏神 " +
            $"{this.伏神爻六亲[爻]?.ToString(WuxingRelationToStringConversions.Liuqin)}" +
            $"{this.伏神爻天干[爻]:C}" +
            $"{this.伏神爻地支[爻]:C}" +
            $"{this.伏神爻五行[爻]:C}");
        _ = builder.AppendLine();

        this.reference = builder.ToString();
    }
    private void 显示占断参考之卦爻(int 爻)
    {
        if (this.之卦 is null)
        {
            this.reference = "请先点击左侧问号选择单拆重交起出卦来。";
            return;
        }

        var builder = new StringBuilder();

        _ = builder.AppendLine(
            $"之卦{this.YaoTitle(爻, this.之卦.Painting[爻])} " +
            $"{this.之卦爻六亲[爻]?.ToString(WuxingRelationToStringConversions.Liuqin)}" +
            $"{this.之卦爻天干[爻]:C}" +
            $"{this.之卦爻地支[爻]:C}" +
            $"{this.之卦爻五行[爻]:C}");
        _ = builder.AppendLine();

        var 爻周易 = this.之卦.EnumerateYaos().ElementAt(爻);
        _ = builder.AppendLine($"爻辞：{爻周易.YaoText}");
        _ = builder.AppendLine($"小象：{爻周易.Xiang}");
        _ = builder.AppendLine();

        this.reference = builder.ToString();
    }
}