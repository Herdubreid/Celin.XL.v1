namespace Celin.Language.XL;

public record SheetProperties(
    string? id = null,
    string? name = null,
    int? position = null,
    bool? enableCalculation = null,
    bool? showGridlines = null,
    bool? showHeadings = null,
    decimal? standardHeight = null,
    decimal? standardWidth = null,
    string? tabColor = null,
    int? tabId = null,
    string? visibility = null);
public class SheetObject : XLObject<SheetProperties>
{
    public override string Key => _xl.id ?? _local.name ?? string.Empty;
    public override SheetProperties Properties
    {
        get => _xl;
        set => _xl = value;
    }
    public override SheetProperties LocalProperties => _local;
    public string? Name
    {
        get => Properties.name;
        set => _local = _local with { name = value };
    }
    SheetProperties _local;
    SheetProperties _xl;
    SheetObject(string sheetName)
    {
        _xl = new SheetProperties();
        _local = new SheetProperties(name: sheetName);
    }
    public static SheetObject Sheet(string name)
        => new SheetObject(name);
}
