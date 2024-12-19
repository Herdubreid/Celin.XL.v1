using Pidgin;
using static Pidgin.Parser;

namespace Celin.Language.XL;

public record CqlProperties
{
    public string? Target { get; set; }
    public string? Max { get; set; }
    public IEnumerable<string>? Fields { get; set; }
    public List<CqlFilterProperties> Filters { get; set; } =
        new List<CqlFilterProperties>();
    public List<CqlOrderProperties> Order { get; set; } =
        new List<CqlOrderProperties>();
}
public class CqlObject : BaseObject<CqlProperties>
{
    public override string? Key => null;
    public override CqlProperties Properties { get => _props; protected set => _props = value; }
    public override CqlProperties LocalProperties { get => _props; set => _ = value; }
    CqlProperties _props = new CqlProperties();
}
public class CqlParser : BaseParser
{
    static Parser<char, Action<CqlObject>> cql =>
        Tok(nameof(cql))
        .Then(STRING_PARAMETER)
        .Select<Action<CqlObject>>(s => query => query.Properties.Target = s);
    static Parser<char, Action<CqlObject>> max =>
        Tok(nameof(max))
        .Then(Tok("no").Or(Values<string>.NUMBER).InBraces())
        .Select<Action<CqlObject>>(s => query => query.Properties.Max = s);
    static Parser<char, Action<CqlObject>> fields =>
        Tok(nameof(fields))
        .Then(ALIAS.Separated(COMMA_SEPARATOR).InBraces())
        .Select<Action<CqlObject>>(l => query => query.Properties.Fields = l);
    static Parser<char, Action<CqlObject>> FILTER =>
        CqlFilterParser.QUERY
        .Select<Action<CqlObject>>(fs => query => query.Properties.Filters.Add(fs));
    static Parser<char, Action<CqlObject>> ORDER =>
        CqlOrderParser.ORDER
        .Select<Action<CqlObject>>(os => query => query.Properties.Order.Add(os));
    public static Parser<char, IEnumerable<Action<CqlObject>>> OPTIONAL =>
        OneOf(FILTER,ORDER)
        .SeparatedAndOptionallyTerminated(DOT_SEPARATOR);
    static Parser<char, List<Action<CqlObject>>> REQUIRED =>
        Map((cql, max, fields) =>
        {
            var list = new List<Action<CqlObject>> { cql };
            if (max.HasValue) list.Add(max.Value);
            if (fields.HasValue) list.Add(fields.Value);
            return list;
        },
        cql,
        max.DotPrefix().Optional(),
        fields.DotPrefix().Optional());
    public static Parser<char, CqlObject> Query =>
        Map((required, optional) =>
        {
            var query = new CqlObject();
            foreach (var prop in required)
                prop(query);
            if (optional.HasValue)
                foreach (var prop in optional.Value)
                    prop(query);
            return query;
        },
        REQUIRED,
        OPTIONAL.DotPrefix().Optional());
}