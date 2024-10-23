using Microsoft.AspNetCore.Components;

namespace SptlWebsite.Pages;

partial class NotFoundPage
{
    [SupplyParameterFromQuery]
    private string? OriginalUri { get; set; } = null;
}