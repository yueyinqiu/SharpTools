using Blazored.LocalStorage;

namespace SharpTools.Services.GradedLocalStoraging;

public sealed partial class GradedLocalStorage(ISyncLocalStorageService localStorage)
{
    public string RootKey => "sharptools";

    public string GetFullKey(string subKey)
    {
        return $"{this.RootKey}.{subKey}";
    }

    public LocalStorageEntry<T> GetEntry<T>(string subKey, int importance) where T : class
    {
        return new LocalStorageEntry<T>(this, subKey, importance);
    }

    public int RemoveUnimportant(int keptImportance)
    {
        var result = 0;
        try
        {
            foreach (var key in localStorage.Keys())
            {
                if (!key.StartsWith(this.RootKey))
                    continue;

                try
                {
                    var item = localStorage.GetItem<GradedData<object>>(key);
                    if (item.Importance < keptImportance)
                    {
                        localStorage.RemoveItem(key);
                        result++;
                    }
                }
                catch
                {
                    localStorage.RemoveItem(key);
                    result++;
                }
            }
            return result;
        }
        catch
        {
            return result;
        }
    }

    public bool SetValue<T>(
        string subKey, T value, int importance, bool keepUnimportant = false)
        where T : class
    {
        try
        {
            localStorage.SetItem(this.GetFullKey(subKey), new GradedData<T>(value, importance));
            return true;
        }
        catch
        {
            if (keepUnimportant)
                return false;
            _ = this.RemoveUnimportant(importance);
            return this.SetValue(subKey, value, importance, true);
        }
    }

    public T? GetValue<T>(string subKey) where T : class
    {
        try
        {
            return localStorage.GetItem<GradedData<T>>(this.GetFullKey(subKey)).Value;
        }
        catch
        {
            return null;
        }
    }
}
