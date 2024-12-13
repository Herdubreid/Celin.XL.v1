using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin.Language.XL;

public class Values<T>
{
    public static Parser<char, bool> TRUE =>
        SkipWhitespaces
        .Then(OneOf(Base.Tok("1"), Base.Tok("yes"), Base.Tok("true")))
        .ThenReturn(true);
    public static Parser<char, bool> FALSE =>
        SkipWhitespaces
        .Then(OneOf(Base.Tok("0"), Base.Tok("no"), Base.Tok("false")))
        .ThenReturn(false);
    public static Parser<char, bool> BOOL =>
        SkipWhitespaces.Then(FALSE.Or(TRUE));
    public static Parser<char, T> STRING =>
        SkipWhitespaces
        .Then(OneOf(
            Literal.DoubleQuoted,
            Literal.SingleQuoted,
            Literal.Plain))
         .Cast<T>();
    public static Parser<char, T> NUMBER =>
        SkipWhitespaces
        .Then(Try(Real.Cast<T>())
        .Or(DecimalNum.Cast<T>()));
    public static Parser<char, List<T>> ARRAY =>
        OneOf(NUMBER, STRING)
        .Optional()
        .SeparatedAtLeastOnce(Char(','))
        .Select(a => a
            .Select(e => e.HasValue ? e.Value : default!)
            .ToList());
    public static Parser<char, List<List<T>>> MATRIX =>
        ARRAY
        .InBracket()
        .SeparatedAtLeastOnce(Char(','))
        .Select(m => m.ToList());
    public static Parser<char, List<List<T>>> Parser =>
        Try(MATRIX)
        .Select(m =>
        {
            var sz = m.MatrixCount();
            var res = m.Select(a => a
                .Concat(Enumerable.Repeat<T>(default!, sz.cols - a.Count()))
                .ToList());
            return res.ToList();
        })
        .Or(ARRAY.Separated(EndOfLine).Select(m => m.ToList()));
    public static IEnumerable<IEnumerable<T>> Parse(string value) =>
        Parser.Before(End).ParseOrThrow(value);
}
