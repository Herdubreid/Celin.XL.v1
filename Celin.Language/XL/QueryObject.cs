using Pidgin;

namespace Celin.Language.XL;


public record QueryProperties
{
    public string? Target { get; set; }
    public string? Max { get; set; }
    public IEnumerable<string>? Fields { get; set; }
    public enum FilterTypes { all, any, orAll, orAny }
    public IEnumerable<string>? Filter { get; set; }
    public IEnumerable<string>? Sequence { get; set; }
}
public class QueryObject : BaseObject<QueryProperties>
{
    public override string? Key => null;

    public override QueryProperties Properties { get => _props; protected set => _props = value; }
    public override QueryProperties LocalProperties { get => _props; set => _ = value; }
    QueryProperties _props = new QueryProperties();
}
public class QueryParser : BaseParser
{
    static Parser<char, Action<QueryObject>> Cql =>
        Tok(nameof(Cql))
        .Then(STRING_PARAMETER)
        .Select<Action<QueryObject>>(s => query => query.Properties.Target = s);
    static Parser<char, Action<QueryObject>> Max =>
        Tok(nameof(Max))
        .Then(Tok("no").Or(Values<string>.NUMBER))
        .InBraces()
        .Select<Action<QueryObject>>(s => query => query.Properties.Max = s);
    static Parser<char, Action<QueryObject>> Fields =>
        Tok(nameof(Fields))
        .Then(ALIAS).Separated(COMMA_SEPARATOR)
        .InBraces()
        .Select<Action<QueryObject>>(l => query => query.Properties.Fields = l);
    static Parse<char, Action<QueryObject>> Filter =>


}