namespace Celin.Language.XL;

public record NamedItemProperties(string? Id = null) : BaseProperties(Id)
{
    public string? Name { get; set; }
    public string? Comment { get; set; }
    public object? Formula { get; set; }
    public string? Scope { get; set; }
    public string? Type { get; set; }
    public object? Value { get; set; }
    public bool? Visible { get; set; }
    public NamedItemProperties() : this(Id: null) { }
}
public class NamedItemObject : BaseObject<NamedItemProperties>
{
    public override string Key => _xl.Name ?? string.Empty;
    public override NamedItemProperties Properties
    {
        get => _xl;
        protected set => _xl = value;
    }
    public override NamedItemProperties LocalProperties
    {
        get => _local;
        set => _local = value;
    }
    NamedItemProperties _local;
    NamedItemProperties _xl;
    public NamedItemObject(string? name)
    {
        _xl = new NamedItemProperties();
        _local = new NamedItemProperties { Name = name };
    }
    public static NamedItemObject NamedItem(string? name = null)
        => new(name);
}
