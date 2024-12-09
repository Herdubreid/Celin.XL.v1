using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record TableProperties(
    string? Id = null,
    string? Name = null,
    bool? HighlightFirstColumn = null,
    bool? HighlightLastColumn = null,
    bool? ShowBandedColumns = null,
    bool? ShowBandedRows = null,
    bool? ShowFilterButton = null,
    bool? ShowHeaders = null,
    bool? ShowTotals = null,
    string? Style = null) : BaseProperties(Id)
{
    public enum Methods { add, get, delete }
    public TableProperties() : this(Name: null) { }
};
public class TableObject : BaseObject<TableProperties>
{
    public TableColumnCollectionObject Columns =>
        new TableColumnCollectionObject(Key ?? throw new NullReferenceException(nameof(Columns)));
    public TableRowCollectionObject Rows =>
        new TableRowCollectionObject(Key ?? throw new NullReferenceException(nameof(Rows)));
    protected KeyValuePair<TableProperties.Methods, object?[]>? _method = null;
    public void Method(TableProperties.Methods method, params object?[] pars) =>
        _method = new(method, pars);
    public override object?[] Params => _method == null
        ? base.Params
        : new object?[] { _method.Value.Key.ToString() }.Concat(_method.Value.Value).ToArray();
    public override string? Key => _xl.Id ?? _local.Name ?? string.Empty;
    public override TableProperties Properties
    {
        get => _xl;
        protected set => _xl = value;
    }
    public override TableProperties LocalProperties
    {
        get => _local;
        set => _local = value;
    }
    protected TableProperties _local;
    protected TableProperties _xl;
    public TableObject(string? tableName)
    {
        _xl = new TableProperties();
        _local = new TableProperties(Name: tableName);
    }
    public static TableObject Table(string? name = null)
        => new TableObject(name);
}
public class TableParser : BaseParser
{
    static Parser<char, Action<TableObject>> Name =>
        Base.Tok(nameof(Name)).Then(STRING_PARAMETER)
        .Select<Action<TableObject>>(s => table => table.LocalProperties = table.LocalProperties with { Name = s });
    static Parser<char, Action<TableObject>> HighlightFirstColumn =>
        Base.Tok(nameof(HighlightFirstColumn)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties = table.LocalProperties with { HighlightFirstColumn = b });
    static Parser<char, Action<TableObject>> HighlightLastColumn =>
        Base.Tok(nameof(HighlightLastColumn)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties = table.LocalProperties with { HighlightLastColumn = b });
    static Parser<char, Action<TableObject>> ShowBandedColumns =>
        Base.Tok(nameof(ShowBandedColumns)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties = table.LocalProperties with { ShowBandedColumns = b });
    static Parser<char, Action<TableObject>> ShowBandedRows =>
        Base.Tok(nameof(ShowBandedRows)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties = table.LocalProperties with { ShowBandedRows = b });
    static Parser<char, Action<TableObject>> ShowFilterButton =>
        Base.Tok(nameof(ShowFilterButton)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties = table.LocalProperties with { ShowFilterButton = b });
    static Parser<char, Action<TableObject>> ShowHeaders =>
        Base.Tok(nameof(ShowHeaders)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties = table.LocalProperties with { ShowHeaders = b });
    static Parser<char, Action<TableObject>> ShowTotals =>
        Base.Tok(nameof(ShowTotals)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties = table.LocalProperties with { ShowTotals = b });
    static Parser<char, Action<TableObject>> Style =>
        Base.Tok(nameof(Style)).Then(STRING_PARAMETER)
        .Select<Action<TableObject>>(s => table => table.LocalProperties = table.LocalProperties with { Style = s });
    static Parser<char, TableObject> TABLE =>
        Base.Tok("table").Then(STRING_PARAMETER.Optional())
        .Select(t => new TableObject(t.HasValue ? t.Value : null));
    public static Parser<char, BaseObject> Parser =>
        Map((t, ps) =>
        {
            foreach (var p in ps) p(t);
            return t as BaseObject;
        },
        Skipper.Next(TABLE).Before(Char('.')),
        OneOf(
            Name,
            HighlightFirstColumn,
            HighlightLastColumn,
            ShowBandedColumns,
            ShowBandedRows,
            ShowFilterButton,
            ShowHeaders,
            ShowTotals,
            Style).Separated(Char('.')));
}
