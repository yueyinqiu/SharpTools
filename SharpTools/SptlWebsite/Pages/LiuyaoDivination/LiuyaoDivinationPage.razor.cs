using System.Collections.Immutable;
using System.Text;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.EntityRelations.EntityCharacteristics.Extensions;
using YiJingFramework.EntityRelations.EntityStrings;
using YiJingFramework.EntityRelations.EntityStrings.Conversions;
using YiJingFramework.EntityRelations.EntityStrings.Extensions;
using YiJingFramework.EntityRelations.GuaCharacters.Extensions;
using YiJingFramework.EntityRelations.GuaHexagramBagongs.Extensions;
using YiJingFramework.EntityRelations.GuaHexagramNajias.Extensions;
using YiJingFramework.EntityRelations.WuxingRelations;
using YiJingFramework.EntityRelations.WuxingRelations.Extensions;
using YiJingFramework.PrimitiveTypes;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;
using static SptlWebsite.Components.InlineNongliSolarDateTimePicker;

namespace SptlWebsite.Pages.LiuyaoDivination;

public partial class LiuyaoDivinationPage
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
                return 动否 ? "重" : "单";
            else
                return 动否 ? "交" : "拆";
        }
    }

    private readonly 单拆重交?[] 设置的单拆重交 = [null, null, null, null, null, null];
    private readonly ImmutableArray<单拆重交?> 空单拆重交 = [
        null,
        单拆重交.单,
        单拆重交.拆,
        单拆重交.重,
        单拆重交.交
    ];

    private void 设置单拆重交(int 要改变的爻)
    {
        var 当前空单拆重交 = 空单拆重交.IndexOf(设置的单拆重交[要改变的爻]);
        var 改为空单拆重交 = (当前空单拆重交 + 1) % 空单拆重交.Length;
        设置的单拆重交[要改变的爻] = 空单拆重交[改为空单拆重交];

        var 内卦给定 = true;
        var 外卦给定 = true;
        var 本卦各爻 = new Yinyang[6];
        var 之卦各爻 = new Yinyang[6];
        for (int 爻 = 0; 爻 < 6; 爻++)
        {
            var 单拆重交 = 设置的单拆重交[爻];
            if (单拆重交 is null)
            {
                if (爻 < 3)
                    内卦给定 = false;
                else
                    外卦给定 = false;
                本卦爻阴阳[爻] = null;
                本卦爻动否[爻] = null;
                之卦爻阴阳[爻] = null;
            }
            else
            {
                本卦各爻[爻] = 单拆重交.阴阳;
                之卦各爻[爻] = 单拆重交.动否 ? !单拆重交.阴阳 : 单拆重交.阴阳;

                本卦爻阴阳[爻] = 本卦各爻[爻];
                本卦爻动否[爻] = 单拆重交.动否;
                之卦爻阴阳[爻] = 之卦各爻[爻];
            }
        }

        if (内卦给定 && 外卦给定)
        {
            var 本卦 = new GuaHexagram(本卦各爻);
            var 之卦 = new GuaHexagram(之卦各爻);

            var (本卦八宫, 本卦世应) = 本卦.Bagong();
            var 本卦五行 = 本卦八宫.Gong.Wuxing();
            var 世爻 = 本卦世应.Shiyao();
            var 应爻 = 本卦世应.Yingyao();

            var 本卦干支 = 本卦.Najia();
            var 之卦干支 = 之卦.Najia();

            HashSet<Wuxing> 缺失五行 = [Wuxing.Mu, Wuxing.Huo, Wuxing.Tu, Wuxing.Jin, Wuxing.Shui];
            for (int i = 0; i < 6; i++)
            {
                var 本卦爻地支 = 本卦干支[i].Dizhi;
                var 本卦爻五行 = 本卦爻地支.Wuxing();
                var 之卦爻地支 = 之卦干支[i].Dizhi;
                var 之卦爻五行 = 之卦爻地支.Wuxing();

                _ = 缺失五行.Remove(本卦爻五行);

                本卦爻天干[i] = 本卦干支[i].Tiangan;
                this.本卦爻地支[i] = 本卦爻地支;
                this.本卦爻五行[i] = 本卦爻五行;

                之卦爻天干[i] = 之卦干支[i].Tiangan;
                this.之卦爻地支[i] = 之卦爻地支;
                this.之卦爻五行[i] = 之卦爻五行;

                本卦爻六亲[i] = 本卦五行.GetRelation(本卦爻五行);
                之卦爻六亲[i] = 本卦五行.GetRelation(之卦爻五行);

                if (i == 世爻)
                    本卦爻世应真世假应[i] = true;
                else if (i == 应爻)
                    本卦爻世应真世假应[i] = false;
                else
                    本卦爻世应真世假应[i] = null;
            }

            var 八纯卦 = new GuaHexagram(本卦八宫.Gong.Concat(本卦八宫.Gong));
            var 八纯卦纳甲 = 八纯卦.Najia();
            for (int i = 0; i < 6; i++)
            {
                var 伏神爻地支 = 八纯卦纳甲[i].Dizhi;
                var 伏神爻五行 = 伏神爻地支.Wuxing();
                if (!缺失五行.Contains(伏神爻五行))
                {
                    伏神爻天干[i] = null;
                    this.伏神爻地支[i] = null;
                    this.伏神爻五行[i] = null;
                    伏神爻六亲[i] = null;
                    continue;
                }
                伏神爻天干[i] = 八纯卦纳甲[i].Tiangan;
                this.伏神爻地支[i] = 伏神爻地支;
                this.伏神爻五行[i] = 伏神爻五行;
                伏神爻六亲[i] = 本卦五行.GetRelation(伏神爻五行);
            }

            this.本卦 = zhouyi.GetHexagram(本卦);
            this.之卦 = zhouyi.GetHexagram(之卦);
        }
        else if (内卦给定)
        {
            var 本卦 = new GuaHexagram(
                本卦各爻[0], 本卦各爻[1], 本卦各爻[2],
                本卦各爻[0], 本卦各爻[1], 本卦各爻[2]);
            var 之卦 = new GuaHexagram(
                之卦各爻[0], 之卦各爻[1], 之卦各爻[2],
                之卦各爻[0], 之卦各爻[1], 之卦各爻[2]);
            var 本卦干支 = 本卦.Najia();
            var 之卦干支 = 之卦.Najia();
            for (int i = 0; i < 3; i++)
            {
                var 本卦爻地支 = 本卦干支[i].Dizhi;
                var 本卦爻五行 = 本卦爻地支.Wuxing();
                var 之卦爻地支 = 之卦干支[i].Dizhi;
                var 之卦爻五行 = 之卦爻地支.Wuxing();

                本卦爻天干[i] = 本卦干支[i].Tiangan;
                this.本卦爻地支[i] = 本卦爻地支;
                this.本卦爻五行[i] = 本卦爻五行;

                之卦爻天干[i] = 之卦干支[i].Tiangan;
                this.之卦爻地支[i] = 之卦爻地支;
                this.之卦爻五行[i] = 之卦爻五行;
            }
            for (int i = 3; i < 6; i++)
            {
                本卦爻天干[i] = null;
                本卦爻地支[i] = null;
                本卦爻五行[i] = null;

                之卦爻天干[i] = null;
                之卦爻地支[i] = null;
                之卦爻五行[i] = null;
            }

            for (int i = 0; i < 6; i++)
            {
                本卦爻六亲[i] = null;
                之卦爻六亲[i] = null;
                本卦爻世应真世假应[i] = null;

                伏神爻天干[i] = null;
                伏神爻地支[i] = null;
                伏神爻五行[i] = null;
                伏神爻六亲[i] = null;
            }

            本卦 = null;
            之卦 = null;
        }
        else if (外卦给定)
        {
            var 本卦 = new GuaHexagram(
                本卦各爻[3], 本卦各爻[4], 本卦各爻[5],
                本卦各爻[3], 本卦各爻[4], 本卦各爻[5]);
            var 之卦 = new GuaHexagram(
                之卦各爻[3], 之卦各爻[4], 之卦各爻[5],
                之卦各爻[3], 之卦各爻[4], 之卦各爻[5]);
            var 本卦干支 = 本卦.Najia();
            var 之卦干支 = 之卦.Najia();
            for (int i = 0; i < 3; i++)
            {
                本卦爻天干[i] = null;
                本卦爻地支[i] = null;
                本卦爻五行[i] = null;

                之卦爻天干[i] = null;
                之卦爻地支[i] = null;
                之卦爻五行[i] = null;
            }
            for (int i = 3; i < 6; i++)
            {
                var 本卦爻地支 = 本卦干支[i].Dizhi;
                var 本卦爻五行 = 本卦爻地支.Wuxing();
                var 之卦爻地支 = 之卦干支[i].Dizhi;
                var 之卦爻五行 = 之卦爻地支.Wuxing();

                本卦爻天干[i] = 本卦干支[i].Tiangan;
                this.本卦爻地支[i] = 本卦爻地支;
                this.本卦爻五行[i] = 本卦爻五行;

                之卦爻天干[i] = 之卦干支[i].Tiangan;
                this.之卦爻地支[i] = 之卦爻地支;
                this.之卦爻五行[i] = 之卦爻五行;
            }

            for (int i = 0; i < 6; i++)
            {
                本卦爻六亲[i] = null;
                之卦爻六亲[i] = null;
                本卦爻世应真世假应[i] = null;

                伏神爻天干[i] = null;
                伏神爻地支[i] = null;
                伏神爻五行[i] = null;
                伏神爻六亲[i] = null;
            }

            本卦 = null;
            之卦 = null;
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                本卦爻天干[i] = null;
                本卦爻地支[i] = null;
                本卦爻五行[i] = null;

                之卦爻天干[i] = null;
                之卦爻地支[i] = null;
                之卦爻五行[i] = null;

                本卦爻六亲[i] = null;
                之卦爻六亲[i] = null;
                本卦爻世应真世假应[i] = null;

                伏神爻天干[i] = null;
                伏神爻地支[i] = null;
                伏神爻五行[i] = null;
                伏神爻六亲[i] = null;
            }
            本卦 = null;
            之卦 = null;
        }
    }

    private readonly EntityToStringConversion<Yinyang> 阴阳文本
        = (x) =>
        {
            return new GuaWith1Yao(x).ToUnicodeChar().ToString();
        };
    private readonly EntityToStringConversion<bool> 世应文本
        = (x) =>
        {
            return x ? "世" : "应";
        };

    private string 本卦爻字符串(int 爻)
    {
        var builder = new StringBuilder();

        void Append(string? s, int length)
        {
            if (s is null)
            {
                _ = builder.Append('　', length);
                return;
            }
            _ = builder.Append('　', length - s.Length).Append(s);
        }

        Append(本卦爻世应真世假应[爻]?.ToString(世应文本), 1);

        Append(本卦爻六亲[爻]?.ToString(WuxingRelationToStringConversions.Liuqin), 3);

        Append(本卦爻天干[爻]?.ToString("C"), 2);
        Append(本卦爻地支[爻]?.ToString("C"), 1);
        Append(本卦爻五行[爻]?.ToString("C"), 1);

        Append(本卦爻阴阳[爻]?.ToString(阴阳文本), 2);

        return builder.ToString();
    }

    private string 之卦爻字符串(int 爻)
    {
        var builder = new StringBuilder();

        void Append(string? s, int length)
        {
            if (s is null)
            {
                _ = builder.Append('　', length);
                return;
            }
            _ = builder.Append('　', length - s.Length).Append(s);
        }

        Append(本卦爻动否[爻] is true ? "动" : null, 1);

        Append(之卦爻六亲[爻]?.ToString(WuxingRelationToStringConversions.Liuqin), 3);

        Append(之卦爻天干[爻]?.ToString("C"), 2);
        Append(之卦爻地支[爻]?.ToString("C"), 1);
        Append(之卦爻五行[爻]?.ToString("C"), 1);

        Append(之卦爻阴阳[爻]?.ToString(阴阳文本), 2);

        return builder.ToString();
    }

    private SelectedNongliSolarDateTime nongliSolar = SelectedNongliSolarDateTime.Empty;

    // 选择单个爻后就可以显示
    private readonly Yinyang?[] 本卦爻阴阳 = [null, null, null, null, null, null];
    private readonly bool?[] 本卦爻动否 = [null, null, null, null, null, null];
    private readonly Yinyang?[] 之卦爻阴阳 = [null, null, null, null, null, null];

    // 在内卦或外卦确定后即可显示
    private readonly Tiangan?[] 本卦爻天干 = [null, null, null, null, null, null];
    private readonly Dizhi?[] 本卦爻地支 = [null, null, null, null, null, null];
    private readonly Wuxing?[] 本卦爻五行 = [null, null, null, null, null, null];
    private readonly Dizhi?[] 之卦爻地支 = [null, null, null, null, null, null];
    private readonly Tiangan?[] 之卦爻天干 = [null, null, null, null, null, null];
    private readonly Wuxing?[] 之卦爻五行 = [null, null, null, null, null, null];

    // 整个卦都确定后才能知道
    private ZhouyiHexagram? 本卦;
    private ZhouyiHexagram? 之卦;
    private readonly bool?[] 本卦爻世应真世假应 = [null, null, null, null, null, null];
    private readonly WuxingRelation?[] 本卦爻六亲 = [null, null, null, null, null, null];
    private readonly WuxingRelation?[] 之卦爻六亲 = [null, null, null, null, null, null];
    private readonly Tiangan?[] 伏神爻天干 = [null, null, null, null, null, null];
    private readonly Dizhi?[] 伏神爻地支 = [null, null, null, null, null, null];
    private readonly Wuxing?[] 伏神爻五行 = [null, null, null, null, null, null];
    private readonly WuxingRelation?[] 伏神爻六亲 = [null, null, null, null, null, null];

    private ZhouyiStore zhouyi = new ZhouyiStore(null);
    protected override async Task OnParametersSetAsync()
    {
        zhouyi = await this.BuiltInZhouyi.GetZhouyiAsync();
    }
}