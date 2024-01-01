namespace SharpTools.Services.GradedLocalStoraging;

public partial class GradedLocalStorage
{
    private sealed record GradedData<T>(T Value, int Importance);
}