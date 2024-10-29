using Microsoft.AspNetCore.Components;

namespace SptlWebsite.Pages;

public partial class NotFoundPage
{
    [SupplyParameterFromQuery]
    private string? OriginalUri { get; set; } = null;
}