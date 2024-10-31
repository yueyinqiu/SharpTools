using Microsoft.AspNetCore.Components;
using OneHexagramPerDayCore;
using SptlWebsite.Extensions;
using YiJingFramework.Nongli.Extensions;
using YiJingFramework.Nongli.Lunar;
using YiJingFramework.PrimitiveTypes;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount.Extensions;

namespace SptlWebsite.Pages.OneHexagramPerDay;

public partial class OneHexagramPerDayPage
{
    private Dictionary<string, ProcessedGua> paintingsToGuas = [];
    private Dictionary<string, ProcessedGua> namesToGuas = [];
    private Dictionary<string, ProcessedGua> upperLowerToGuas = [];

    private ProcessedGua? displayingGua;
    private (GuaHexagram gua, string display) todaysGua = 
        (new(Enumerable.Repeat(Yinyang.Yang, 6)), "");

    private void SetDontTouchAs(ProcessedGua gua)
    {
        this.inputPaintingDontTouchMe = gua.PaintingOption;
        this.inputNameDontTouchMe = gua.NameOption;
        this.inputUpperLowerDontTouchMe = gua.UpperLowerOption;
    }

    private string? inputPaintingDontTouchMe;
    private string? InputPainting
    {
        get
        {
            return inputPaintingDontTouchMe;
        }
        set
        {
            if (paintingsToGuas.TryGetValue(value!, out displayingGua))
            {
                SetDontTouchAs(displayingGua);
            }
            else
            {
                inputPaintingDontTouchMe = value;
                this.displayingGua = null;
                this.InputName = null;
                this.InputUpperLower = null;
            }
        }
    }
    private string? inputNameDontTouchMe;
    private string? InputName
    {
        get
        {
            return inputNameDontTouchMe;
        }
        set
        {
            if (namesToGuas.TryGetValue(value!, out displayingGua))
            {
                SetDontTouchAs(displayingGua);
            }
            else
            {
                inputNameDontTouchMe = value;
                this.displayingGua = null;
                this.InputPainting = null;
                this.InputUpperLower = null;
            }
        }
    }
    private string? inputUpperLowerDontTouchMe;
    private string? InputUpperLower
    {
        get
        {
            return inputUpperLowerDontTouchMe;
        }
        set
        {
            if (upperLowerToGuas.TryGetValue(value!, out displayingGua))
            {
                SetDontTouchAs(displayingGua);
            }
            else
            {
                inputUpperLowerDontTouchMe = value;
                this.displayingGua = null;
                this.InputPainting = null;
                this.InputName = null;
            }
        }
    }

    [Parameter]
    [SupplyParameterFromQuery]
    public string? DefaultGua { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        var zhouyiRaw = await this.BuiltInZhouyi.GetZhouyiAsync();
        var zhouyi = new ZhouyiStoreWithLineTitles(zhouyiRaw);

        this.paintingsToGuas = [];
        this.namesToGuas = [];
        this.upperLowerToGuas = [];

        for (int i = 0b000000; i <= 0b111111; i++)
        {
            var gua = Gua.Parse(Convert.ToString(i, 2).PadLeft(6, '0'));
            var zhouyiGua = new ProcessedGua(gua.AsFixed<GuaHexagram>(), zhouyi);

            paintingsToGuas.Add(zhouyiGua.PaintingOption, zhouyiGua);
            namesToGuas.Add(zhouyiGua.NameOption, zhouyiGua);
            upperLowerToGuas.Add(zhouyiGua.UpperLowerOption, zhouyiGua);
        }

        var date = DateOnly.FromDateTime(DateTime.Now);
        var nongli = LunarDateTime.FromGregorian(date.ToDateTime(new TimeOnly(6, 30)));
        var todaysGuaString =
            $"一日一卦于 " +
            $"{nongli.Nian:C}年{nongli.YueInChinese()}月{nongli.RiInChinese()} " +
            $"{date:yyyy/MM/dd}";
        var todaysGua = new HexagramProvider(date).GetHexagram();
        this.todaysGua = (todaysGua, todaysGuaString);

        if (this.DefaultGua is null)
            this.InputPainting = this.todaysGua.gua.ToString();
        else
        {
            this.InputPainting = this.DefaultGua;
            await this.HistoryBlazor.ReplaceStateWithCurrentStateAsync(
                new UriBuilder(this.Navigation.Uri)
                .SetQuery()
                .ToString());
        }
    }
}