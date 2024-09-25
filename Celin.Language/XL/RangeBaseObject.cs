using System.Text;
using System.Text.RegularExpressions;

namespace Celin.Language.XL;
public interface IRangeObjectFactory<T>
{
    T Create(string? address);
}

public abstract partial class RangeBaseObject<T1, T2> : BaseObject<T1>
    where T1 : new()
    where T2 : RangeBaseObject<T1, T2>
{
    public IEnumerator<T2> GetEnumerator()
    {
        if (!string.IsNullOrEmpty(_address))
        {
            var m = CELLREF().Match(_address);
            var dim = Dim(_address);
            for (int col = dim.Left; col <= dim.Right; col++)
                for (int row = dim.Top; row <= dim.Bottom; row++)
                {
                    string address = $"{ToSheet(m.Groups[1].Value)}{NumberToColumn(col + 1)}{row + 1}";
                    yield return (T2)Activator.CreateInstance(typeof(T2), address)!;
                }
        }
    }
    public string? Address => _address;
    public string? CellRange
    {
        get
        {
            var m = CELLREF().Match(_address?.ToUpper() ?? throw new ArgumentNullException(nameof(Dim)));
            if (m.Success)
            {
                return $"{m.Groups[2]}{m.Groups[3].Value}{(string.IsNullOrEmpty(m.Groups[4].Value) ? "" : $":{m.Groups[4].Value}{m.Groups[5].Value}")}";
            }
            throw InvalidCells(_address);
        }
    }
    public override string? Key => _address;
    public override T1 Properties
    {
        get => _xl;
        protected set => _xl = value;
    }
    public override T1 LocalProperties
    {
        get => _local;
        set => _local = value;
    }
    public T2 Expand(int cols, int rows)
    {
        var m = CELLREF().Match(_address?.ToUpper() ?? "A1");
        if (m.Success)
        {
            var row = string.IsNullOrEmpty(m.Groups[3].Value)
                ? 1 : int.Parse(m.Groups[3].Value);
            var col = ColumnToNumber(m.Groups[4].Success ? m.Groups[4].Value : m.Groups[2].Value);
            _address = $"{ToSheet(m.Groups[1].Value)}{m.Groups[2]}{row}:{NumberToColumn(col + cols - 1)}{row + rows - 1}";
        }
        else
        {
            throw InvalidCells(_address!);
        }

        return (T2)this;
    }
    public static (int Left, int Top, int Right, int Bottom) Dim(string address)
    {
        var m = CELLREF().Match(address?.ToUpper() ?? throw new ArgumentNullException(nameof(Dim)));
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
    protected static string NumberToColumn(int number)
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
    protected static int ColumnToNumber(string column)
    {
        int number = 0;
        for (int i = 0; i < column.Length; i++)
        {
            number *= 26;
            number += column[i] - 'A' + 1;
        }
        return number;
    }
    protected static ArgumentException InvalidCells(string cells) => new ArgumentException($"Invalid Cell Reference: {cells}");
    protected static string? ToSheet(string? sheet) => string.IsNullOrEmpty(sheet)
        ? null
        : $"'{sheet}'!";
    protected string? _address;
    protected T1 _local;
    protected T1 _xl;
    protected RangeBaseObject(string? address)
    {
        if (address != null)
            _ = Dim(address);
        _address = address;
        _local = new T1();
        _xl = new T1();
    }

    [GeneratedRegex(@"^(?:'?([^']+)'?!)?([a-zA-Z]{0,3})(\d*)(?::([a-zA-Z]{1,3})(\d*))?$")]
    private static partial Regex CELLREF();
}
