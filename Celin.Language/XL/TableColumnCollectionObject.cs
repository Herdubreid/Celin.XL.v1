namespace Celin.Language.XL;

public record TableColumnProperties(
    int? Id = null,
    int? Index = null,
    string? Name = null,
    List<List<object?>>? Values = null)
{
    public enum Methods { add, getCount, getItem }
    public TableColumnProperties() : this(Id: null) { }
}
public class TableColumnCollectionObject(string _name) : BaseObject<List<TableColumnProperties>>
{
    protected KeyValuePair<TableColumnProperties.Methods, object?[]>? _method = null;
    public void Method(TableColumnProperties.Methods method, params object?[] pars) =>
        _method = new(method, pars);
    public override object?[] Params => _method == null
        ? base.Params
        : [_method.Value.Key.ToString(), _method.Value.Value]; 
    public override string? Key => _name;
    public override List<TableColumnProperties> Properties { get => _xl; protected set => _xl = value; }
    public override List<TableColumnProperties> LocalProperties { get => _local; set => _local = value; }
    protected List<TableColumnProperties> _local = new List<TableColumnProperties>();
    protected List<TableColumnProperties> _xl = new List<TableColumnProperties>();
    public static TableColumnCollectionObject TableColumnCollection(string name) => new(name);
}
