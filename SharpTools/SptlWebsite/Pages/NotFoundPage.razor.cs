using Microsoft.AspNetCore.Components;
using SptlWebsite.Extensions;

namespace SptlWebsite.Pages;

public partial class NotFoundPage
{
    [SupplyParameterFromQuery]
    private string? OriginalUri { get; set; } = null;

    protected override async Task OnParametersSetAsync()
    {
        await this.HistoryBlazor.ReplaceStateWithCurrentStateAsync(
            new UriBuilder(this.Navigation.Uri)
            .SetQuery()
            .ToString());
    }
}