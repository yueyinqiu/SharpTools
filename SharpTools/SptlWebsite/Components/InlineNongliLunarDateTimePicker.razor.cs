using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using YiJingFramework.Nongli.Lunar;
using YiJingFramework.PrimitiveTypes;

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
    private sealed record NullableNumber(int? Value)
    {
        public override string ToString()
        {
            if (!this.Value.HasValue)
                return "？";
            return this.Value.Value.ToString();
        }
    }

    public sealed record SelectedNongliLunarDateTime(
        Tiangan? Niangan, Dizhi? Nianzhi, 
        int? Yue, bool? IsRunyue, 
        int? Ri,
        Dizhi? Shi)
    {
        public static SelectedNongliLunarDateTime Empty =>
            new(null, null, null, null, null, null);

        public SelectedNongliLunarDateTime(LunarDateTime dateTime)
            : this(dateTime.Nian.Tiangan, dateTime.Nian.Dizhi,
                  dateTime.Yue, dateTime.IsRunyue, 
                  dateTime.Ri,
                  dateTime.Shi)
        {

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
    private readonly ImmutableArray<NullableNumber> possibleYues =
        Enumerable.Range(1, 12)
        .Select(x => new NullableNumber(x))
        .Prepend(new NullableNumber(null))
        .ToImmutableArray();
    private readonly ImmutableArray<NullableNumber> possibleRis =
        Enumerable.Range(1, 30)
        .Select(x => new NullableNumber(x))
        .Prepend(new NullableNumber(null))
        .ToImmutableArray();

    private NullableTiangan Niangan
    {
        get => new(this.Value.Niangan);
        set
        {
            this.Value = this.Value with { Niangan = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableDizhi Nianzhi
    {
        get => new(this.Value.Nianzhi);
        set
        {
            this.Value = this.Value with { Nianzhi = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableNumber Yue
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
    private NullableNumber Ri
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