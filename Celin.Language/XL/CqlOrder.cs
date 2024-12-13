using Celin.AIS;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record CqlOrderProperties(string direction, IEnumerable<string> Ids)
{
    public static explicit operator List<AggregationItem>(CqlOrderProperties order) =>
        order.Ids
        .Select(s => new AggregationItem { direction = order.direction, column = s })
        .ToList();
}
public class CqlOrderParser : BaseParser
{
    static Parser<char, CqlOrderProperties> asc =>
        Tok(nameof(asc))
        .Then(ALIAS.Separated(COMMA_SEPARATOR).InBraces())
        .Select(l => new CqlOrderProperties(AggregationItem.ASC, l));
    static Parser<char, CqlOrderProperties> desc =>
        Tok(nameof(desc))
        .Then(ALIAS.Separated(COMMA_SEPARATOR).InBraces())
        .Select(l => new CqlOrderProperties(AggregationItem.DESC, l));
    public static Parser<char, CqlOrderProperties> ORDER =>
        OneOf(asc, desc);
}
