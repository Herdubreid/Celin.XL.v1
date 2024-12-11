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
            var m =  CellReference.PAT().Match(_address);
            var sheet = string.IsNullOrEmpty(m.Groups[1].Value) ? string.Empty : $"'{m.Groups[1].Value}'!";
            var dim = CellReference.Dim(_address);
            for (int col = dim.Left; col <= dim.Right; col++)
                for (int row = dim.Top; row <= dim.Bottom; row++)
                {
                    string address = $"{sheet}{CellReference.NumberToColumn(col + 1)}{row + 1}";
                    yield return (T2)Activator.CreateInstance(typeof(T2), address)!;
                }
        }
    }
    public string? Address => _address;
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
        var m = CellReference.PAT().Match(_address?.ToUpper() ?? "A1");
        if (m.Success)
        {
            var sheet = string.IsNullOrEmpty(m.Groups[1].Value) ? string.Empty : $"'{m.Groups[1].Value}'!";
            var row = string.IsNullOrEmpty(m.Groups[3].Value)
                ? 1 : int.Parse(m.Groups[3].Value);
            var col = CellReference.ColumnToNumber(m.Groups[4].Success ? m.Groups[4].Value : m.Groups[2].Value);
            _address = $"{sheet}{m.Groups[2]}{row}:{CellReference.NumberToColumn(col + cols - 1)}{row + rows - 1}";
        }
        else
        {
            throw CellReference.InvalidCells(_address!);
        }

        return (T2)this;
    }
    protected string? _address;
    protected T1 _local;
    protected T1 _xl;
    protected RangeBaseObject(string? address)
    {
        if (address != null)
            _ = CellReference.Dim(address);
        _address = address;
        _local = new T1();
        _xl = new T1();
    }
}
