using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;

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
    public static Parser<char, object?[,]> Parser
        => Try(MATRIX)
                .Select(m =>
                {
                    var sz = m.MatrixCount();
                    var res = new object?[sz.rows, sz.cols];
                    m.Select((a, row) => a.Select((e, col) => res[row, col] = e));
                    for (int row = 0; row < sz.rows; row++)
                        for (int col = 0; col < m.ElementAt(row).Count(); col++)
                            res[row, col] = m.ElementAt(row).ElementAt(col);
                    return res;
                })
            .Or(ARRAY
                .Select(a =>
                {
                    var res = new object?[1, a.Count()];
                    a.Select((a, i) => res[1, i] = a);
                    for (int col = 0; col < a.Count(); col++)
                        res[0, col] = a.ElementAt(col);
                    return res;
                }));
}
