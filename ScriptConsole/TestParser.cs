using Celin.Language;
using Celin.Language.XL;
using Microsoft.Extensions.Logging;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin;

static class TestParser
{
    static AddressType Range(string cmd)
        => Address.Parser
            .Before(End).ParseOrThrow(cmd);
    static object?[,] ParseValue(string value)
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
                //var m = ParseValue(ln);
                //Console.WriteLine(m);
                var ex = ParseExpression(ln);
                //Console.WriteLine($"'{ex.LH}'{(ex.RH.HasValue ? $" = '{ex.RH.Value}'" : string.Empty)}");
                try
                {
                    var lh = ParseLH(ex.LH);
                    Console.Write(lh.Operand);
                }
                catch
                {
                    Console.Write(ex.LH);
                }
                if (ex.RH.HasValue)
                {
                    Console.Write(" = ");
                    
                    try
                    {
                        var ph = ParsePlaceHolders(ex.RH.Value);
                        var rh = ParseRH(ph.Success ? ph.Value : ex.RH.Value);
                        Console.Write(rh.Operand);
                    }
                    catch
                    {
                        Console.Write(ex.RH.Value);
                    }
                }
                Console.WriteLine();
                /*StringBuilder sb = new StringBuilder();
                switch (c.LeftHand.Operand)
                {
                    case Operand.variable:
                        sb.Append($"Variables[{c.LeftHand.Argument}]");
                        break;
                    case Operand.xlrange:
                        string sheet = c.LeftHand.Address!.sheet == null
                            ? string.Empty : $"{c.LeftHand.Address!.sheet}, ";
                        sb.Append($"XL.Range({sheet}{c.LeftHand.Address.range}).Values");
                        break;
                }
                if (c.RightHand != null)
                    switch (c.RightHand.Operand)
                    {
                        case Operand.variable:
                            sb.Append($"Variables[{c.LeftHand.Argument}]");
                            break;
                        case Operand.xlrange:
                            string sheet = c.LeftHand.Address!.sheet == null
                                ? string.Empty : $"{c.LeftHand.Address!.sheet}, ";
                            sb.Append($"XL.Range({sheet}{c.LeftHand.Address.range}).Values");
                            break;
                        case Operand.value:
                            sb.Append($"Values({}")
                    }
                {

                }*/
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
