using Microsoft.AspNetCore.Components;
using OneHexagramPerDayCore;
using SptlWebsite.Extensions;
using System.Collections.Immutable;
using YiJingFramework.Nongli.Extensions;
using YiJingFramework.Nongli.Lunar;
using YiJingFramework.PrimitiveTypes;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount.Extensions;

namespace SptlWebsite.Pages.OneHexagramPerDay;

public partial class OneHexagramPerDayPage
{
    private ImmutableArray<ProcessedGua> guas = [];
    private ProcessedGua? displayingGua;
    private (GuaHexagram gua, string display) todaysGua =
        (new(Enumerable.Repeat(Yinyang.Yang, 6)), "");

    [Parameter]
    [SupplyParameterFromQuery]
    public string? DefaultGua { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        {
            var zhouyiRaw = await this.BuiltInZhouyi.GetZhouyiAsync();
            var zhouyi = new ZhouyiStoreWithLineTitles(zhouyiRaw);
            var guasBuilder = ImmutableArray.CreateBuilder<ProcessedGua>();
            for (int i = 0b000000; i <= 0b111111; i++)
            {
                var gua = Gua.Parse(Convert.ToString(i, 2).PadLeft(6, '0'));
                var zhouyiGua = new ProcessedGua(gua.AsFixed<GuaHexagram>(), zhouyi);
                guasBuilder.Add(zhouyiGua);
            }
            this.guas = guasBuilder.MoveToImmutable();
        }

        {
            var date = DateOnly.FromDateTime(DateTime.Now);
            var nongli = LunarDateTime.FromGregorian(date.ToDateTime(new TimeOnly(6, 30)));
            var todaysGuaString =
                $"一日一卦于 " +
                $"{nongli.Nian:C}年{nongli.YueInChinese()}月{nongli.RiInChinese()} " +
                $"{date:yyyy/MM/dd}";
            var todaysGua = new HexagramProvider(date).GetHexagram();
            this.todaysGua = (todaysGua, todaysGuaString);
        }

        {
            if (GuaHexagram.TryParse(this.DefaultGua, out var defaultGua))
            {
                await this.HistoryBlazor.ReplaceStateWithCurrentStateAsync(
                    new UriBuilder(this.Navigation.Uri)
                    .SetQuery()
                    .ToString());
            }
            else
            {
                defaultGua = this.todaysGua.gua;
            }

            displayingGua = this.guas.Single(x => x.Gua.Painting == defaultGua);
        }
    }
}