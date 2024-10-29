using YiJingFramework.Annotating.Zhouyi;

namespace SptlServices.BuiltInZhouyi;

internal sealed class BuiltInZhouyiAccessor(string baseAddress) : IBuiltInZhouyiAccessor
{
    public async Task<ZhouyiStore> GetZhouyiAsync()
    {
        using var client = new HttpClient() { BaseAddress = new(baseAddress) };
        var json = await client.GetStringAsync($"_content/SptlServices/zhouyi/975345ca.json");
        var result = ZhouyiStore.DeserializeFromJsonString(json);
        if (result is null)
            return new ZhouyiStore(null);
        return result;
    }
}
