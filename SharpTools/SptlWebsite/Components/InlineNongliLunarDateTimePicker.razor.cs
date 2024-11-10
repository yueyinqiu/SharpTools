using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using YiJingFramework.Nongli.Lunar;
using YiJingFramework.Nongli.Solar;
using YiJingFramework.PrimitiveTypes;
using static SptlWebsite.Components.InlineNongliSolarDateTimePicker;

namespace SptlWebsite.Components;

public partial class InlineNongliLunarDateTimePicker
{
    private sealed record NullableTiangan(Tiangan? Value)
    {
        public override string ToString()
        {
            if (!this.Value.HasValue)
                return "？";
            return this.Value.Value.ToString("C");
        }
    }
    private sealed record NullableDizhi(Dizhi? Value)
    {
        public override string ToString()
        {
            if (!this.Value.HasValue)
                return "？";
            return this.Value.Value.ToString("C");
        }
    }
    private sealed record PingRun(bool? IsRun)
    {
        public override string ToString()
        {
            if (!this.IsRun.HasValue)
                return "？";
            return this.IsRun.Value ? "闰" : "平";
        }
    }
    private sealed record NullableYueNumber(int? Value)
    {
        public override string ToString()
        {
            return this.Value switch
            {
                1 => "正",
                2 => "二",
                3 => "三",
                4 => "四",
                5 => "五",
                6 => "六",
                7 => "七",
                8 => "八",
                9 => "九",
                10 => "十",
                11 => "冬",
                12 => "腊",
                _ => "？"
            };
        }
    }
    private sealed record NullableRiNumber(int? Value)
    {
        public override string ToString()
        {
            return this.Value switch
            {
                1 => "初一",
                2 => "初二",
                3 => "初三",
                4 => "初四",
                5 => "初五",
                6 => "初六",
                7 => "初七",
                8 => "初八",
                9 => "初九",
                10 => "初十",
                11 => "十一",
                12 => "十二",
                13 => "十三",
                14 => "十四",
                15 => "十五",
                16 => "十六",
                17 => "十七",
                18 => "十八",
                19 => "十九",
                20 => "二十",
                21 => "廿一",
                22 => "廿二",
                23 => "廿三",
                24 => "廿四",
                25 => "廿五",
                26 => "廿六",
                27 => "廿七",
                28 => "廿八",
                29 => "廿九",
                30 => "三十",
                _ => "？？"
            };
        }
    }

    public sealed record SelectedNongliLunarDateTime(
        Dizhi? Nian, 
        int? Yue, bool? IsRunyue, 
        int? Ri,
        Dizhi? Shi)
    {
        public static SelectedNongliLunarDateTime Empty =>
            new(null, null, null, null, null);

        public SelectedNongliLunarDateTime(LunarDateTime dateTime)
            : this(dateTime.Nian.Dizhi,
                  dateTime.Yue, dateTime.IsRunyue, 
                  dateTime.Ri,
                  dateTime.Shi)
        {

        }

        public bool Meet(SelectedNongliLunarDateTime other)
        {
            if (this.Nian is not null && other.Nian is not null && this.Nian != other.Nian)
                return false;
            if (this.Yue is not null && other.Yue is not null && this.Yue != other.Yue)
                return false;
            if (this.IsRunyue is not null && other.IsRunyue is not null && this.IsRunyue != other.IsRunyue)
                return false;
            if (this.Ri is not null && other.Ri is not null && this.Ri != other.Ri)
                return false;
            if (this.Shi is not null && other.Shi is not null && this.Shi != other.Shi)
                return false;
            return true;
        }

        public bool Meet(LunarDateTime other)
        {
            return this.Meet(new SelectedNongliLunarDateTime(other));
        }
    }

    private readonly ImmutableArray<NullableTiangan> possibleTiangans =
        Enumerable.Range(1, 10)
        .Select(Tiangan.FromIndex)
        .Select(x => new NullableTiangan(x))
        .Prepend(new NullableTiangan(null))
        .ToImmutableArray();
    private readonly ImmutableArray<NullableDizhi> possibleDizhis =
        Enumerable.Range(1, 12)
        .Select(Dizhi.FromIndex)
        .Select(x => new NullableDizhi(x))
        .Prepend(new NullableDizhi(null))
        .ToImmutableArray();
    private readonly ImmutableArray<PingRun> possiblePingRuns = [new(null), new(false), new(true)];
    private readonly ImmutableArray<NullableYueNumber> possibleYues =
        Enumerable.Range(1, 12)
        .Select(x => new NullableYueNumber(x))
        .Prepend(new NullableYueNumber(null))
        .ToImmutableArray();
    private readonly ImmutableArray<NullableRiNumber> possibleRis =
        Enumerable.Range(1, 30)
        .Select(x => new NullableRiNumber(x))
        .Prepend(new NullableRiNumber(null))
        .ToImmutableArray();

    private NullableDizhi Nian
    {
        get => new(this.Value.Nian);
        set
        {
            this.Value = this.Value with { Nian = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableYueNumber Yue
    {
        get => new(this.Value.Yue);
        set
        {
            this.Value = this.Value with { Yue = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private PingRun IsRunyue
    {
        get => new(this.Value.IsRunyue);
        set
        {
            this.Value = this.Value with { IsRunyue = value.IsRun };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableRiNumber Ri
    {
        get => new(this.Value.Ri);
        set
        {
            this.Value = this.Value with { Ri = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableDizhi Shi
    {
        get => new(this.Value.Shi);
        set
        {
            this.Value = this.Value with { Shi = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }

    [Parameter]
    public SelectedNongliLunarDateTime Value { get; set; } = SelectedNongliLunarDateTime.Empty;
    [Parameter]
    public EventCallback<SelectedNongliLunarDateTime> ValueChanged { get; set; }
}