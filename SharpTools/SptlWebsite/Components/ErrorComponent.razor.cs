using Microsoft.AspNetCore.Components;

namespace SptlWebsite.Components;

partial class ErrorComponent
{
    [Parameter]
    public Exception? Exception { get; set; }
}