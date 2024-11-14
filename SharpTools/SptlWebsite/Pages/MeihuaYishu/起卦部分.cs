using Microsoft.JSInterop;
using System.Diagnostics;
using System.Text;
using YiJingFramework.EntityRelations.EntityCharacteristics.Extensions;
using YiJingFramework.EntityRelations.TianganNianYuesAndRishis.Extensions;
using YiJingFramework.Nongli.Extensions;
using YiJingFramework.Nongli.Lunar;
using YiJingFramework.Nongli.Solar;
using YiJingFramework.PrimitiveTypes;
using static SptlWebsite.Components.InlineNongliLunarDateTimePicker;
using static SptlWebsite.Components.InlineNongliSolarDateTimePicker;

namespace SptlWebsite.Pages.MeihuaYishu;

public partial class MeihuaYishuPage
{
    private string upperInput = "年+月+日";
    private string lowerInput = "年+月+日+时";
    private string changingInput = "年+月+日+时";

    private const string defaultScript =
        """
        function calculate()
        {
            let 年 = nongliLunar.nian.index;
            let 月 = nongliLunar.yue;
            let 日 = nongliLunar.ri;
            let 时 = nongliLunar.shi.index;

            try
            {
                outputs.shanggua = eval(shangguaInput);
            }
            catch (ex)
            {
                outputs.error = `上卦计算失败。请检查输入的时间是否完整，以及上卦表达式是否正确。详细信息：${ex}`
                return;
            }
            let 上 = outputs.shanggua;

            try
            {
                outputs.shanggua = eval(shangguaInput);
            }
            catch (ex)
            {
                outputs.error = `下卦计算失败。请检查输入的时间是否完整，以及下卦表达式是否正确。详细信息：${ex}`
                return;
            }
            let 下 = eval(xiaguaInput)

            try
            {
                outputs.dongyao = eval(dongyaoInput);
            }
            catch (ex)
            {
                outputs.error = `动爻计算失败。请检查输入的时间是否完整，以及动爻表达式是否正确。详细信息：${ex}`
                return;
            }

            if (!Number.isInteger(outputs.shanggua) ||
                !Number.isInteger(outputs.xiagua) ||
                !Number.isInteger(outputs.dongyao))
            {
                outputs.error = `计算得到的卦数并非整数。其中上卦为 ${outputs.shanggua} ，下卦为 ${outputs.xiagua} ，动爻为 ${outputs.dongyao} 。`;
                return;
            }

            outputs.error = null;
        }
        calculate()
        """;
    private string script = defaultScript;

    private void ResetScript()
    {
        script = defaultScript;
    }

    private string output = "";

    private sealed class RawTools
    {

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


    public void GetGuas()
    {
        DateTime? western;
        if (WesternDate.HasValue && WesternTime.HasValue)
        {
            var date = WesternDate.Value.Date;
            var time = WesternTime.Value.TimeOfDay;
            western = date.Add(time);
        }
        else
        {
            western = null;
        }

        var rawInputs = new RawInputs(
            new(this.NongliLunar), new(this.NongliSolar), western, 
            script,
            upperInput, lowerInput, changingInput);
        using var rawTools = DotNetObjectReference.Create(new RawTools());

        var result = jsModule.Invoke<RawOutputs>("calculate", [
            rawInputs, new RawTools()
        ]);
        output = result.ToString();
    }
}