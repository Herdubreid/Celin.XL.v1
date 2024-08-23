using System.Text;
using System.Text.RegularExpressions;

namespace Celin.Language.XL;

public record RangeProperties(
    string? Address = null,
    int? CellCount = null,
    int? ColumnCount = null,
    bool? ColumnHidden = null,
    int? ColumnIndex = null,
    List<List<object?>>? Formulas = null,
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
    List<List<string?>>? Text = null,
    List<List<object?>>? Values = null,
    List<List<string?>>? ValueTypes = null)
{
    public RangeProperties() : this(Address: null) { }
};

public class RangeObject : BaseObject<RangeProperties>
{
    public RangeObject Rows(int rows)
    {
        var m = CELLREF.Match(_xl.Address ?? "A1");
        if (m.Success)
        {
            var row = string.IsNullOrEmpty(m.Groups[3].Value)
                ? 1 : int.Parse(m.Groups[3].Value);
            _xl = _xl with
            {
                Address = $"{ToSheet(m.Groups[1].Value)}{m.Groups[2]}{row}:{(m.Groups[4].Success ? m.Groups[4].Value : m.Groups[2].Value)}{row + rows - 1}"
            };
        }
        else
        {
            throw InvalidCells(_xl.Address!);
        }

        return this;
    }
    public RangeObject Cols(int cols)
    {
        var m = CELLREF.Match(_xl.Address ?? "A1");
        if (m.Success)
        {
            var col = ColumnToNumber(m.Groups[4].Success ? m.Groups[4].Value : m.Groups[2].Value);
            _xl = _xl with
            {
                Address = $"{ToSheet(m.Groups[1].Value)}{m.Groups[2]}{m.Groups[3]}:{NumberToColumn(col + cols - 1)}{m.Groups[5].Value}"
            };
        }
        else
        {
            throw InvalidCells(_xl.Address!);
        }

        return this;
    }
    public override string Key => _xl.Address ?? _local.Address ?? string.Empty;
    public override RangeProperties Properties
    {
        get => _xl;
        protected set => _xl = value;
    }
    public override RangeProperties LocalProperties
    {
        get => _local;
        protected set => _local = value;
    }
    public string? Address => _xl.Address;
    public ListObject<object?> Values =>
        new("values", this, _getLocalValues, _setLocalValues, _getXlValues, _setXlValues);
    public ListObject<string?> NumberFormat =>
        new("numberFormat", this, _getLocalNumerFormat, _setLocalNumberFormat, _getXlNumberFormat, _setXlNumberFormat);
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
            throw InvalidCells(Address);

        return this;
    }
    public static (int Left, int Top, int Right, int Bottom) Dim(string address)
    {
        var m = CELLREF.Match(address ?? throw new ArgumentNullException(nameof(Address)));
        if (m.Success)
        {
            int left = m.Groups[2].Success ? ColumnToNumber(m.Groups[2].Value) - 1 : 0;
            int top = string.IsNullOrEmpty(m.Groups[3].Value)
                ? 0 : int.Parse(m.Groups[3].Value) - 1;
            int right = m.Groups[4].Success
                ? ColumnToNumber(m.Groups[4].Value) - 1
                : left;
            int bottom = string.IsNullOrEmpty(m.Groups[5].Value)
                ? top
                : int.Parse(m.Groups[5].Value) - 1;
            return (left, top, right, bottom);
        }
        throw InvalidCells(address);
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
            throw InvalidCells(cells);
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
    static ArgumentException InvalidCells(string cells) => new ArgumentException($"Invalid Cell Reference: {cells}");
    static string? ToSheet(string? sheet) => string.IsNullOrEmpty(sheet)
        ? null
        : $"'{sheet}'!";
    static readonly Regex CELLREF = new Regex(@"^(?:'?([^']+)'?!)?([a-zA-Z]{1,3})(\d*)(?::([a-zA-Z]{1,3})(\d*))?$");
    #region delegates
    List<List<string?>>? _getLocalNumerFormat() => _local.NumberFormat;
    List<List<string?>>? _getXlNumberFormat() => _xl.NumberFormat;
    void _setLocalNumberFormat(List<List<string?>> values) =>
        _local = _local with { NumberFormat = values };
    void _setXlNumberFormat(List<List<string?>> values) =>
        _xl = _xl with { NumberFormat = values };
    List<List<object?>>? _getLocalValues() => _local.Values;
    List<List<object?>>? _getXlValues() => _xl.Values;
    void _setLocalValues(List<List<object?>> values) =>
        _local = _local with { Values = values };
    void _setXlValues(List<List<object?>> values) =>
        _xl = _xl with { Values = values };
    #endregion
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
