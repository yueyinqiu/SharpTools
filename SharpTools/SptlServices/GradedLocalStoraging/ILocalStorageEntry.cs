namespace SptlServices.GradedLocalStoraging;

public interface ILocalStorageEntry<T>
{
    string FullKey { get; }

    void Set(T data);
    bool TryGet(out T? value);
}