using System.Collections.Immutable;
using System.Text;
using YiJingFramework.Annotating.Zhouyi;
using YiJingFramework.Annotating.Zhouyi.Entities;
using YiJingFramework.EntityRelations.EntityCharacteristics.Extensions;
using YiJingFramework.EntityRelations.EntityStrings;
using YiJingFramework.EntityRelations.EntityStrings.Conversions;
using YiJingFramework.EntityRelations.EntityStrings.Extensions;
using YiJingFramework.EntityRelations.GuaCharacters.Extensions;
using YiJingFramework.EntityRelations.GuaHexagramBagongs.Extensions;
using YiJingFramework.EntityRelations.GuaHexagramNajias.Extensions;
using YiJingFramework.EntityRelations.WuxingRelations;
using YiJingFramework.EntityRelations.WuxingRelations.Extensions;
using YiJingFramework.PrimitiveTypes;
using YiJingFramework.PrimitiveTypes.GuaWithFixedCount;
using static SptlWebsite.Components.InlineNongliSolarDateTimePicker;

namespace SptlWebsite.Pages.LiuyaoDivination;

public partial class LiuyaoDivinationPage
{
    private ZhouyiStore zhouyi = new ZhouyiStore(null);
    protected override async Task OnParametersSetAsync()
    {
        this.zhouyi = await this.BuiltInZhouyi.GetZhouyiAsync();
    }
}