using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record TableProperties(string? Id = null) : BaseProperties(Id)
{
    public string? Name { get; set; }
    public bool? HighlightFirstColumn { get; set; }
    public bool? HighlightLastColumn { get; set; }
    public bool? ShowBandedColumns { get; set; }
    public bool? ShowBandedRows { get; set; }
    public bool? ShowFilterButton { get; set; }
    public bool? ShowHeaders { get; set; }
    public bool? ShowTotals { get; set; }
    public string? Style { get; set; }
    public enum Methods { getTable, addTable }
    public TableProperties() : this(Id: null) { }
};
public class TableObject : BaseObject<TableProperties>
{
    public CqlObject? Cql { get; set; }
    public TableColumnsObject Columns =>
        new TableColumnsObject(Key ?? throw new NullReferenceException(nameof(Columns)));
    public TableRowsObject Rows =>
        new TableRowsObject(Key ?? throw new NullReferenceException(nameof(Rows)));
    protected KeyValuePair<TableProperties.Methods, object?[]>? _method = null;
    public void Method(TableProperties.Methods method, params object?[] pars) =>
        _method = new(method, pars);
    public override object?[] Params => _method == null
        ? base.Params
        : new object?[] { _method.Value.Key.ToString() }.Concat(_method.Value.Value).ToArray();
    public override string? Key => _xl.Id ?? _xl.Name ?? _local.Name ?? string.Empty;
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
    protected TableProperties _local = new TableProperties();
    protected TableProperties _xl = new TableProperties();
}
public class TableParser : BaseParser
{
    static Parser<char, Action<TableObject>> Name =>
        Tok(nameof(Name)).Then(STRING_PARAMETER)
        .Select<Action<TableObject>>(s => table => table.LocalProperties.Name = s);
    static Parser<char, Action<TableObject>> HighlightFirstColumn =>
        Tok(nameof(HighlightFirstColumn)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties.HighlightFirstColumn = b);
    static Parser<char, Action<TableObject>> HighlightLastColumn =>
        Tok(nameof(HighlightLastColumn)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties.HighlightLastColumn = b);
    static Parser<char, Action<TableObject>> ShowBandedColumns =>
        Tok(nameof(ShowBandedColumns)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties.ShowBandedColumns = b);
    static Parser<char, Action<TableObject>> ShowBandedRows =>
        Tok(nameof(ShowBandedRows)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties.ShowBandedRows = b);
    static Parser<char, Action<TableObject>> ShowFilterButton =>
        Tok(nameof(ShowFilterButton)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties.ShowFilterButton = b);
    static Parser<char, Action<TableObject>> ShowHeaders =>
        Tok(nameof(ShowHeaders)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties.ShowHeaders = b);
    static Parser<char, Action<TableObject>> ShowTotals =>
        Tok(nameof(ShowTotals)).Then(BOOL_PARAMETER)
        .Select<Action<TableObject>>(b => table => table.LocalProperties.ShowTotals = b);
    static Parser<char, Action<TableObject>> Style =>
        Tok(nameof(Style)).Then(STRING_PARAMETER)
        .Select<Action<TableObject>>(s => table => table.LocalProperties.Style = s);
    static Parser<char, Action<TableObject>> Cql =>
        CqlParser.Query
        .Select<Action<TableObject>>(cql => table => table.Cql = cql);
    static Parser<char, TableObject> TABLE =>
        Tok("table")
        .Then(ADDRESS_PARAMETER.Optional())
        .Select(s =>
        {
            var table = new TableObject();
            table.Method(TableProperties.Methods.addTable, s.HasValue ? s.Value : null);
            return table;
        });
    static Parser<char, TableObject> TABLES =>
        Tok("tables")
        .Then(STRING_PARAMETER)
        .Select(s =>
        {
            var table = new TableObject();
            table.Method(TableProperties.Methods.getTable, s);
            return table;
        });
    public static Parser<char, IEnumerable<Action<TableObject>>> Actions =>
        OneOf(
            Name,
            HighlightFirstColumn,
            HighlightLastColumn,
            ShowBandedColumns,
            ShowBandedRows,
            ShowFilterButton,
            ShowHeaders,
            ShowTotals,
            Style,
            Cql)
        .Separated(DOT_SEPARATOR);
    public static Parser<char, BaseObject> Object =>
        OneOf(
            TABLES.Actions(Actions),
            TABLE.Actions(Actions))
        .Cast<BaseObject>();
}
