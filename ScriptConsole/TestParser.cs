using Celin.Language;
using Microsoft.Extensions.Logging;
using Pidgin;
using static Pidgin.Parser<char>;

namespace Celin;

static class TestParser
{
    static Language.XL.AddresType Range(string cmd)
        => Language.XL.Address.Parser
            .Before(End).ParseOrThrow(cmd);
    static IEnumerable<IEnumerable<object>> ParseValue(string value)
        => Language.XL.Values.Parser
            .Before(End).ParseOrThrow(value);
    static PromptCommand ParseCommand(string value)
        => PromptCommand.Parser
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
                //var p = Range(ln);
                //Console.WriteLine($"{(p.sheet.HasValue ? p.sheet.Value : string.Empty)}!{p.range}");
                //var m = ParseValue(ln);
                //foreach(var r in m)
                //    foreach(var e in r) Console.WriteLine(e);
                var c = ParseCommand(ln);
                Console.WriteLine($"{c.LeftHand}, {c.RightHand}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
