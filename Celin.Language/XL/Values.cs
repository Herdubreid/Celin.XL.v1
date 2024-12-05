using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin.Language.XL;

public class Values<T>
{
    public static readonly Parser<char, T> STRING =
        SkipWhitespaces
            .Then(OneOf(
                Literal.DoubleQuoted,
                Literal.SingleQuoted,
                Literal.Plain))
            .Cast<T>();
    public static readonly Parser<char, T> NUMBER =
        SkipWhitespaces
            .Then(Try(Real.Cast<T>())
            .Or(DecimalNum.Cast<T>()));
    public static readonly Parser<char, IEnumerable<T>> ARRAY =
        OneOf(NUMBER, STRING)
            .Optional()
            .SeparatedAtLeastOnce(Char(','))
            .Select(a => a
                .Select(e => e.HasValue ? e.Value : default!));
    public static readonly Parser<char, IEnumerable<IEnumerable<T>>> MATRIX =
        ARRAY
            .Between(Char('['), Char(']'))
            .SeparatedAtLeastOnce(Char(','));
    public static Parser<char, IEnumerable<IEnumerable<T>>> Parser
        => Try(MATRIX)
                .Select(m =>
                {
                    var sz = m.MatrixCount();
                    var res = m.Select(a => a.Concat(Enumerable.Repeat<T>(default!, sz.cols - a.Count())));
                    return res;
                })
            .Or(ARRAY.Separated(EndOfLine));
    public static IEnumerable<IEnumerable<T>> Parse(string value)
        => Values<T>.Parser
            .Before(End).ParseOrThrow(value);
}
