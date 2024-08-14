namespace Celin.Language.XL;

public record SheetProperties(
    string? Id = null,
    string? Name = null,
    int? Position = null,
    bool? EnableCalculation = null,
    bool? ShowGridlines = null,
    bool? ShowHeadings = null,
    decimal? StandardHeight = null,
    decimal? StandardWidth = null,
    string? TabColor = null,
    int? TabId = null,
    string? Visibility = null)
{
    public SheetProperties() : this(Id: null) { }
};
public class SheetObject : BaseObject<SheetProperties>
{
    public override string Key => _xl.Id ?? _local.Name ?? string.Empty;
    public override SheetProperties Properties
    {
        get => _xl;
        protected set => _xl = value;
    }
    public override SheetProperties LocalProperties
    {
        get => _local;
        protected set => _local = value;
    }
    public string? Name
    {
        get => Properties.Name;
        set => _local = _local with { Name = value };
    }
    SheetProperties _local;
    SheetProperties _xl;
    SheetObject(string? sheetName)
    {
        _xl = new SheetProperties();
        _local = new SheetProperties(Name: sheetName);
    }
    public static SheetObject Sheet(string? name = null)
        => new SheetObject(name);
}
