using Celin.AIS;
using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record CqlConditionProperties(string Id, string Operator, IEnumerable<string> Values)
{
    public static implicit operator Condition(CqlConditionProperties condition) =>
        new Condition
        {
            controlId = condition.Id,
            @operator = condition.Operator,
            value = condition.Values.Select(v => new Value
            {
                content = v,
                specialValueId = Value.LITERAL
            })
        };
}
public record CqlFilterProperties(
    string AndOr,
    string MatchType,
    IEnumerable<CqlConditionProperties> Conditions)
{
    public static implicit operator Query(CqlFilterProperties queryFilter) =>
        new Query
        {
            matchType = queryFilter.MatchType,
            condition = queryFilter.Conditions.Select(c => (Condition)c)
        };

    public static implicit operator ComplexQuery(CqlFilterProperties queryFilter) =>
        new ComplexQuery
        {
            andOr = queryFilter.AndOr,
            query = queryFilter
        };
}
public class CqlFilterParser : BaseParser
{
    static Parser<char, string> BETWEEN =>
        Tok("bw").ThenReturn(Condition.BETWEEN);
    static Parser<char, string> LIST =>
        Tok("in").ThenReturn(Condition.LIST);
    static Parser<char, string> NOT_EQUAL =>
        Tok("<>").ThenReturn(Condition.NOT_EQUAL);
    static Parser<char, string> LESS_EQUAL =>
        Tok("<=").ThenReturn(Condition.LESS_EQUAL);
    static Parser<char, string> GREATER_EQUAL =>
        Tok(">=").ThenReturn(Condition.GREATER_EQUAL);
    static Parser<char, string> EQUAL =>
        Tok("=").ThenReturn(Condition.EQUAL);
    static Parser<char, string> GREATER =>
        Tok(">").ThenReturn(Condition.GREATER);
    static Parser<char, string> LESS =>
        Tok("<").ThenReturn(Condition.LESS);
    static Parser<char, string> STR_START_WITH =>
        Tok("^").ThenReturn(Condition.STR_START_WITH);
    static Parser<char, string> STR_END_WITH =>
        Tok("$").ThenReturn(Condition.STR_END_WITH);
    static Parser<char, string> STR_CONTAIN =>
        Tok("?").ThenReturn(Condition.STR_CONTAIN);
    static Parser<char, string> STR_BLANK =>
        Tok("_").ThenReturn(Condition.STR_BLANK);
    static Parser<char, string> STR_NOT_BLANK =>
        Tok("!").ThenReturn(Condition.STR_NOT_BLANK);
    static Parser<char, string> OPERATOR =>
        OneOf(
            BETWEEN,
            LIST,
            NOT_EQUAL,
            LESS_EQUAL,
            GREATER_EQUAL,
            EQUAL,
            GREATER,
            LESS,
            STR_START_WITH,
            STR_END_WITH,
            STR_CONTAIN,
            STR_BLANK,
            STR_NOT_BLANK);
    static Parser<char, CqlConditionProperties> CONDITION =>
        Map((id, op, values) => new CqlConditionProperties(id, op, values),
        ALIAS.Trim(),
        OPERATOR.Trim(),
        Values<string>.ARRAY);
    static Parser<char, (string, string)> all =>
        Tok(nameof(all)).ThenReturn((ComplexQuery.AND, Query.MATCH_ALL));
    static Parser<char, (string, string)> any =>
        Tok(nameof(any)).ThenReturn((ComplexQuery.AND, Query.MATCH_ANY));
    static Parser<char, (string, string)> orAll =>
        Tok(nameof(orAll)).ThenReturn((ComplexQuery.OR, Query.MATCH_ALL));
    static Parser<char, (string, string)> orAny =>
        Tok(nameof(orAny)).ThenReturn((ComplexQuery.OR, Query.MATCH_ANY));
    public static Parser<char, CqlFilterProperties> QUERY =>
        Map((qt, cs) => new CqlFilterProperties(qt.Item1, qt.Item2, cs),
        OneOf(all, any, orAll, orAny),
        CONDITION.Separated(Whitespaces).InBraces());

    //    public static Parser<char, QueryFilterProperties> PARSER => QUERY;
    //    public static Parser<char, IEnumerable<Condition>> Parser => CONDITIONS;
    //    public static Parser<char, Condition> Parser => CONDITION;
    //    public static Parser<char, string> Parser => OPERATOR;
}