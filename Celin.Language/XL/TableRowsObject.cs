namespace Celin.Language.XL;

public record TableRowProperties(
    int? Index = null,
    List<object?>? Values = null)
{
    public enum Methods { add }
    public TableRowProperties() : this(Index: null) { }
}
public class TableRowsObject(string _name) : BaseObject<List<TableRowProperties>>
{
    protected KeyValuePair<TableRowProperties.Methods, object?[]>? _method = null;
    public void Method(TableRowProperties.Methods method, params object?[] pars) =>
        _method = new(method, pars);
    public override object?[] Params => _method == null
        ? base.Params
        : new object?[] { _method.Value.Key.ToString() }.Concat(_method.Value.Value).ToArray();
    public override string? Key => _name;
    public override List<TableRowProperties> Properties { get => _xl; protected set => _xl = value; }
    public override List<TableRowProperties> LocalProperties { get => _local; set => _local = value; }
    protected List<TableRowProperties> _local = new List<TableRowProperties>();
    protected List<TableRowProperties> _xl = new List<TableRowProperties>();
    public static TableRowsObject TableRowCollection(string name) => new(name);
}
