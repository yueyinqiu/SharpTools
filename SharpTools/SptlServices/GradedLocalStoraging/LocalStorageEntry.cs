using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SptlServices.GradedLocalStoraging;

internal sealed class LocalStorageEntry<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(
    ISyncLocalStorageService localStorage,
    ILogger<GradedLocalStorage> logger,
    string rootKey,
    string subKey,
    int importance) : ILocalStorageEntry<T>
{
    public string FullKey { get; } = $"{rootKey}.{subKey}";

    private const string separator = "::SEPARATOR::";

    private int RemoveUnimportant()
    {
        var result = 0;
        try
        {
            foreach (var key in localStorage.Keys())
            {
                if (!key.StartsWith(rootKey))
                    continue;

                var dataString = localStorage.GetItemAsString(key);
                if (dataString is null)
                    continue;
                var split = dataString.Split(separator, 2);
                if (!int.TryParse(split[0], out var dataImportance))
                    dataImportance = -1;
                if (dataImportance < importance)
                {
                    localStorage.RemoveItem(key);
                    result++;
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Cannot access the local storage.");
            return result;
        }
    }

    private static readonly JsonSerializerOptions serializer = new JsonSerializerOptions()
    {
        RespectNullableAnnotations = true,
        RespectRequiredConstructorParameters = true
    };

    public void Set(T data)
    {
        var serialized = JsonSerializer.Serialize(data, serializer);
        var dataString = $"{importance}{separator}{serialized}";
        try
        {
            localStorage.SetItemAsString(this.FullKey, dataString);
        }
        catch
        {
            _ = this.RemoveUnimportant();

            localStorage.SetItemAsString(this.FullKey, dataString);
        }
    }

    public bool TryGet(out T? data)
    {
        var dataString = localStorage.GetItemAsString(this.FullKey);
        if (dataString is null)
        {
            data = default;
            return false;
        }

        var split = dataString.Split(separator, 2);
        if (split.Length is not 2)
        {
            data = default;
            return false;
        }
        if (!int.TryParse(split[0], out _))
        {
            data = default;
            return false;
        }

        var serialized = split[1];
        try
        {
            data = JsonSerializer.Deserialize<T>(serialized, serializer);
            return true;
        }
        catch (JsonException)
        {
            data = default;
            return false;
        }
    }
}