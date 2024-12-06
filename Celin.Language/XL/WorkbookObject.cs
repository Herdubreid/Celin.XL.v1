namespace Celin.Language.XL;

public record WorkbookProperties(
    string? Name = null)
{
    public enum Methods { getActiveCell }
    public WorkbookProperties() : this(Name: null) { }
}
public class WorkbookObject : BaseObject<WorkbookProperties>
{
    protected KeyValuePair<WorkbookProperties.Methods, object?[]>? _method = null;
    public void Method(WorkbookProperties.Methods method, params object?[] pars) =>
        _method = new(method, pars);
    public override object?[] Params => _method == null
        ? base.Params
        : new object?[] { _method.Value.Key.ToString() }.Concat(_method.Value.Value).ToArray();
    public override string? Key => null;
    public override WorkbookProperties Properties { get => _xl; protected set => _xl = value; }
    public override WorkbookProperties LocalProperties { get => _local; set => _local = value; }
    protected WorkbookProperties _local = new WorkbookProperties();
    protected WorkbookProperties _xl = new WorkbookProperties();
}
