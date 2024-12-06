using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record TableProperties(
    bool? HighlightFirstColumn = null,
    bool? HighlightLastColumn = null,
    string? Id = null,
    string? LegacyId = null,
    string? Name = null,
    bool? ShowBandedColumns = null,
    bool? ShowBandedRows = null,
    bool? ShowFilterButton = null,
    bool? ShowHeaders = null,
    bool? ShowTotals = null,
    string? Style = null) : BaseProperties
{
    public enum Methods { add, get, delete }
    public TableProperties() : this(Id: null) { }
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
    TableProperties _local;
    TableProperties _xl;
    public TableObject(string? tableName)
    {
        _xl = new TableProperties();
        _local = new TableProperties(Name: tableName);
    }
    public static TableObject Table(string? name = null)
        => new TableObject(name);
    #region parser
    protected static readonly Parser<char, string> TABLES =
        Try(Skipper.Next(Base.Tok(".tables")));
    protected static readonly Parser<char, TableProperties.Methods> GET =
        Try(Skipper.Next(Base.Tok($".{TableProperties.Methods.get}")))
            .ThenReturn(TableProperties.Methods.get);
    protected static readonly Parser<char, TableProperties.Methods> ADD =
        Try(Skipper.Next(Base.Tok($".{TableProperties.Methods.add}")))
            .ThenReturn(TableProperties.Methods.add);
    protected static readonly Parser<char, TableProperties.Methods> DELETE =
        Try(Skipper.Next(Base.Tok($".{TableProperties.Methods.delete}")))
            .ThenReturn(TableProperties.Methods.delete);
    protected static readonly Parser<char, string> NAME =
        Values<string>.STRING.Between(Char('('), Char(')'));
    public static Parser<char, TableObject> Parser =>
        Map((a, n) => new TableObject(n),
        XL.Then(TABLES.Then(ADD.Or(GET))),
        NAME);
    #endregion
}
