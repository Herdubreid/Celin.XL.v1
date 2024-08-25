namespace Celin.Language.XL;

public record FontProperties(
    bool? Bold = null,
    string? Color = null,
    bool? Italic = null,
    string? Name = null,
    int? Size = null,
    bool? Strikethrough = null,
    bool? Subscript = null,
    bool? Superscript = null,
    double? TintAndShade = null,
    string? Underline = null)
{
    public FontProperties() : this(Bold: null) { }
}
public class FontObject : RangeBaseObject<FontProperties, FontObject>
{
    public bool? Bold
    {
        get => _xl.Bold;
        set => _local = _local with { Bold = value };
    }
    public string? Color
    {
        get => _xl.Color;
        set => _local = _local with { Color = value };
    }
    public bool? Italic
    {
        get => _xl.Italic;
        set => _local = _local with { Italic = value };
    }
    public string? Name
    {
        get => _xl.Name;
        set => _local = _local with { Name = value };
    }
    public int? Size
    {
        get => _xl.Size;
        set => _local = _local with { Size = value };
    }
    public bool? Strikethrough
    {
        get => _xl.Strikethrough;
        set => _local = _local with { Strikethrough = value };
    }
    public bool? Subscript
    {
        get => _xl.Subscript;
        set => _local = _local with { Subscript = value };
    }
    public bool? Superscript
    {
        get => _xl.Superscript;
        set => _local = _local with { Superscript = value };
    }
    public double? TintAndShade
    {
        get => _xl.TintAndShade;
        set => _local = _local with { TintAndShade = value };
    }
    public string? Underline
    {
        get => _xl.Underline;
        set => _local = _local with { Underline = value };
    }
    public FontObject(string? address) : base(address) { }
    public static FontObject Font(string? address) => new(address);
}
