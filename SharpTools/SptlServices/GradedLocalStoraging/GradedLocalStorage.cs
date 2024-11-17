using Blazored.LocalStorage;
using Microsoft.Extensions.Logging;

namespace SptlServices.GradedLocalStoraging;

internal sealed partial class GradedLocalStorage(
    ILogger<GradedLocalStorage> logger,
    ISyncLocalStorageService localStorage,
    string rootKey) : IGradedLocalStorage
{
    public ILocalStorageEntry<T> GetEntry<T>(string subKey, Importance importance)
    {
        return new LocalStorageEntry<T>(localStorage, logger, rootKey, subKey, (int)importance);
    }
}
