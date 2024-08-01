using Celin.Language;
using Celin.Language.XL;
using Microsoft.Extensions.Logging;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin;

static class TestParser
{
    static (string? sheet, string cells) Range(string cmd)
        => CellReference.Parser
            .Before(End).ParseOrThrow(cmd);
    static IEnumerable<IEnumerable<object?>> ParseValue(string value)
        => Values.Parser
            .Before(End).ParseOrThrow(value);
    static (string LH, Maybe<string> RH) ParseExpression(string value)
        => Expression.Parser
            .Before(End).ParseOrThrow(value);
    static ExpressionType ParseLH(string value)
        => Expression.ParseLH
            .Before(End).ParseOrThrow(value);
    static ExpressionType ParseRH(string value)
        => Expression.ParseRH
            .Before(End).ParseOrThrow(value);
    static Result<char, string> ParsePlaceHolders(string value)
        => PlaceHolderString.Parser
            .Parse(value);
    static IEnumerable<Maybe<string>> Test(string value)
        => Any.AtLeastOnceString().Optional()
            .Separated(Char(','))
            .Before(End).ParseOrThrow(value);
    public static void Run(ILogger log)
    {
        while (true)
        {
            var ln = Console.ReadLine();
            if (string.IsNullOrEmpty(ln))
                break;
            try
            {
                //var s = ParsePlaceHolders(ln);
                //Console.WriteLine(s);
                //var p = Range(ln);
                //Console.WriteLine($"{(p.sheet.HasValue ? p.sheet.Value : string.Empty)}!{p.range}");
                var m = ParseValue(ln);
                Console.WriteLine(m);
                //Console.WriteLine($"{c.LeftHand}, {c.RightHand}");
                //var a = Test(ln);
                //Console.WriteLine(string.Join(",", a
                //    .Select(e => e.HasValue ? e.Value : null)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
