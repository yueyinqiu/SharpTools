using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using YiJingFramework.Nongli.Solar;
using YiJingFramework.PrimitiveTypes;

namespace SptlWebsite.Components;

public partial class InlineNongliSolarDateTimePicker
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

    public sealed record SelectedNongliSolarDateTime(
        Tiangan? Niangan, Dizhi? Nianzhi, Tiangan? Yuegan, Dizhi? Yuezhi,
        Tiangan? Rigan, Dizhi? Rizhi, Tiangan? Shigan, Dizhi? Shizhi)
    {
        public static SelectedNongliSolarDateTime Empty =>
            new(null, null, null, null, null, null, null, null);

        public SelectedNongliSolarDateTime(SolarDateTime dateTime)
            : this(dateTime.Nian.Tiangan, dateTime.Nian.Dizhi,
                  dateTime.Yue.Tiangan, dateTime.Yue.Dizhi,
                  dateTime.Ri.Tiangan, dateTime.Ri.Dizhi,
                  dateTime.Shi.Tiangan, dateTime.Shi.Dizhi)
        {

        }

        public bool Meet(SelectedNongliSolarDateTime other)
        {
            if (this.Niangan is not null && other.Niangan is not null && this.Niangan != other.Niangan)
                return false;
            if (this.Nianzhi is not null && other.Nianzhi is not null && this.Nianzhi != other.Nianzhi)
                return false;
            if (this.Yuegan is not null && other.Yuegan is not null && this.Yuegan != other.Yuegan)
                return false;
            if (this.Yuezhi is not null && other.Yuezhi is not null && this.Yuezhi != other.Yuezhi)
                return false;
            if (this.Rigan is not null && other.Rigan is not null && this.Rigan != other.Rigan)
                return false;
            if (this.Rizhi is not null && other.Rizhi is not null && this.Rizhi != other.Rizhi)
                return false;
            if (this.Shigan is not null && other.Shigan is not null && this.Shigan != other.Shigan)
                return false;
            if (this.Shizhi is not null && other.Shizhi is not null && this.Shizhi != other.Shizhi)
                return false;
            return true;
        }

        public bool Meet(SolarDateTime other)
        {
            return this.Meet(new SelectedNongliSolarDateTime(other));
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
    private NullableTiangan Yuegan
    {
        get => new(this.Value.Yuegan);
        set
        {
            this.Value = this.Value with { Yuegan = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableDizhi Yuezhi
    {
        get => new(this.Value.Yuezhi);
        set
        {
            this.Value = this.Value with { Yuezhi = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableTiangan Rigan
    {
        get => new(this.Value.Rigan);
        set
        {
            this.Value = this.Value with { Rigan = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableDizhi Rizhi
    {
        get => new(this.Value.Rizhi);
        set
        {
            this.Value = this.Value with { Rizhi = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableTiangan Shigan
    {
        get => new(this.Value.Shigan);
        set
        {
            this.Value = this.Value with { Shigan = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }
    private NullableDizhi Shizhi
    {
        get => new(this.Value.Shizhi);
        set
        {
            this.Value = this.Value with { Shizhi = value.Value };
            _ = this.ValueChanged.InvokeAsync(this.Value);
        }
    }

    [Parameter]
    public SelectedNongliSolarDateTime Value { get; set; } = SelectedNongliSolarDateTime.Empty;
    [Parameter]
    public EventCallback<SelectedNongliSolarDateTime> ValueChanged { get; set; }
}