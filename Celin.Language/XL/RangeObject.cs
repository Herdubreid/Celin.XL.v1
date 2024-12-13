using Pidgin;

namespace Celin.Language.XL;

public record RangeProperties() : BaseProperties
{
    public string? Address { get; set; }
    public int? CellCount { get; set; }
    public int? ColumnCount { get; set; }
    public bool? ColumnHidden { get; set; }
    public int? ColumnIndex { get; set; }
    public List<List<object?>>? Formulas { get; set; }
    public bool? HasSpill { get; set; }
    public decimal? Height { get; set; }
    public bool? Hidden { get; set; }
    public bool? IsEntireColumn { get; set; }
    public bool? IsEntireRow { get; set; }
    public List<List<string?>>? NumberFormat { get; set; }
    public int? RowCount { get; set; }
    public bool? RowHidden { get; set; }
    public int? RowIndex { get; set; }
    public string? Style { get; set; }
    public List<List<string?>>? Text { get; set; }
    public List<List<object?>>? Values { get; set; }
    public List<List<string?>>? ValueTypes { get; set; }
};

public class RangeObject : RangeBaseObject<RangeProperties, RangeObject>
{
    public override RangeProperties Properties
    {
        get => _xl;
        protected set
        {
            _xl = value;
            _address = _xl.Address;
        }
    }
    public async Task Set(params BaseProperties[] properties)
    {
        foreach (var prop in properties)
        {
            switch (prop)
            {
                case RangeProperties range:
                    await Set(Key, range, Params);
                    break;
                case FormatProperties format:
                    await FormatObject.Set(Key, format, Params);
                    break;
                case FontProperties font:
                    await FontObject.Set(Key, font, Params);
                    break;
                case FillProperties fill:
                    await FillObject.Set(Key, fill, Params);
                    break;
                default:
                    throw new InvalidCastException(prop.ToString());
            };
        }
    }
    public FormatObject Format => FormatObject.Format(_address);
    public ListObject<string?> ValueTypes
    {
        get => new("valueTypes", this, _getLocalValueTypes, _setLocalValueTypes, _getXlValueTypes, _setXlValueTypes);
        set => _local = _local with { ValueTypes = value.Properties };
    }
    public ListObject<string?> Text
    {
        get => new("text", this, _getLocalText, _setLocalText, _getXlText, _setXlText);
        set => _local = _local with { Text = value.Properties };
    }
    public ListObject<object?> Formulas
    {
        get => new("values", this, _getLocalFormulas, _setLocalFormulas, _getXlFormulas, _setXlFormulas);
        set => _local = _local with { Formulas = value.Properties };
    }
    public ListObject<object?> Values
    {
        get => new("values", this, _getLocalValues, _setLocalValues, _getXlValues, _setXlValues);
        set => _local = _local with { Values = value.Properties };
    }
    public ListObject<string?> NumberFormat
    {
        get => new("numberFormat", this, _getLocalNumerFormat, _setLocalNumberFormat, _getXlNumberFormat, _setXlNumberFormat);
        set => _local = _local with { NumberFormat = value.Properties };
    }
    #region delegates
    List<List<string?>>? _getLocalValueTypes() => _local.ValueTypes;
    List<List<string?>>? _getXlValueTypes() => _xl.ValueTypes;
    void _setLocalValueTypes(List<List<string?>>? values) =>
        _local = _local with { ValueTypes = values };
    void _setXlValueTypes(List<List<string?>>? values) =>
        _xl = _xl with { ValueTypes = values };
    List<List<string?>>? _getLocalText() => _local.Text;
    List<List<string?>>? _getXlText() => _xl.Text;
    void _setLocalText(List<List<string?>>? values) =>
        _local = _local with { Text = values };
    void _setXlText(List<List<string?>>? values) =>
        _xl = _xl with { Text = values };
    List<List<string?>>? _getLocalNumerFormat() => _local.NumberFormat;
    List<List<string?>>? _getXlNumberFormat() => _xl.NumberFormat;
    void _setLocalNumberFormat(List<List<string?>>? values) =>
        _local = _local with { NumberFormat = values };
    void _setXlNumberFormat(List<List<string?>>? values) =>
        _xl = _xl with { NumberFormat = values };
    List<List<object?>>? _getLocalFormulas() => _local.Formulas;
    List<List<object?>>? _getXlFormulas() => _xl.Formulas;
    void _setLocalFormulas(List<List<object?>>? values) =>
        _local = _local with { Formulas = values };
    void _setXlFormulas(List<List<object?>>? values) =>
        _xl = _xl with { Formulas = values };
    List<List<object?>>? _getLocalValues() => _local.Values;
    List<List<object?>>? _getXlValues() => _xl.Values;
    void _setLocalValues(List<List<object?>>? values) =>
        _local = _local with { Values = values };
    void _setXlValues(List<List<object?>>? values) =>
        _xl = _xl with { Values = values };
    #endregion
    public static async Task Set(string? key, RangeProperties prop, object?[] pars) =>
        await SyncAsyncDelegate(key, prop, pars);
    public RangeObject(string? address) : base(address) { }
    public RangeObject(RangeProperties xl) : base(xl.Address)
    {
        _xl = xl;
    }
    public static RangeObject Range(string? address = null) => new(address);
    #region operators
    public static explicit operator RangeObject(WorkbookObject<RangeProperties> wb) =>
        new RangeObject(wb.Properties);
    #endregion
}
public class RangeParser : BaseParser
{
    static Parser<char, Action<RangeObject>> Address =>
        Tok(nameof(Address)).Then(ADDRESS_PARAMETER)
        .Select<Action<RangeObject>>(a => range => range.LocalProperties.Address = a);
    static Parser<char, Action<RangeObject>> ColumnCount =>
        Tok(nameof(ColumnCount)).Then(INT_PARAMETER)
        .Select<Action<RangeObject>>(i => range => range.LocalProperties.ColumnCount = i);
    static Parser<char, Action<RangeObject>> ColumnHidden =>
        Tok(nameof(ColumnHidden)).Then(BOOL_PARAMETER)
        .Select<Action<RangeObject>>(b => range => range.LocalProperties.ColumnHidden = b);
    static Parser<char, Action<RangeObject>> RowHidden =>
        Tok(nameof(RowHidden)).Then(BOOL_PARAMETER)
        .Select<Action<RangeObject>>(b => range => range.LocalProperties.RowHidden = b);
    static Parser<char, Action<RangeObject>> Formulas =>
        Tok(nameof(Formulas)).Then(OBJECT_MATRIX_PARAMETER)
        .Select<Action<RangeObject>>(m => range => range.LocalProperties.Formulas = m);
    static Parser<char, Action<RangeObject>> NumberFormat =>
        Tok(nameof(NumberFormat)).Then(STRING_MATRIX_PARAMETER)
        .Select<Action<RangeObject>>(m => range => range.LocalProperties.NumberFormat = m);
    static Parser<char, Action<RangeObject>> Text =>
        Tok(nameof(Text)).Then(STRING_MATRIX_PARAMETER)
        .Select<Action<RangeObject>>(m => range => range.LocalProperties.Text = m);
    static Parser<char, Action<RangeObject>> Values =>
        Tok(nameof(Values)).Then(OBJECT_MATRIX_PARAMETER)
        .Select<Action<RangeObject>>(m => range => range.LocalProperties.Values = m);
    static Parser<char, Action<RangeObject>> ValueTypes =>
        Tok(nameof(ValueTypes)).Then(STRING_MATRIX_PARAMETER)
        .Select<Action<RangeObject>>(m => range => range.LocalProperties.ValueTypes = m);
    static Parser<char, Action<RangeObject>> Style =>
        Tok(nameof(Style)).Then(STRING_PARAMETER)
        .Select<Action<RangeObject>>(s => range => range.LocalProperties.Style = s);
}