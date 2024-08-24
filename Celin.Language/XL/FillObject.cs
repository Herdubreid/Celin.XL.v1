namespace Celin.Language.XL;

public record FillProperties(
    string? Color = null,
    string? Pattern = null,
    string? PatternColor = null,
    double? PatternTintAndShade = null,
    double? TintAndShade = null)
{
    public FillProperties() :  this(Color: null) { }
};
public class FillObject : BaseObject<FillProperties>
{
    public string? Color
    {
        get => _xl.Color;
        set => _local = _local with { Color = value };
    }
    public string? Pattern
    {
        get => _xl.Pattern;
        set => _local = _local with { Pattern = value };
    }
    public string? PatternColor
    {
        get => _xl.PatternColor;
        set => _local = _local with { PatternColor = value };
    }
    public double? PatternTintAndShade
    {
        get => _xl.PatternTintAndShade;
        set => _local = _local with { PatternTintAndShade = value };
    }
    public double? TintAndShade
    {
        get => _xl.TintAndShade;
        set => _local = _local with { TintAndShade = value };
    }
    public override string Key => _address ?? string.Empty;
    public override FillProperties Properties
    { 
        get => _xl;
        protected set => _xl = value;
    }
    public override FillProperties LocalProperties
    {
        get => _local;
        protected set => _local = value;
    }
    string? _address;
    FillProperties _local;
    FillProperties _xl;
    protected FillObject(string? address)
    {
        if (address != null)
            _ = RangeObject.Dim(address);
        _address = address;
        _local = new FillProperties();
        _xl = new FillProperties();
    }
    public static FillObject Fill(string? address)
        => new(address);
}
