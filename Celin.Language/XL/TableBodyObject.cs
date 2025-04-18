using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record TableBodyProperties(
    IEnumerable<IEnumerable<object?>>? Values,
    IEnumerable<string?>? NumberFormat = null)
{
    public TableBodyProperties() : this(Values: null) { }
}
public class TableBodyObject : BaseObject<TableBodyProperties>
{
    public string? Id { set; get; }
    public override string? Key => Id;
    public override TableBodyProperties Properties { get => _xl; protected set => _xl = value; }
    public override TableBodyProperties LocalProperties { get => _local; set => _local = value; }
    protected TableBodyProperties _local = new TableBodyProperties();
    protected TableBodyProperties _xl = new TableBodyProperties();
}
public class TableBodyParser : BaseParser
{
    static Parser<char, List<string?>> Format =>
        Tok(nameof(Format)).Then(STRING_ARRAY_PARAMETER);
    static Parser<char, List<List<object?>>> Body =>
        Tok(nameof(Body)).Then(OBJECT_MATRIX_PARAMETER);
    public static Parser<char, TableBodyObject> Parser =>
        Map((fmt, body) =>
        new TableBodyObject
        { LocalProperties = new TableBodyProperties(body, fmt.HasValue ? fmt.Value : null) },
        Format.Before(DOT_SEPARATOR).Optional(),
        Body
        );
}
