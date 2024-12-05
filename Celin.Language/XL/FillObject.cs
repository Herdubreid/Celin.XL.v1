namespace Celin.Language.XL;

public record FillProperties(
    string? Color = null,
    string? Pattern = null,
    string? PatternColor = null,
    double? PatternTintAndShade = null) : BaseProperties
{
    public FillProperties() : this(Color: null) { }
};
public class FillObject : RangeBaseObject<FillProperties, FillObject>
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
    public static async Task Set(string? key, FillProperties prop, object?[] pars) =>
        await SyncAsyncDelegate(key, prop, pars);
    public FillObject(string? address) : base(address) { }
    public static FillObject Fill(string? address) => new(address);
}
