using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin.Language.XL;

public class Values
{
    static readonly Parser<char, object?> STRING =
        SkipWhitespaces
            .Then(OneOf(
                Literal.DoubleQuoted,
                Literal.SingleQuoted,
                Literal.Plain))
            .Cast<object?>();
    static readonly Parser<char, object?> NUMBER =
        SkipWhitespaces
            .Then(Try(DecimalNum.Cast<object?>())
            .Or(Real.Cast<object?>()));
    static readonly Parser<char, IEnumerable<object?>> ARRAY =
        OneOf(STRING, NUMBER)
            .Optional()
            .SeparatedAtLeastOnce(Char(','))
            .Select(a => a
                .Select(e => e.HasValue ? e.Value : null));
    static readonly Parser<char, IEnumerable<IEnumerable<object?>>> MATRIX =
        ARRAY
            .Between(Char('['), Char(']'))
            .SeparatedAtLeastOnce(Char(','));
    public static Parser<char, IEnumerable<IEnumerable<object?>>> Parser
        => Try(MATRIX)
                .Select(m =>
                {
                    var sz = m.MatrixCount();
                    var res = m.Select(a => a.Concat(Enumerable.Repeat<object?>(null, sz.cols - a.Count())));
                    return res;
                })
            .Or(ARRAY
                .Select(a =>
                {
                    var res = new List<IEnumerable<object?>> { a };
                    return res.AsEnumerable();
                }));
    public static IEnumerable<IEnumerable<object?>> Parse(string value)
        => Values.Parser
            .Before(End).ParseOrThrow(value);
}
