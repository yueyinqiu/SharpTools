using hyjiacan.py4n;
using Microsoft.FluentUI.AspNetCore.Components;
using System.Collections.Immutable;

namespace SptlWebsite.Pages.PinyinConverter;

public partial class PinyinConverterPage
{
    private PinyinFormat caseFormatDontTouchMe = PinyinFormat.LOWERCASE;
    private PinyinFormat CaseFormat
    {
        get
        {
            return caseFormatDontTouchMe;
        }
        set
        {
            this.caseFormatDontTouchMe = value;
            this.SavePreference();
        }
    }
    private readonly ImmutableArray<PinyinFormat> CaseFormats = [
        PinyinFormat.LOWERCASE,
        PinyinFormat.UPPERCASE,
        PinyinFormat.CAPITALIZE_FIRST_LETTER,
    ];

    private PinyinFormat vFormatDontTouchMe = PinyinFormat.WITH_V;
    private PinyinFormat VFormat
    {
        get
        {
            return vFormatDontTouchMe;
        }
        set
        {
            this.vFormatDontTouchMe = value;
            this.SavePreference();
        }
    }
    private readonly ImmutableArray<PinyinFormat> VFormats = [
        PinyinFormat.WITH_V,
        PinyinFormat.WITH_U_UNICODE,
        PinyinFormat.WITH_YU,
        PinyinFormat.WITH_U_AND_COLON
    ];

    private PinyinFormat toneFormatDontTouchMe = PinyinFormat.WITH_TONE_NUMBER;
    private PinyinFormat ToneFormat
    {
        get
        {
            return toneFormatDontTouchMe;
        }
        set
        {
            this.toneFormatDontTouchMe = value;
            this.SavePreference();
        }
    }
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

    private string input = "你好！";

    private sealed class OutputItem
    {
        public ImmutableArray<string> AvailablePinyins { get; }
        public string SelectedPinyin { get; set; }
        public OutputItem(PinyinItem item)
        {
            this.AvailablePinyins = [.. item, item.RawChar.ToString()];
            this.SelectedPinyin = this.AvailablePinyins[0];
        }
    }
    private ImmutableArray<OutputItem> Output
    {
        get
        {
            return Pinyin4Net.GetPinyinArray(input, CaseFormat | VFormat | ToneFormat)
                .Select(x => new OutputItem(x))
                .ToImmutableArray();
        }
    }

    private void SavePreference()
    {
    }
}