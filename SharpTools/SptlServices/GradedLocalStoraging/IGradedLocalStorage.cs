namespace SptlServices.GradedLocalStoraging;

public interface IGradedLocalStorage
{
    ILocalStorageEntry<T> GetEntry<T>(string subKey, Importance importance);
}