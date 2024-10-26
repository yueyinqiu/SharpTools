using YiJingFramework.Annotating.Zhouyi;

namespace SptlServices.BuiltInZhouyi;
public interface IBuiltInZhouyiAccessor
{
    Task<ZhouyiStore> GetZhouyiAsync();
}