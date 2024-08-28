namespace Celin.Language.XL;

public record BorderProperties(
    string? Color = null,
    string? SideIndex = null,
    string? Style = null,
    string? Weight = null) : BaseProperties
{
    public BorderProperties() : this(Color: null) { }
}
public class BorderSetting
{
    public string? Color
    {
        get => _getXl?.Color;
        set
        {
            int ndx = _getLocal;
            if (ndx < 0)
            {
                _local.Add(new BorderProperties(SideIndex: _sideIndex, Color: value));
            }
            else
            {
                var e = _local[ndx] with { Color = value };
                _local[ndx] = e;
            }
        }
    }
    public string? Weight
    {
        get => _getXl?.Weight;
        set
        {
            int ndx = _getLocal;
            if (ndx < 0)
            {
                _local.Add(new BorderProperties(SideIndex: _sideIndex, Weight: value));
            }
            else
            {
                var e = _local[ndx] with { Weight = value };
                _local[ndx] = e;
            }
        }
    }
    public string? Style
    {
        get => _getXl?.Style;
        set
        {
            int ndx = _getLocal;
            if (ndx < 0)
            {
                _local.Add(new BorderProperties(SideIndex: _sideIndex, Style: value));
            }
            else
            {
                var e = _local[ndx] with { Style = value };
                _local[ndx] = e;
            }
        }
    }
    BorderProperties? _getXl => _xl.Find(x => x.SideIndex == _sideIndex);
    int _getLocal => _local.FindIndex(x => x.SideIndex == _sideIndex);
    string? _sideIndex;
    List<BorderProperties> _xl;
    List<BorderProperties> _local;
    public BorderSetting(string sideIndex, List<BorderProperties> xl, List<BorderProperties> local)
    {
        _sideIndex = sideIndex;
        _xl = xl;
        _local = local;
    }
}
public class BordersObject : RangeBaseObject<List<BorderProperties>, BordersObject>
{
    public BorderSetting EdgeTop => new BorderSetting(nameof(EdgeTop), _xl, _local);
    public BorderSetting EdgeBottom => new BorderSetting(nameof(EdgeBottom), _xl, _local);
    public BorderSetting EdgeLeft => new BorderSetting(nameof(EdgeLeft), _xl, _local);
    public BorderSetting EdgeRight => new BorderSetting(nameof(EdgeRight), _xl, _local);
    public BorderSetting InsideVertical => new BorderSetting(nameof(InsideVertical), _xl, _local);
    public BorderSetting InsideHorizontal => new BorderSetting(nameof(InsideHorizontal), _xl, _local);
    public BorderSetting DiagonalDown => new BorderSetting(nameof(DiagonalDown), _xl, _local);
    public BorderSetting DiagonalUp => new BorderSetting(nameof(DiagonalUp), _xl, _local);
    public BordersObject(string? address) : base(address) { }
}
