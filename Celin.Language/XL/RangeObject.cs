using System.Text;
using System.Text.RegularExpressions;

namespace Celin.Language.XL;

public record RangeProperties(
    string? address = null,
    int? cellCount = null,
    int? columnCount = null,
    bool? columnHidden = null,
    int? columnIndex = null,
    IEnumerable<IEnumerable<object?>>? formulas = null,
    bool? hasSpill = null,
    decimal? height = null,
    bool? hidden = null,
    bool? isEntireColumn = null,
    bool? isEntireRow = null,
    List<List<string?>>? numberFormat = null,
    int? rowCount = null,
    bool? rowHidden = null,
    int? rowIndex = null,
    string? style = null,
    IEnumerable<IEnumerable<string?>>? text = null,
    List<List<object?>>? values = null,
    IEnumerable<IEnumerable<string?>>? valueTypes = null)
{
    public RangeProperties() : this(address: null) { }
};

public class RangeObject : BaseObject<RangeProperties>
{
    public override string Key => _xl.address ?? _local.address ?? string.Empty;
    public override RangeProperties Properties
    {
        get => _xl;
        protected set => _xl = value;
    }
    public override RangeProperties LocalProperties
    {
        get => _local with { values = _values?.LocalProperties.local };
        protected set
        {
            _local = value;
            _values = new ValuesObject<object?>(Address, value.values, null);
        }
    }
    public string? Address => _xl.address;
    public ValuesObject<object?> Values
    {
        get
        {
            if (_values == null)
                _values = new(Address, _xl.values, _local.values);
            return _values;
        }
    }
    public ValuesObject<string?> NumberFormat => new(Address, _xl.numberFormat, _local.numberFormat);
    public RangeObject Resize(int deltaRows, int deltaColumns)
    {
        var m = CELLREF.Match(Address ?? throw new ArgumentNullException(nameof(Address)));
        if (m.Success)
        {
            int row = int.Parse(
                string.IsNullOrEmpty(m.Groups[5].Value)
                ? m.Groups[3].Value : m.Groups[5].Value)
                + deltaRows;
            int col = ColumnToNumber(
                string.IsNullOrEmpty(m.Groups[4].Value)
                ? m.Groups[1].Value : m.Groups[4].Value)
                + deltaColumns;
            var _cells = $"{m.Groups[1].Value}{m.Groups[3].Value}:{NumberToColumn(col)}{row}";
        }
        else
            throw new ArgumentException($"Invalid Cell Reference: {Address}");

        return this;
    }
    public static (int Left, int Top, int Right, int Bottom) Dim(string address)
    {
        var m = CELLREF.Match(address ?? throw new ArgumentNullException(nameof(Address)));
        if (m.Success)
        {
            int left = ColumnToNumber(m.Groups[2].Value) - 1;
            int top = int.Parse(m.Groups[3].Value) - 1;
            int right = string.IsNullOrEmpty(m.Groups[4].Value)
                ? left : ColumnToNumber(m.Groups[4].Value) - 1;
            int bottom = string.IsNullOrEmpty(m.Groups[5].Value)
                ? top : int.Parse(m.Groups[5].Value) - 1;
            return (left, top, right, bottom);
        }
        throw new ArgumentException($"Invalid Cell Reference: '{address}'");
    }
    static string AdjustCells(string? cells, int rows, int cols)
    {
        var m = CELLREF.Match(cells ?? throw new ArgumentNullException(nameof(Address)));
        if (m.Success)
        {
            if (!string.IsNullOrEmpty(m.Groups[3].Value))
                return cells;
            int startCol = ColumnToNumber(m.Groups[1].Value);
            int startRow = int.Parse(m.Groups[2].Value);
            return $"{m.Groups[1].Value}{m.Groups[3].Value}:{NumberToColumn(startCol + cols - 1)}{startRow + rows - 1}";
        }
        else
            throw new ArgumentException($"Invalid Cell Reference: {cells}");
    }
    static string NumberToColumn(int number)
    {
        StringBuilder column = new StringBuilder();

        while (number > 0)
        {
            number--; // Adjust number to 0-indexed
            column.Insert(0, (char)('A' + number % 26));
            number /= 26;
        }

        return column.ToString();
    }
    static int ColumnToNumber(string column)
    {
        int number = 0;
        for (int i = 0; i < column.Length; i++)
        {
            number *= 26;
            number += column[i] - 'A' + 1;
        }
        return number;
    }
    static readonly Regex CELLREF = new Regex(@"(?:'?([^']+)'?!)?([a-zA-Z]+)(\d+)(?::([a-zA-Z]+)(\d+))?");
    ValuesObject<object?>? _values;
    RangeProperties _local;
    RangeProperties _xl;
    RangeObject(string? address)
    {
        if (address != null)
            _ = Dim(address.ToUpper());
        _local = new RangeProperties();
        _xl = new RangeProperties(address: address?.ToUpper());
    }
    public static RangeObject Range(string? address = null)
        => new(address);
}
