using System.Text;
using YiJingFramework.EntityRelations.EntityCharacteristics.Extensions;
using YiJingFramework.EntityRelations.TianganNianYuesAndRishis.Extensions;
using YiJingFramework.Nongli.Extensions;
using YiJingFramework.Nongli.Lunar;
using YiJingFramework.Nongli.Solar;
using YiJingFramework.PrimitiveTypes;
using static SptlWebsite.Components.InlineNongliLunarDateTimePicker;
using static SptlWebsite.Components.InlineNongliSolarDateTimePicker;

namespace SptlWebsite.Pages.LiuyaoDivination;

public partial class LiuyaoDivinationPage
{
    private enum 六神
    {
        青龙,
        朱雀,
        勾陈,
        螣蛇,
        白虎,
        玄武
    }

    private SelectedNongliSolarDateTime nongliSolarDontTouchMe = SelectedNongliSolarDateTime.Empty;
    private SelectedNongliSolarDateTime NongliSolar
    {
        get
        {
            return nongliSolarDontTouchMe;
        }
        set
        {
            nongliSolarDontTouchMe = value;
            if (!value.Rigan.HasValue)
            {
                for (int i = 0; i < 6; i++)
                    各爻六神[i] = null;
            }
            else
            {
                var start = (int)value.Rigan.Value.Wuxing() switch
                {
                    0 => 六神.青龙, // 木
                    1 => 六神.朱雀, // 火
                    3 => 六神.白虎, // 金
                    4 => 六神.玄武, // 水
                    _ when value.Rigan.Value == Tiangan.Wu => 六神.勾陈, // 戊
                    _ => 六神.螣蛇, // 己
                };
                var current = (int)start;
                for (int i = 0; i < 6; i++)
                {
                    各爻六神[i] = (六神)current;
                    current = (current + 1) % 6;
                }
            }
            this.ValidateTime();
            this.复原占断参考();
        }
    }
    private readonly 六神?[] 各爻六神 = [null, null, null, null, null, null];

    private SelectedNongliLunarDateTime nongliLunarDontTouchMe = SelectedNongliLunarDateTime.Empty;
    private SelectedNongliLunarDateTime NongliLunar
    {
        get => nongliLunarDontTouchMe;
        set
        {
            nongliLunarDontTouchMe = value;
            ValidateTime();
            this.复原占断参考();
        }
    }
    private DateTime? westernDateDontTouchMe = null;
    private DateTime? WesternDate
    {
        get => westernDateDontTouchMe;
        set
        {
            westernDateDontTouchMe = value;
            ValidateTime();
            this.复原占断参考();
        }
    }
    private DateTime? westernTimeDontTouchMe = null;
    private DateTime? WesternTime
    {
        get => westernTimeDontTouchMe;
        set
        {
            westernTimeDontTouchMe = value;
            ValidateTime();
            this.复原占断参考();
        }
    }
    private (LunarDateTime lunar, SolarDateTime solar)? GetNongliFromWestern()
    {
        if (WesternDate.HasValue && WesternTime.HasValue)
        {
            var date = WesternDate.Value.Date;
            var time = WesternDate.Value.TimeOfDay;
            var dateTime = date.Add(time);

            return (LunarDateTime.FromGregorian(dateTime), 
                SolarDateTime.FromGregorian(dateTime));
        }
        return null;
    }
    
    private void FillNongli()
    {
        var value = GetNongliFromWestern();
        if (!value.HasValue)
            return;
        this.NongliLunar = new(value.Value.lunar);
        this.NongliSolar = new(value.Value.solar);
    }
    private void FillCurrent()
    {
        var now = DateTime.Now;
        this.WesternDate = now;
        this.WesternTime = now;
        this.FillNongli();
    }
    private void ClearDateTime()
    {
        this.WesternDate = null;
        this.WesternTime = null;
        this.NongliSolar = SelectedNongliSolarDateTime.Empty;
        this.NongliLunar = SelectedNongliLunarDateTime.Empty;
    }

    private string timeWarnings = "未填入时间。时间乃断卦必须。";
    private void ValidateTime()
    {
        if (this.WesternDate is null &&
            this.WesternTime is null &&
            this.NongliLunar == SelectedNongliLunarDateTime.Empty &&
            this.NongliSolar == SelectedNongliSolarDateTime.Empty)
        {
            timeWarnings = "未填入时间。时间乃断卦必须。";
            return;
        }

        StringBuilder builder = new StringBuilder();

        if (this.NongliSolar.Yuezhi is null)
            _ = builder.AppendLine("未填入月支。月将为重中之重。");
        if (this.NongliSolar.Rizhi is null)
            _ = builder.AppendLine("未填入日支。日辰者不可或缺。");
        if (this.NongliSolar.Rigan is null)
            _ = builder.AppendLine("未填入日干。不能够得知六神。");

        var nongliFromWestern = this.GetNongliFromWestern();
        if (nongliFromWestern.HasValue)
        {
            var (lunar, solar) = nongliFromWestern.Value;

            if (!NongliLunar.Meet(lunar))
                _ = builder.AppendLine(
                    $"阴历与西历不符。按西历应为" +
                    $"{lunar.Nian.Dizhi:C}年{lunar.YueInChinese()}月" +
                    $"{lunar.RiInChinese()}日{lunar.Shi:C}时。");
            if (!NongliSolar.Meet(solar))
                _ = builder.AppendLine(
                    $"干支与西历不符。按西历应为" +
                    $"{solar.Nian:C}年{solar.Yue:C}月" +
                    $"{solar.Ri:C}日{solar.Shi:C}时。");
        }

        if (NongliLunar.Nian.HasValue && NongliSolar.Nianzhi.HasValue)
        {
            if (NongliSolar.Nianzhi != NongliLunar.Nian &&
                NongliSolar.Nianzhi != NongliLunar.Nian.Value.Next(1) &&
                NongliSolar.Nianzhi != NongliLunar.Nian.Value.Next(-1))
                _ = builder.AppendLine("干支与阴历年不一致。");
        }

        if (NongliLunar.Yue.HasValue && NongliSolar.Yuezhi.HasValue)
        {
            if (NongliSolar.Yuezhi != (Dizhi)(NongliLunar.Yue + 1) &&
                NongliSolar.Yuezhi != (Dizhi)(NongliLunar.Yue + 2) &&
                NongliSolar.Yuezhi != (Dizhi)(NongliLunar.Yue))
                _ = builder.AppendLine("阴历与干支月不一致。");
        }

        if (NongliLunar.Shi.HasValue && NongliSolar.Shizhi.HasValue)
        {
            if (NongliSolar.Shizhi != NongliLunar.Shi)
                _ = builder.AppendLine("干支与阴历时不一致。");
        }

        if (NongliSolar.Niangan.HasValue && NongliSolar.Nianzhi.HasValue)
        {
            if (NongliSolar.Niangan.Value.Yinyang() != NongliSolar.Nianzhi.Value.Yinyang())
                _ = builder.AppendLine("干支历年干支阴阳不一致。");
        }

        if (NongliSolar.Yuegan.HasValue && NongliSolar.Yuezhi.HasValue)
        {
            if (NongliSolar.Yuegan.Value.Yinyang() != NongliSolar.Yuezhi.Value.Yinyang())
                _ = builder.AppendLine("干支历月干支阴阳不一致。");
        }

        if (NongliSolar.Rigan.HasValue && NongliSolar.Rizhi.HasValue)
        {
            if (NongliSolar.Rigan.Value.Yinyang() != NongliSolar.Rizhi.Value.Yinyang())
                _ = builder.AppendLine("干支历日干支阴阳不一致。");
        }

        if (NongliSolar.Shigan.HasValue && NongliSolar.Shizhi.HasValue)
        {
            if (NongliSolar.Shigan.Value.Yinyang() != NongliSolar.Shizhi.Value.Yinyang())
                _ = builder.AppendLine("干支历时干支阴阳不一致。");
        }

        if (NongliSolar.Niangan.HasValue && NongliSolar.Yuegan.HasValue && NongliSolar.Yuezhi.HasValue)
        {
            var yues = NongliSolar.Niangan.Value.AsNianGetYues();
            var ganzhi = yues[NongliSolar.Yuezhi.Value];
            if (ganzhi.Tiangan != NongliSolar.Yuegan)
                _ = builder.AppendLine($"干支" +
                    $"{NongliSolar.Niangan:C}年无{NongliSolar.Yuegan:C}{NongliSolar.Yuezhi:C}月。" +
                    $"只有{ganzhi:C}月。");
        }

        if (NongliSolar.Rigan.HasValue && NongliSolar.Shigan.HasValue && NongliSolar.Shizhi.HasValue)
        {
            var yues = NongliSolar.Rigan.Value.AsRiGetShis();
            var ganzhi = yues[NongliSolar.Shizhi.Value];
            if (ganzhi.Tiangan != NongliSolar.Shigan)
                _ = builder.AppendLine($"干支" +
                    $"{NongliSolar.Rigan:C}日无{NongliSolar.Shigan:C}{NongliSolar.Shizhi:C}时。" +
                    $"只有{ganzhi:C}时。");
        }

        this.timeWarnings = builder.ToString();
    }
}