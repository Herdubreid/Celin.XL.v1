using System.Text;

namespace Celin.Language;

public static class ValuesExtensions
{
    public static bool IsSequential(this IEnumerable<int> source) =>
        !source.Select((e, i) => e == i).Any(e => !e);
    public static IEnumerable<int> GetTransposeOrder(
        this IEnumerable<string> source,
        IList<string> target) => source.Select(target.IndexOf);
    public static IEnumerable<T?> Transpose<T>(
        this IEnumerable<T> source, IList<int> order) =>
        order.IsSequential() ? source : order.Select(ndx => source.ElementAtOrDefault(ndx));
    public static IEnumerable<IEnumerable<T?>> Transpose<T>(
        this IEnumerable<IEnumerable<T>> source, IList<int> order) =>
        order.IsSequential() ? source : source.Select(r => order.Select(ndx => r.ElementAtOrDefault(ndx)));
    public static string ToSingle(this IEnumerable<IEnumerable<object?>> source)
        => source.FirstOrDefault()?.FirstOrDefault()?.ToString() ?? string.Empty;
    public static string ToList(this IEnumerable<IEnumerable<object?>> source)
        => string.Join(',', source.FirstOrDefault() ?? Enumerable.Empty<object?>());
    public static string ToCsv(this IEnumerable<object?> source) => string.Join(",", source);
    public static string ToCsv(this IEnumerable<IEnumerable<object?>> source)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var r in source ?? Enumerable.Empty<IEnumerable<IEnumerable<object?>>>())
        {
            sb.AppendLine(r.ToCsv());
        }
        return sb.ToString();
    }
    public static IEnumerable<IEnumerable<object?>> ToMatrix(this object? source) =>
        source switch
        {
            string s => XL.Values<object?>.Parse(s),
            IEnumerable<IEnumerable<object?>> e => e,
            _ => Enumerable.Empty<IEnumerable<object?>>()
        };
    public static IEnumerable<IEnumerable<object?>> ToMatrix(this string source) =>
        XL.Values<object?>.Parse(source);
    public static IEnumerable<object?> Flatten(this IEnumerable<IEnumerable<object?>> source) =>
        source.SelectMany(x => x);
    public static (int rows, int cols) MatrixCount<T>(this IEnumerable<IEnumerable<T>> source) =>
        (source.Count(),
         source.Count() > 0 ? source.Max(m => m.Count()) : 0);
}
