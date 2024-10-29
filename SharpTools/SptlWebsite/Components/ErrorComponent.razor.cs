using Microsoft.AspNetCore.Components;

namespace SptlWebsite.Components;

public partial class ErrorComponent
{
    [Parameter]
    public Exception? Exception { get; set; }
}