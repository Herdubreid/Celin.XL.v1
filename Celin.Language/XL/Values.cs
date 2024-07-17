using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public class Values
{
    static readonly Parser<char, object?> STRING
        = SkipWhitespaces
        .Then(OneOf(
            Literal.DoubleQuoted,
            Literal.SingleQuoted,
            Literal.Plain))
        .Cast<object?>();
    static readonly Parser<char, object?> NUMBER
        = SkipWhitespaces
        .Then(Try(DecimalNum.Cast<object?>())
            .Or(Real.Cast<object?>()));
    static readonly Parser<char, IEnumerable<object?>> ARRAY
        = OneOf(STRING, NUMBER)
            .Optional()
            .SeparatedAtLeastOnce(Char(','))
            .Select(a => a
                .Select(e => e.HasValue ? e.Value : null));
    static readonly Parser<char, IEnumerable<IEnumerable<object?>>> MATRIX
        = ARRAY
            .Between(Char('['), Char(']'))
            .SeparatedAtLeastOnce(Char(','));
    public static Parser<char, IEnumerable<IEnumerable<object?>>> Parser
        => Try(MATRIX)
            .Or(ARRAY.Select(a =>
            {
                var l = new List<IEnumerable<object?>> { a };
                return l.AsEnumerable();
            }));
}
