using System.Text;

namespace Celin.Language;

public static class MatrixExtensions
{
    public static object?[]? Row(this object?[,] source)
    {
        var a = new object?[source.GetLength(1)];
        for (int col = 0; col < source.GetLength(1); col++)
            a[col] = source[0, col];
        return a;
    }
    public static object? Single(this object?[,] source)
        => source[0, 0];

    public static string ToMatrixString(this object?[,] source)
    {
        StringBuilder sb = new StringBuilder();
        for (int row = 0; row < source.GetLength(0); row++)
        {
            sb.Append($"{row,4}: {source[row, 0]}");
            for (int col = 1; col < source.GetLength(1); col++)
                sb.Append($", {source[row,col]}");
            sb.AppendLine();
        }
        return sb.ToString();
    }
    public static IEnumerable<object?> Flatten(this IEnumerable<IEnumerable<object?>> source)
        => source.SelectMany(x => x);
    public static string ToMatrixString(this IEnumerable<IEnumerable<object?>> source)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var r in source ?? Enumerable.Empty<IEnumerable<IEnumerable<object?>>>())
        {
            sb.AppendLine(string.Join(", ", r));
        }
        return sb.ToString();
    }
    public static (int rows, int cols) MatrixCount(this IEnumerable<IEnumerable<object?>> source)
        => (source.Count(),
            source.Count() > 0 ? source.Max(m => m.Count()) : 0);
}
