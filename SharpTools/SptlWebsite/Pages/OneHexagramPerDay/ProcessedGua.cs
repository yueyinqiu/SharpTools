using OneHexagramPerDayCore;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.EntityRelations.GuaCharacters.Extensions;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;

namespace SptlWebsite.Pages.OneHexagramPerDay;

public partial class OneHexagramPerDayPage
{
    private sealed class ProcessedGua
    {
        public ProcessedGua(GuaHexagram gua, ZhouyiStoreWithLineTitles zhouyi) 
        {
            this.Gua = zhouyi[gua];
            var (upper, lower) = this.Gua.SplitToTrigrams(zhouyi.InnerStore);

            this.PaintingOption = this.Gua.Painting.ToString();

            this.NameOption = this.Gua.Name?.ToString() ?? this.PaintingOption;
            if (string.IsNullOrEmpty(this.NameOption))
                this.UpperLowerOption = this.PaintingOption;

            this.UpperLowerOption = $"{upper.Name}上{lower.Name}下";
            if(string.IsNullOrEmpty(this.UpperLowerOption))
                this.UpperLowerOption = this.PaintingOption;

            if (upper.Painting == lower.Painting)
            {
                this.DisplayedTitle = 
                    $"{this.Gua.Painting.ToUnicodeChar()} {this.Gua.Name}为{upper.Nature}";
            }
            else
            {
                this.DisplayedTitle = 
                    $"{this.Gua.Painting.ToUnicodeChar()} {upper.Nature}{lower.Nature}{this.Gua.Name}";
            }
        }

        public string PaintingOption { get; }
        public string NameOption { get; }
        public string UpperLowerOption { get; }
        public ZhouyiHexagram Gua { get; }
        public string DisplayedTitle { get; }
    }
}