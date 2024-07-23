using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record AddressType(string? sheet, string range);
public class CellReference
{
    static readonly Parser<char, string> SHEET
        = OneOf(
            Literal.DoubleQuoted,
            Literal.SingleQuoted,
            Literal.Plain)
        .Before(Char('!'));
    static readonly Parser<char, string> CELL
        = Letter
            .Then(Digit.AtLeastOnceString(), (l, r) => l + r);
    static readonly Parser<char, string> RANGE
        = Map((l, r) => r.HasValue
                ? $"{l}:{r.Value}"
                : l,
            CELL,
            Char(':')
                .Then(CELL)
                .Optional());
    public static Parser<char, (string? sheet, string cells)> Parser
        => Map((s, r) => (s.HasValue ? s.Value : null, r),
            Try(SHEET).Optional(),
            RANGE);
}
