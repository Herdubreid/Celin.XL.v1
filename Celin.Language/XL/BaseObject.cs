namespace Celin.Language.XL;

public delegate ValueTask<T> GetAsyncDelegate<T>(string key);
public delegate ValueTask<T> SetAsyncDelegate<T>(string key, T values);
public abstract class BaseObject<T>
{
    public abstract string Key { get; }
    public abstract T Properties { get; set; }
    public abstract T LocalProperties { get; }
    public static GetAsyncDelegate<T> GetAsyncDelegate { get; set; } = null!;
    public static SetAsyncDelegate<T> SetAsyncDelegate { get; set; } = null!;
    public async Task GetAsync()
    {
        Properties = await GetAsyncDelegate(Key);
    }
    public async Task SetAsync()
    {
        Properties = await SetAsyncDelegate(Key, LocalProperties);
    }
}
