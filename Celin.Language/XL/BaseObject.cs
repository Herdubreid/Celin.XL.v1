using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
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
    public void Set(IEnumerable<Action<T>> actions)
    {
        var props = new T();
        foreach (var action in actions) action(props);
        LocalProperties = props;
    }
}
public static class ParserExtensions
{
    public static Parser<char, T> InBraces<T>(this Parser<char, T> parser) =>
        Char('(').Then(parser).Before(Char(')'));
}
public class BaseParser
{
    protected static Parser<char, T> Tok<T>(Parser<char, T> p) =>
        Try(p).Before(SkipWhitespaces);
    protected static Parser<char, char> Tok(char value) => Tok(CIChar(value));
    protected static Parser<char, string> Tok(string value) => Tok(CIString(value));
    protected static Parser<char, string> ALIAS =>
        Map((l, r) => l + (r.HasValue ? r.Value : string.Empty),
        Values<string>.STRING,
        Values<string>.STRING.Before(Char('.')).Optional());
    protected static Parser<char, char> DOT_SEPARATOR =>
        SkipWhitespaces.Then(Char('.'));
    protected static Parser<char, char> COMMA_SEPARATOR =>
        SkipWhitespaces.Then(Char(','));
    protected static Parser<char, string> ADDRESS =>
        AnyCharExcept(')').ManyString().Select(s =>
        {
            var cref = new CellReference(s);
            return cref.Address;
        });
    protected static Parser<char, string> ADDRESS_PARAMETER =>
        ADDRESS.InBraces(); //.Between(Char('('), Char(')'));
    protected static Parser<char, string> STRING_PARAMETER =>
        Values<string>.STRING.InBraces(); //.Between(Char('('), Char(')'));
    protected static Parser<char, int> INT_PARAMETER =>
        Values<int>.NUMBER.Between(Char('('), Char(')'));
    protected static Parser<char, decimal> DECIMAL_PARAMETER =>
        Values<decimal>.NUMBER.Between(Char('('), Char(')'));
    protected static Parser<char, bool> BOOL_PARAMETER =>
        Values<string>.BOOL.Between(Char('('), Char(')'));
}