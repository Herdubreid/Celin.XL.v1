using System.Text;
using System.Text.RegularExpressions;

namespace Celin.Language.XL;

public record RangeProperties(
    string? Address = null,
    int? CellCount = null,
    int? ColumnCount = null,
    bool? ColumnHidden = null,
    int? ColumnIndex = null,
    IEnumerable<IEnumerable<object?>>? Formulas = null,
    bool? HasSpill = null,
    decimal? Height = null,
    bool? Hidden = null,
    bool? IsEntireColumn = null,
    bool? IsEntireRow = null,
    List<List<string?>>? NumberFormat = null,
    int? RowCount = null,
    bool? RowHidden = null,
    int? RowIndex = null,
    string? Style = null,
    IEnumerable<IEnumerable<string?>>? Text = null,
    List<List<object?>>? Values = null,
    IEnumerable<IEnumerable<string?>>? ValueTypes = null)
{
    public RangeProperties() : this(Address: null) { }
};

public class RangeObject : BaseObject<RangeProperties>
{
    public override string Key => _xl.Address ?? _local.Address ?? string.Empty;
    public override RangeProperties Properties
    {
        get => _xl;
        protected set => _xl = value;
    }
    public override RangeProperties LocalProperties
    {
        get => _local with { Values = _values?.LocalProperties.Local };
        protected set
        {
            _local = value;
            _values = new ValuesObject<object?>(Address, value.Values, null);
        }
    }
    public string? Address => _xl.Address;
    public ValuesObject<object?> Values
    {
        get
        {
            if (_values == null)
                _values = new(Address, _xl.Values, _local.Values);
            return _values;
        }
    }
    public ValuesObject<string?> NumberFormat => new(Address, _xl.NumberFormat, _local.NumberFormat);
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
        _xl = new RangeProperties(Address: address?.ToUpper());
    }
    public static RangeObject Range(string? address = null)
        => new(address);
}
