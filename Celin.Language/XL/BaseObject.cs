using Pidgin;
using static Pidgin.Parser;
using System.Text.Json.Serialization;

namespace Celin.Language.XL;

[JsonDerivedType(typeof(WorkbookProperties), nameof(WorkbookProperties))]
[JsonDerivedType(typeof(WorksheetProperties), nameof(WorksheetProperties))]
[JsonDerivedType(typeof(RangeProperties), nameof(RangeProperties))]
[JsonDerivedType(typeof(FormatProperties), nameof(FormatProperties))]
[JsonDerivedType(typeof(FillProperties), nameof(FillProperties))]
[JsonDerivedType(typeof(FontProperties), nameof(FontProperties))]
[JsonDerivedType(typeof(TableProperties), nameof(TableProperties))]
[JsonDerivedType(typeof(TableColumnProperties), nameof(TableColumnProperties))]
[JsonDerivedType(typeof(TableRowProperties), nameof(TableRowProperties))]
public record BaseProperties(
    string? Id = null)
{
    public BaseProperties() : this(Id: null) { }
}
public delegate ValueTask<T> SyncAsyncDelegate<T>(string? key, T values, params object?[] pars);
public abstract class BaseObject { }
public abstract class BaseObject<T> : BaseObject
    where T : new()
{
    public abstract string? Key { get; }
    public virtual object?[] Params => [];
    public abstract T Properties { get; protected set; }
    public abstract T LocalProperties { get; set; }
    public static SyncAsyncDelegate<T> SyncAsyncDelegate { get; set; } = null!;
    public async Task Sync()
    {
        Properties = await SyncAsyncDelegate(Key, LocalProperties, Params);
        LocalProperties = new();
    }
}
public class BaseParser
{
    protected static Parser<char, string> STRING_PARAMETER =>
    Values<string>.STRING.Between(Char('('), Char(')'));
    protected static Parser<char, int> INT_PARAMETER =>
    Values<int>.NUMBER.Between(Char('('), Char(')'));
    protected static Parser<char, decimal> DECIMAL_PARAMETER =>
    Values<decimal>.NUMBER.Between(Char('('), Char(')'));
    protected static Parser<char, bool> BOOL_PARAMETER =>
        Values<string>.BOOL.Between(Char('('), Char(')'));
}