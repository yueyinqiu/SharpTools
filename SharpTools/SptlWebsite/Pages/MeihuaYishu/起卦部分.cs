using Microsoft.JSInterop;
using System.Diagnostics;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.EntityRelations.GuaDerivations.Extensions;
using YiJingFramework.Nongli.Lunar;
using YiJingFramework.Nongli.Solar;
using YiJingFramework.PrimitiveTypes;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;
using static SptlWebsite.Components.InlineNongliLunarDateTimePicker;
using static SptlWebsite.Components.InlineNongliSolarDateTimePicker;

namespace SptlWebsite.Pages.MeihuaYishu;

public partial class MeihuaYishuPage
{
    private string upperInput = "年+月+日";
    private string lowerInput = "年月日加时总数";
    private string changingInput = "下卦卦数";

    private const string defaultScript =
        """
        // https://github.com/yueyinqiu/SharpTools/blob/main/SharpTools/SptlWebsite/Pages/MeihuaYishu/MeihuaYishuPage.razor.js

        const 年 = nongliLunar.nian?.index ?? NaN;
        const 月 = nongliLunar.yue ?? NaN;
        const 日 = nongliLunar.ri ?? NaN;
        const 时 = nongliLunar.shi?.index ?? NaN;

        const 年月日加时总数 = 年 + 月 + 日 + 时;

        const 乾 = 1;
        const 兑 = 2;
        const 离 = 3;
        const 震 = 4;
        const 巽 = 5;
        const 坎 = 6;
        const 艮 = 7;
        const 坤 = 8;

        try
        {
            outputs.shanggua = eval(shangguaInput);
        }
        catch (ex)
        {
            outputs.error = `上卦计算失败。请检查输入的时间是否完整，以及上卦表达式是否正确。详细信息：${ex}`;
            return;
        }
        if (!Number.isInteger(outputs.shanggua))
        {
            outputs.error = `计算得到的上卦卦数并非整数。请检查输入的时间是否完整，以及上卦表达式是否正确。具体值：${outputs.shanggua}`;
            return;
        }
        const 上卦卦数 = outputs.shanggua;

        try
        {
            outputs.xiagua = eval(xiaguaInput);
        }
        catch (ex)
        {
            outputs.error = `下卦计算失败。请检查输入的时间是否完整，以及下卦表达式是否正确。详细信息：${ex}`;
            return;
        }
        if (!Number.isInteger(outputs.xiagua))
        {
            outputs.error = `计算得到的下卦卦数并非整数。请检查输入的时间是否完整，以及下卦表达式是否正确。具体值：${outputs.xiagua}`;
            return;
        }
        const 下卦卦数 = outputs.xiagua;

        try
        {
            outputs.dongyao = eval(dongyaoInput);
        }
        catch (ex)
        {
            outputs.error = `动爻计算失败。请检查输入的时间是否完整，以及动爻表达式是否正确。详细信息：${ex}`;
            return;
        }
        if (!Number.isInteger(outputs.dongyao))
        {
            outputs.error = `计算得到的动爻数并非整数。请检查输入的时间是否完整，以及下卦表达式是否正确。具体值：${outputs.dongyao}`;
            return;
        }

        outputs.error = null;
        return;
        """;
    private string script = defaultScript;

    private sealed class RawTools
    {
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable CA1822 // 将成员标记为 static
        [JSInvokable]
        public RawInputs.RawNongliLunarInputs GetNongliLunarFromGregorian(DateTime dateTime)
        {
            var lunar = LunarDateTime.FromGregorian(dateTime);
            var selected = new SelectedNongliLunarDateTime(lunar);
            return new RawInputs.RawNongliLunarInputs(selected);
        }

        [JSInvokable]
        public RawInputs.RawNongliSolarInputs GetNongliSolarFromGregorian(DateTime dateTime)
        {
            var lunar = SolarDateTime.FromGregorian(dateTime);
            var selected = new SelectedNongliSolarDateTime(lunar);
            return new RawInputs.RawNongliSolarInputs(selected);
        }
#pragma warning restore CA1822 // 将成员标记为 static
#pragma warning restore IDE0079 // 请删除不必要的忽略
    }

    private sealed record RawInputs(
        RawInputs.RawNongliLunarInputs NongliLunar,
        RawInputs.RawNongliSolarInputs NongliSolar,
        DateTime? GregorianCalendar,
        string Script,
        string Shanggua,
        string Xiagua,
        string Dongyao)
    {
        public sealed record RawNongliLunarInputs(
            int? Nian, int? Yue, bool? IsRunyue, int? Ri, int? Shi)
        {
            public RawNongliLunarInputs(SelectedNongliLunarDateTime selected)
                : this(selected.Nian?.Index, selected.Yue,
                      selected.IsRunyue, selected.Ri,
                      selected.Shi?.Index)
            { }
        }
        public sealed record RawNongliSolarInputs(
            int? Niangan, int? Nianzhi,
            int? Yuegan, int? Yuezhi,
            int? Rigan, int? Rizhi,
            int? Shigan, int? Shizhi)
        {
            public RawNongliSolarInputs(SelectedNongliSolarDateTime selected)
                : this(selected.Niangan?.Index, selected.Nianzhi?.Index,
                      selected.Yuegan?.Index, selected.Yuezhi?.Index,
                      selected.Rigan?.Index, selected.Rizhi?.Index,
                      selected.Shigan?.Index, selected.Shizhi?.Index)
            { }
        }
    }

    private sealed record RawOutputs(
        string? Error, string? Warning, int Shanggua, int Xiagua, int Dongyao);


    private string? errorOrWarning = "还未起卦。";

    private string? currentUpperInput = null;
    private string? currentLowerInput = null;
    private string? currentChangingInput = null;

    private int? upperNumber = null;
    private int? lowerNumber = null;
    private int? changingNumber = null;

    private ZhouyiHexagram? 本卦 = null;
    private ZhouyiHexagram? 互卦 = null;
    private ZhouyiHexagram? 变卦 = null;

    private DateTime? currentWestern;
    private SelectedNongliLunarDateTime? currentNongliLunar;
    private SelectedNongliSolarDateTime? currentNongliSolar;

    private static GuaTrigram 按先天数取卦(int 先天数)
    {
        return ((先天数 % 8 + 8) % 8) switch
        {
            1 => new(Yinyang.Yang, Yinyang.Yang, Yinyang.Yang),
            2 => new(Yinyang.Yang, Yinyang.Yang, Yinyang.Yin),
            3 => new(Yinyang.Yang, Yinyang.Yin, Yinyang.Yang),
            4 => new(Yinyang.Yang, Yinyang.Yin, Yinyang.Yin),
            5 => new(Yinyang.Yin, Yinyang.Yang, Yinyang.Yang),
            6 => new(Yinyang.Yin, Yinyang.Yang, Yinyang.Yin),
            7 => new(Yinyang.Yin, Yinyang.Yin, Yinyang.Yang),
            _ => new(Yinyang.Yin, Yinyang.Yin, Yinyang.Yin),
        };
    }

    private ZhouyiHexagram? displayingGua;

    public void GetGuas()
    {
        DateTime? western;
        if (this.WesternDate.HasValue && this.WesternTime.HasValue)
        {
            var date = this.WesternDate.Value.Date;
            var time = this.WesternTime.Value.TimeOfDay;
            western = date.Add(time);
        }
        else
        {
            western = null;
        }

        var rawInputs = new RawInputs(
            new(this.NongliLunar), new(this.NongliSolar), western,
            this.script,
            this.upperInput, this.lowerInput, this.changingInput);
        using var rawTools = DotNetObjectReference.Create(new RawTools());

        Debug.Assert(this.jsModule is not null);
        var result = this.jsModule.Invoke<RawOutputs>("calculate", [
            rawInputs, new RawTools()
        ]);

        if (result.Error is not null)
        {
            this.errorOrWarning = result.Error;
            this.upperNumber = null;
            this.lowerNumber = null;
            this.changingNumber = null;
            this.本卦 = null;
            this.互卦 = null;
            this.变卦 = null;
            this.displayingGua = null;

            this.currentWestern = null;
            this.currentNongliLunar = null;
            this.currentNongliSolar = null;

            this.currentUpperInput = null;
            this.currentLowerInput = null;
            this.currentChangingInput = null;
        }
        else
        {
            this.errorOrWarning = result.Warning;
            this.upperNumber = result.Shanggua;
            this.lowerNumber = result.Xiagua;
            this.changingNumber = result.Dongyao;

            var 上卦 = 按先天数取卦(result.Shanggua);
            var 下卦 = 按先天数取卦(result.Xiagua);

            this.本卦 = this.zhouyi[new(下卦.Concat(上卦))];
            this.互卦 = this.zhouyi[this.本卦.Painting.Hugua()];
            this.变卦 = this.zhouyi[this.本卦.Painting.ChangeYaos(((result.Dongyao - 1) % 6 + 6) % 6)];
            this.displayingGua = null;

            this.currentWestern = western;
            this.currentNongliLunar = this.NongliLunar;
            this.currentNongliSolar = this.NongliSolar;

            this.currentUpperInput = this.upperInput;
            this.currentLowerInput = this.lowerInput;
            this.currentChangingInput = this.changingInput;

            this.SavePreferences();
        }
    }
}