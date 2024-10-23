using SptlServices.GradedLocalStoraging;
using System.Text.Json.Serialization;

namespace SptlWebsite.Layout;

partial class MainLayout
{
    [JsonSourceGenerationOptions(WriteIndented = false)]
    [JsonSerializable(typeof(int))]
    internal partial class SerializerContext : JsonSerializerContext
    {
    }

    private ILocalStorageEntry<int> NavMenuSizeStorageEntry => 
        this.LocalStorage.GetEntry(
            "MainLayout.NavMenuSize", 10,
            SerializerContext.Default.Int32);

    private string navMenuSize = "250px";
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if(this.NavMenuSizeStorageEntry.TryGet(out var navMenuSize))
        {
            this.navMenuSize = $"{navMenuSize}px";
            this.StateHasChanged();
        }
    }

    private void SaveNavMenuSize(int navMenuSize)
    {
        this.NavMenuSizeStorageEntry.Set(navMenuSize);
    }
}