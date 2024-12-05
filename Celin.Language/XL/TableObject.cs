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
    public TableProperties() : this(Id: null) { }
};
public class TableObject : BaseObject<TableProperties>
{
    public TableColumnCollectionObject Columns =>
        new TableColumnCollectionObject(Key ?? throw new NullReferenceException(nameof(Columns)));
    public override string? Key => _xl.Id ?? _local.Name ?? string.Empty;
    public override string[] Params => [_method?.ToString() ?? string.Empty];
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
    protected enum Methods { add, get, delete }
    protected Methods? _method;
    protected static readonly Parser<char, string> TABLES =
        Try(Skipper.Next(Base.Tok(".tables")));
    protected static readonly Parser<char, Methods> GET =
        Try(Skipper.Next(Base.Tok($".{Methods.get}")))
            .ThenReturn(Methods.get);
    protected static readonly Parser<char, Methods> ADD =
        Try(Skipper.Next(Base.Tok($".{Methods.add}")))
            .ThenReturn(Methods.add);
    protected static readonly Parser<char, Methods> DELETE =
        Try(Skipper.Next(Base.Tok($".{Methods.delete}")))
            .ThenReturn(Methods.delete);
    protected static readonly Parser<char, string> NAME =
        Values<string>.STRING.Between(Char('('), Char(')'));
    public static Parser<char, TableObject> Parser =>
        Map((a, n) => new TableObject(n) { _method = a },
        XL.Then(TABLES.Then(ADD.Or(GET))),
        NAME);
    #endregion
}
