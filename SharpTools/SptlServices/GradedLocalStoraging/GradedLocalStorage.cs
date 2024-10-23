using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization.Metadata;

namespace SptlServices.GradedLocalStoraging;

internal sealed partial class GradedLocalStorage(
    ILogger<GradedLocalStorage> logger,
    ISyncLocalStorageService localStorage,
    string rootKey) : IGradedLocalStorage
{
    public ILocalStorageEntry<T> GetEntry<T>(
        string subKey, int importance, JsonTypeInfo<T> serializer)
    {
        return new LocalStorageEntry<T>(
            localStorage, logger, rootKey, subKey, importance, serializer);
    }
}
