using Pidgin;

namespace Celin.Language.XL;

public record TableHeaderProperties(List<string?>? Items)
{
    public TableHeaderProperties() : this(Items: null) { }
}
public class TableHeaderObject : BaseObject<TableHeaderProperties>
{
    public string? Id { set; get; }
    public override string? Key => Id;
    public override TableHeaderProperties Properties { get => _xl; protected set => _xl = value; }
    public override TableHeaderProperties LocalProperties { get => _local; set => _local = value; }
    protected TableHeaderProperties _local = new TableHeaderProperties();
    protected TableHeaderProperties _xl = new TableHeaderProperties();
}
public class TableHeaderParser : BaseParser
{
    public static Parser<char, TableHeaderObject> Header =>
        Tok(nameof(Header)).Then(STRING_ARRAY_PARAMETER)
        .Select(a => new TableHeaderObject
        { LocalProperties = new TableHeaderProperties(a) });
}
