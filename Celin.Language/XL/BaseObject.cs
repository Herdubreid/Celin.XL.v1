namespace Celin.Language.XL;

public delegate ValueTask<T> SyncAsyncDelegate<T>(string? key, T values, params string[] pars);
public abstract class BaseObject<T>
    where T : new()
{
    public abstract string? Key { get; }
    public virtual string[] Params => [];
    public abstract T Properties { get; protected set; }
    public abstract T LocalProperties { get; protected set; }
    public static SyncAsyncDelegate<T> SyncAsyncDelegate { get; set; } = null!;
    public async Task Sync()
    {
        Properties = await SyncAsyncDelegate(Key, LocalProperties, Params);
        LocalProperties = new();
    }
}
