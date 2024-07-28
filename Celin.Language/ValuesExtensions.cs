using System.Text;

namespace Celin.Language;

public static class ValuesExtensions
{
    public static string ToSingle(this IEnumerable<IEnumerable<object?>> source)
        => source.FirstOrDefault()?.FirstOrDefault()?.ToString() ?? string.Empty;
    public static string ToList(this IEnumerable<IEnumerable<object?>> source)
        => string.Join(',', source.FirstOrDefault() ?? Enumerable.Empty<object?>());
    public static string ToMatrix(this IEnumerable<IEnumerable<object?>> source)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var r in source ?? Enumerable.Empty<IEnumerable<IEnumerable<object?>>>())
        {
            sb.AppendLine(string.Join(", ", r));
        }
        return sb.ToString();
    }
    public static IEnumerable<IEnumerable<object?>> ToMatrix(this object? source) => source switch
    {
        string s => XL.Values.Parse(s),
        IEnumerable<IEnumerable<object?>> e => e,
        _ => Enumerable.Empty<IEnumerable<object?>>()
    };
    public static IEnumerable<IEnumerable<object?>> ToMatrix(this string source)
        => XL.Values.Parse(source);
    public static IEnumerable<object?> Flatten(this IEnumerable<IEnumerable<object?>> source)
        => source.SelectMany(x => x);
    public static (int rows, int cols) MatrixCount(this IEnumerable<IEnumerable<object?>> source)
        => (source.Count(),
            source.Count() > 0 ? source.Max(m => m.Count()) : 0);
}
