using YiJingFramework.Annotating.Zhouyi;

namespace SptlWebsite.Pages.LiuyaoDivination;

public partial class LiuyaoDivinationPage
{
    private ZhouyiStore zhouyi = new ZhouyiStore(null);
    protected override async Task OnParametersSetAsync()
    {
        this.zhouyi = await this.BuiltInZhouyi.GetZhouyiAsync();
    }
}