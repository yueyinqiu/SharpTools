using hyjiacan.py4n;
using System.Collections.Immutable;

namespace SptlWebsite.Pages.PinyinConverter;

public partial class PinyinConverterPage
{
    private PinyinFormat caseFormat = PinyinFormat.LOWERCASE;
    private readonly ImmutableArray<PinyinFormat> CaseFormats = [
        PinyinFormat.LOWERCASE,
        PinyinFormat.UPPERCASE,
        PinyinFormat.CAPITALIZE_FIRST_LETTER,
    ];

    private PinyinFormat vFormat = PinyinFormat.WITH_V;
    private readonly ImmutableArray<PinyinFormat> VFormats = [
        PinyinFormat.WITH_V,
        PinyinFormat.WITH_U_UNICODE,
        PinyinFormat.WITH_YU,
        PinyinFormat.WITH_U_AND_COLON
    ];

    private PinyinFormat toneFormat = PinyinFormat.WITH_TONE_NUMBER;
    private readonly ImmutableArray<PinyinFormat> ToneFormats = [
        PinyinFormat.WITH_TONE_NUMBER,
        PinyinFormat.WITH_TONE_MARK,
        PinyinFormat.WITHOUT_TONE,
    ];

    private string GetOptionText(PinyinFormat format)
    {
        return format switch
        {
            PinyinFormat.LOWERCASE => "全部小写",
            PinyinFormat.UPPERCASE => "全部大写",
            PinyinFormat.CAPITALIZE_FIRST_LETTER => "首字母大写",

            PinyinFormat.WITH_U_UNICODE => "ü",
            PinyinFormat.WITH_V => "v",
            PinyinFormat.WITH_YU => "yu",
            PinyinFormat.WITH_U_AND_COLON => "u:",

            PinyinFormat.WITH_TONE_MARK => "带声调",
            PinyinFormat.WITH_TONE_NUMBER => "数字声调",
            PinyinFormat.WITHOUT_TONE => "不带声调",

            _ => ""
        };
    }

    private string input = "";

    private sealed class OutputItem
    {
        public ImmutableArray<string> AvailablePinyins { get; }
        public string SelectedPinyin { get; set; }
        public OutputItem(PinyinItem item)
        {
            this.AvailablePinyins = [.. item, item.RawChar.ToString()];
            this.SelectedPinyin = this.AvailablePinyins[0];
        }
        public OutputItem(IEnumerable<string> item)
        {
            this.AvailablePinyins = [.. item];
            this.SelectedPinyin = this.AvailablePinyins[0];
        }
    }
    private void Convert()
    {
        if (toneFormat == PinyinFormat.WITH_TONE_MARK && caseFormat != PinyinFormat.WITH_U_UNICODE)
        {
            output = [new(["无法在 v 、 yu 或 u: 上标记声调，可以改用 ü 或者数字声调"])];
            return;
        }

        output = Pinyin4Net.GetPinyinArray(input, caseFormat | vFormat | toneFormat)
            .Select(x => new OutputItem(x))
            .ToImmutableArray();
    }

    private ImmutableArray<OutputItem> output = [new(["点击转换开始转换"])];

    private async Task CopyAsync()
    {
        var items = output.Select(x => x.SelectedPinyin);
        var result = string.Join(" ", items);
        await ClipboardService.CopyTextToClipboardAsync(result);
    }
}