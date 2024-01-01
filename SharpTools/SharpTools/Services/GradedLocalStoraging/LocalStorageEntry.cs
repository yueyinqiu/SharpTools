namespace SharpTools.Services.GradedLocalStoraging;

public sealed class LocalStorageEntry<T>(
    GradedLocalStorage root, string subKey, int importance)
    where T : class
{
    public void Set(T data)
    {
        root.SetValue(subKey, data, importance);
    }

    public T? Get()
    {
        return root.GetValue<T>(subKey);
    }
}