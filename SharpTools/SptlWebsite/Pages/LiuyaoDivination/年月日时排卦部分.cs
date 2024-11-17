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

    private SelectedNongliSolarDateTime NongliSolar
    {
        get;
        set
        {
            field = value;
            if (!value.Rigan.HasValue)
            {
                for (int i = 0; i < 6; i++)
                    this.各爻六神[i] = null;
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
                    this.各爻六神[i] = (六神)current;
                    current = (current + 1) % 6;
                }
            }
            this.ValidateTime();
            this.复原占断参考();
        }
    } = SelectedNongliSolarDateTime.Empty;

    private readonly 六神?[] 各爻六神 = [null, null, null, null, null, null];

    private SelectedNongliLunarDateTime NongliLunar
    {
        get;
        set
        {
            field = value;
            this.ValidateTime();
            this.复原占断参考();
        }
    } = SelectedNongliLunarDateTime.Empty;
    private DateTime? WesternDate
    {
        get;
        set
        {
            field = value;
            this.ValidateTime();
            this.复原占断参考();
        }
    } = null;

    private DateTime? WesternTime
    {
        get;
        set
        {
            field = value;
            this.ValidateTime();
            this.复原占断参考();
        }
    } = null;
    private (LunarDateTime lunar, SolarDateTime solar)? GetNongliFromWestern()
    {
        if (this.WesternDate.HasValue && this.WesternTime.HasValue)
        {
            var date = this.WesternDate.Value.Date;
            var time = this.WesternTime.Value.TimeOfDay;
            var dateTime = date.Add(time);

            return (LunarDateTime.FromGregorian(dateTime),
                SolarDateTime.FromGregorian(dateTime));
        }
        return null;
    }

    private void FillNongli()
    {
        var value = this.GetNongliFromWestern();
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
            this.timeWarnings = "未填入时间。时间乃断卦必须。";
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

            if (!this.NongliLunar.Meet(lunar))
                _ = builder.AppendLine(
                    $"阴历与西历不符。按西历应为" +
                    $"{lunar.Nian.Dizhi:C}年{lunar.YueInChinese()}月" +
                    $"{lunar.RiInChinese()}日{lunar.Shi:C}时。");
            if (!this.NongliSolar.Meet(solar))
                _ = builder.AppendLine(
                    $"干支与西历不符。按西历应为" +
                    $"{solar.Nian:C}年{solar.Yue:C}月" +
                    $"{solar.Ri:C}日{solar.Shi:C}时。");
        }

        if (this.NongliLunar.Nian.HasValue && this.NongliSolar.Nianzhi.HasValue)
        {
            if (this.NongliSolar.Nianzhi != this.NongliLunar.Nian &&
                this.NongliSolar.Nianzhi != this.NongliLunar.Nian.Value.Next(1) &&
                this.NongliSolar.Nianzhi != this.NongliLunar.Nian.Value.Next(-1))
                _ = builder.AppendLine("干支与阴历年不一致。");
        }

        if (this.NongliLunar.Yue.HasValue && this.NongliSolar.Yuezhi.HasValue)
        {
            if (this.NongliSolar.Yuezhi != (Dizhi)(this.NongliLunar.Yue + 1) &&
                this.NongliSolar.Yuezhi != (Dizhi)(this.NongliLunar.Yue + 2) &&
                this.NongliSolar.Yuezhi != (Dizhi)(this.NongliLunar.Yue))
                _ = builder.AppendLine("阴历与干支月不一致。");
        }

        if (this.NongliLunar.Shi.HasValue && this.NongliSolar.Shizhi.HasValue)
        {
            if (this.NongliSolar.Shizhi != this.NongliLunar.Shi)
                _ = builder.AppendLine("干支与阴历时不一致。");
        }

        if (this.NongliSolar.Niangan.HasValue && this.NongliSolar.Nianzhi.HasValue)
        {
            if (this.NongliSolar.Niangan.Value.Yinyang() != this.NongliSolar.Nianzhi.Value.Yinyang())
                _ = builder.AppendLine("干支历年干支阴阳不一致。");
        }

        if (this.NongliSolar.Yuegan.HasValue && this.NongliSolar.Yuezhi.HasValue)
        {
            if (this.NongliSolar.Yuegan.Value.Yinyang() != this.NongliSolar.Yuezhi.Value.Yinyang())
                _ = builder.AppendLine("干支历月干支阴阳不一致。");
        }

        if (this.NongliSolar.Rigan.HasValue && this.NongliSolar.Rizhi.HasValue)
        {
            if (this.NongliSolar.Rigan.Value.Yinyang() != this.NongliSolar.Rizhi.Value.Yinyang())
                _ = builder.AppendLine("干支历日干支阴阳不一致。");
        }

        if (this.NongliSolar.Shigan.HasValue && this.NongliSolar.Shizhi.HasValue)
        {
            if (this.NongliSolar.Shigan.Value.Yinyang() != this.NongliSolar.Shizhi.Value.Yinyang())
                _ = builder.AppendLine("干支历时干支阴阳不一致。");
        }

        if (this.NongliSolar.Niangan.HasValue && this.NongliSolar.Yuegan.HasValue && this.NongliSolar.Yuezhi.HasValue)
        {
            var yues = this.NongliSolar.Niangan.Value.AsNianGetYues();
            var ganzhi = yues[this.NongliSolar.Yuezhi.Value];
            if (ganzhi.Tiangan != this.NongliSolar.Yuegan)
                _ = builder.AppendLine($"干支" +
                    $"{this.NongliSolar.Niangan:C}年无{this.NongliSolar.Yuegan:C}{this.NongliSolar.Yuezhi:C}月。" +
                    $"只有{ganzhi:C}月。");
        }

        if (this.NongliSolar.Rigan.HasValue && this.NongliSolar.Shigan.HasValue && this.NongliSolar.Shizhi.HasValue)
        {
            var yues = this.NongliSolar.Rigan.Value.AsRiGetShis();
            var ganzhi = yues[this.NongliSolar.Shizhi.Value];
            if (ganzhi.Tiangan != this.NongliSolar.Shigan)
                _ = builder.AppendLine($"干支" +
                    $"{this.NongliSolar.Rigan:C}日无{this.NongliSolar.Shigan:C}{this.NongliSolar.Shizhi:C}时。" +
                    $"只有{ganzhi:C}时。");
        }

        this.timeWarnings = builder.ToString();
    }
}