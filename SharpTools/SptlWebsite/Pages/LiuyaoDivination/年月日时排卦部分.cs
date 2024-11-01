using YiJingFramework.EntityRelations.EntityCharacteristics.Extensions;
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
        }
    }
    private readonly 六神?[] 各爻六神 = [null, null, null, null, null, null];

    private SelectedNongliLunarDateTime nongliLunar = SelectedNongliLunarDateTime.Empty;
    private DateTime? westernDate = null;
    private DateTime? westernTime = null;
    private void FillNongli()
    {
        if (westernDate.HasValue && westernTime.HasValue)
        {
            var date = westernDate.Value.Date;
            var time = westernDate.Value.TimeOfDay;
            var dateTime = date.Add(time);

            var lunar = LunarDateTime.FromGregorian(dateTime);
            this.nongliLunar = new SelectedNongliLunarDateTime(lunar);

            var solar = SolarDateTime.FromGregorian(dateTime);
            this.NongliSolar = new SelectedNongliSolarDateTime(solar);
        }
    }
    private void FillCurrent()
    {
        var now = DateTime.Now;
        this.westernDate = now;
        this.westernTime = now;
        this.FillNongli();
    }
    private void ClearDateTime()
    {
        this.westernDate = null;
        this.westernTime = null;
        this.NongliSolar = SelectedNongliSolarDateTime.Empty;
        this.nongliLunar = SelectedNongliLunarDateTime.Empty;
    }
}