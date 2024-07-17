using System.Text;

namespace Celin.Language;

public static class MatrixExtensions
{
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
            source.Count() > 0 ? source.First().Count() : 0);
}
