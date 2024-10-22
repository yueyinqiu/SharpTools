using Microsoft.AspNetCore.Components;

namespace SptlWebsite.Pages;

partial class ErrorPage
{
    [Parameter]
    public Exception? Exception { get; set; }
}