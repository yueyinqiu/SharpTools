namespace SptlWebsite.Extensions;

internal static class UriBuilderExtensions
{
    public static UriBuilder SetQuery(this UriBuilder uriBuilder, string query = "")
    {
        uriBuilder.Query = query;
        return uriBuilder;
    }
}
