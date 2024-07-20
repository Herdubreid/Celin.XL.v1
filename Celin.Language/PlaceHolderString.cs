using Pidgin;
using System.Text.RegularExpressions;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin.Language;

public class PlaceHolderString
{
    static Regex PLACEHOLDER = new Regex(@"\$([a-zA-Z0-9_]+)");
    static readonly Parser<char, string> PLAIN =
        AnyCharExcept('"', '\'')
            .ManyString()
                .Select(s => PLACEHOLDER.Replace(s, "{$1}"));
    static readonly Parser<char, string> DOUBLE =
        AnyCharExcept('"')
            .ManyString()
                .Between(Char('"'))
                .Select(s => $"\"{s}\"");
    static readonly Parser<char, string> SINGLE =
        AnyCharExcept('\'')
            .ManyString()
                .Between(Char('\''))
                .Select(s => $"\'{s}\'");
    public static Parser<char, string> Parser
        => OneOf(SINGLE, DOUBLE, PLAIN)
            .ManyThen(End)
                .Select(s => string.Join("", s.Item1));
}
