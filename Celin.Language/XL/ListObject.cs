namespace Celin.Language.XL;

public class ListObject<T> : BaseObject<List<List<T>>>
{
    public ListObject<T> Set(string value)
    {
        var v = Values<T>.Parse(value);
        var local = _getLocal() ?? Init;
        for (int row = 0; row < v.Count(); row++)
            for (int col = 0; col < v.ElementAt(row).Count(); col++)
                local[row][col] = v.ElementAt(row).ElementAt(col);
        _setLocal(local);

        return this;
    }
    public T this[int row, int col]
    {
        get => (_getXl() ?? Init).ElementAt(row)!.ElementAt(col)!;
        set
        {
            var local = _getLocal() ?? Init;
            local[row][col] = value;
            _setLocal(local);
        }
    }
    public override string Key => _range.Address ?? string.Empty;
    public override string[] Params { get; }
    public override List<List<T>> Properties
    {
        get => _getXl() ?? Init;
        protected set => _setXl(value);
    }
    public override List<List<T>> LocalProperties
    {
        get => _getLocal() ?? Init;
        protected set => _setLocal(value ?? Init);
    }
    public (int Left, int Top, int Right, int Bottom) Dim { get; protected set; }
    List<List<T>> Init =>
        Enumerable.Range(0, Dim.Bottom - Dim.Top + 1)!
            .Select(_ => Enumerable.Range(0, Dim.Right - Dim.Left + 1)!
               .Select(_ => default(T)!)
               .ToList())
            .ToList();
    readonly RangeObject _range;
    readonly Func<List<List<T>>?> _getLocal;
    readonly Action<List<List<T>>> _setLocal;
    readonly Func<List<List<T>>?> _getXl;
    readonly Action<List<List<T>>> _setXl;
    public ListObject(
        string props,
        RangeObject range,
        Func<List<List<T>>?> getLocal,
        Action<List<List<T>>> setLocal,
        Func<List<List<T>>?> getXl,
        Action<List<List<T>>> setXl)
    {
        Dim = RangeObject.Dim(range.Address!);
        Params = [props];
        _range = range;
        _getLocal = getLocal;
        _setLocal = setLocal;
        _getXl = getXl;
        _setXl = setXl;
    }
}
