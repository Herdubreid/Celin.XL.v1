namespace Celin.Language.XL;
public record FormatProperties(
    bool? AutoIndent = null,
    decimal? ColumnWidth = null,
    string? HorizontalAlignment = null,
    int? IndentLevel = null,
    string? ReadingOrder = null,
    decimal? RowHeight = null,
    bool? ShrinkToFit = null,
    int? TextOrientation = null,
    bool? UseStandardHeight = null,
    bool? UseStandardWidth = null,
    string? VerticalAlignment = null,
    bool? WrapText = null)
{
    public FormatProperties() : this(AutoIndent: null) { }
};
public class FormatObject : RangeBaseObject<FormatProperties, FormatObject>
{
    public FillObject Fill => FillObject.Fill(_address);
    public FontObject Font => FontObject.Font(_address);
    public bool? AutoIndent
    {
        get => _xl.AutoIndent;
        set => _local = _local with { AutoIndent = value };
    }
    public decimal? ColumnWidth
    {
        get => _xl.ColumnWidth;
        set => _local = _local with { ColumnWidth = value };
    }
    public string? HorizontalAlignment
    {
        get => _xl.HorizontalAlignment;
        set => _local = _local with { HorizontalAlignment = value };
    }
    public int? IndentLevel
    {
        get => _xl.IndentLevel;
        set => _local = _local with { IndentLevel = value };
    }
    public string? ReadingOrder
    {
        get => _xl.ReadingOrder;
        set => _local = _local with { ReadingOrder = value };
    }
    public decimal? RowHeight
    {
        get => _xl.RowHeight;
        set => _local = _local with { RowHeight = value };
    }
    public bool? ShrinkToFit
    {
        get => _xl.ShrinkToFit;
        set => _local = _local with { ShrinkToFit = value };
    }
    public int? TextOrientation
    {
        get => _xl.TextOrientation;
        set => _local = _local with { TextOrientation = value };
    }
    public bool? UseStandardHeight
    {
        get => _xl.UseStandardHeight;
        set => _local = _local with { UseStandardHeight = value };
    }
    public bool? UseStandardWidth
    {
        get => _xl.UseStandardWidth;
        set => _local = _local with { UseStandardWidth = value };
    }
    public string? VerticalAlignment
    {
        get => _xl.VerticalAlignment;
        set => _local = _local with { VerticalAlignment = value };
    }
    public bool? WrapText
    {
        get => _xl.WrapText;
        set => _local = _local with { WrapText = value };
    }
    protected FormatObject(string? address) : base(address) { }
    public static FormatObject Format(string? address)
        => new(address);
}
