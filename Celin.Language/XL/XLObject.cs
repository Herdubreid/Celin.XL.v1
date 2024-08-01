namespace Celin.Language.XL;

public delegate ValueTask<T> ExcelSyncFrom<T>(string key);
public delegate ValueTask<T> ExcelSyncTo<T>(string key, T values);
public abstract class XLObject<T>
{
    public abstract string Key { get; }
    public abstract T Properties { get; set; }
    public abstract T LocalProperties { get; }
    public static ExcelSyncFrom<T> SyncFromDelegate { get; set; } = null!;
    public static ExcelSyncTo<T> SyncToDelegate { get; set; } = null!;
    public async Task Sync(bool fromExcel = true)
    {
        if (fromExcel)
            Properties = await SyncFromDelegate(Key);
        else
            Properties = await SyncToDelegate(Key, LocalProperties);
    }
}
