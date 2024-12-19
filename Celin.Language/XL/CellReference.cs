using System.Text;
using System.Text.RegularExpressions;

namespace Celin.Language.XL;

public partial class CellReference
{
    public string Address { get; private set; }
    public static (int Left, int Top, int Right, int Bottom) Dim(string address)
    {
        var m = PAT().Match(address?.ToUpper() ?? throw new ArgumentNullException(nameof(Dim)));
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
    public static string NumberToColumn(int number)
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
    public static int ColumnToNumber(string column)
    {
        int number = 0;
        for (int i = 0; i < column.Length; i++)
        {
            number *= 26;
            number += column[i] - 'A' + 1;
        }
        return number;
    }
    [GeneratedRegex(@"^(?:'?([^']+)'?!)?([a-zA-Z]{0,3})(\d*)(?::([a-zA-Z]{1,3})(\d*))?$")]
    public static partial Regex PAT();
    public static ArgumentException InvalidCells(string cref) => new ArgumentException($"Invalid Cell Reference: {cref}");

    public CellReference(string address)
    {
        var m = PAT().Match(address);
        if (m.Success)
        {
            var sheet = string.IsNullOrEmpty(m.Groups[1].Value) ? string.Empty : $"'{m.Groups[1].Value}'!";
            var left = string.IsNullOrEmpty(m.Groups[2].Value) ? "A" : m.Groups[2].Value.ToUpper();
            var top = string.IsNullOrEmpty(m.Groups[3].Value) ? "1" : m.Groups[3].Value;
            var right = string.IsNullOrEmpty(m.Groups[4].Value) ? left : m.Groups[4].Value.ToUpper();
            var bottom = string.IsNullOrEmpty(m.Groups[5].Value) ? top : m.Groups[5].Value;
            if (left.CompareTo(right) > 0 || top.CompareTo(bottom) > 0)
                throw InvalidCells(address);
            Address = $"{sheet}{left}{top}:{right}{bottom}";
        }
        else
        {
            throw InvalidCells(address);
        }
    }
}
