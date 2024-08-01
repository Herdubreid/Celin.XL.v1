using Celin.Language.XL;

namespace Celin;

static class Matrix
{
    static readonly Dictionary<(int col, int row), object?> _values
        = new Dictionary<(int col, int row), object?>();
    public static Task<SheetObject> Sync(SheetObject value, bool FromExcel)
    {
        value.id = "Synced...";
        return Task.FromResult(value);
    }
    public static Task Set((string? sheet, string? cells, string? name) address, IEnumerable<IEnumerable<object?>> values)
    {
        var c = RangeObject.ToRef(address.cells!);
        for (int row = 0; row < values.Count(); row++)
            for (int col = 0; col < values.ElementAt(row).Count(); col++)
                _values[(col + c.Left, row + c.Top)] = values.ElementAt(row).ElementAt(col);
        return Task.CompletedTask;
    }
    public static Task<IEnumerable<IEnumerable<object?>>> Get((string? sheet, string? cells, string? name) address)
    {
        var c = RangeObject.ToRef(address.cells!);
        var m = Enumerable.Range(0, c.Bottom - c.Top + 1)
            .Select((_, row) => Enumerable.Range(0, c.Right - c.Left + 1)
               .Select((_, col) =>
               {
                   object? val = null;
                   _values.TryGetValue((col + c.Left, row + c.Top), out val);
                   return val;
               }));
       return Task.FromResult(m);
    }
}
