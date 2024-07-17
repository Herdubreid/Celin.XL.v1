using Celin.Language;
using Celin.Language.XL;
using Microsoft.Extensions.Logging;
using Pidgin;
using static Pidgin.Parser<char>;
using static Pidgin.Parser;

namespace Celin;

static class TestParser
{
    static Language.XL.AddressType Range(string cmd)
        => Language.XL.Address.Parser
            .Before(End).ParseOrThrow(cmd);
    static IEnumerable<IEnumerable<object?>> ParseValue(string value)
        => Language.XL.Values.Parser
            .Before(End).ParseOrThrow(value);
    static PromptCommand ParseCommand(string value)
        => PromptCommand.Parser
            .Before(End).ParseOrThrow(value);
    static IEnumerable<Maybe<string>> Test(string value)
        => Any.AtLeastOnceString().Optional()
            .Separated(Char(','))
            .Before(End).ParseOrThrow(value);
    public static void Run(ILogger log)
    {
        XL.Range(new AddressType(null, "")).Values
            = Enumerable.Empty<IEnumerable<IEnumerable<object?>>>();
        while (true)
        {
            var ln = Console.ReadLine();
            if (string.IsNullOrEmpty(ln))
                break;
            try
            {
                //var p = Range(ln);
                //Console.WriteLine($"{(p.sheet.HasValue ? p.sheet.Value : string.Empty)}!{p.range}");
                var m = ParseValue(ln);
                Console.WriteLine(m.ToMatrixString());
                //var c = ParseCommand(ln);
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
