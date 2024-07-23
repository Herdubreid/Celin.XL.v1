using System.Text;
using System.Text.RegularExpressions;

namespace Celin.Language.XL;

public delegate void SetRangeValue((string? sheet, string? cells, string? name) address, object?[,] value);
public delegate object?[,] GetRangeValue((string? sheet, string? cells, string? name) address);
public class XL
{
    public class RangeObject
    {
        static readonly Regex CELLREF = new Regex(@"([a-zA-Z]+)(\d+)(?::([a-zA-Z]+)(\d+))?");
        public static SetRangeValue SetRangeValue { get; set; } = null!;
        public static GetRangeValue GetRangeValue { get; set; } = null!;
        public RangeObject Sheet(string sheet)
        {
            _sheet = sheet;
            return this;
        }
        public RangeObject Cells(string cells)
        {
            _cells = cells.ToUpper();
            return this;
        }
        public RangeObject Resize(int deltaRows, int deltaColumns)
        {
            var cells = CELLREF.Match(_cells ?? throw new ArgumentNullException(nameof(Cells)));
            if (cells.Success)
            {
                int row = int.Parse(
                    string.IsNullOrEmpty(cells.Groups[4].Value)
                    ? cells.Groups[2].Value : cells.Groups[4].Value)
                    + deltaRows;
                int col = ColumnToNumber(
                    string.IsNullOrEmpty(cells.Groups[3].Value)
                    ? cells.Groups[1].Value : cells.Groups[3].Value)
                    + deltaColumns;
                _cells = $"{cells.Groups[1]}{cells.Groups[2]}:{NumberToColumn(col)}{row}";
            }
            else
                throw new ArgumentException($"Invalid Cell Reference: {_cells}");

            return this;
        }
        public RangeObject Name(string name)
        {
            _name = name;
            return this;
        }
        public object?[,] Values
        {
            get => GetRangeValue((_sheet, _cells, _name));
            set => SetRangeValue((_sheet, _cells, _name), value);
        }
        public override string ToString()
            => _name == null
            ? $"{_sheet}!{_cells}"
            : _name;
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
        string? _cells;
        string? _sheet;
        string? _name;
        internal RangeObject() { }
    }
    public static RangeObject Range
        => new RangeObject();
}
