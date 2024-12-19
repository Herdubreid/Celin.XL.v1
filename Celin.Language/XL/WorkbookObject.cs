using Celin.AIS.Data;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Celin.Language.XL;

public record WorkbookProperties(
    string? Name = null)
{
    public enum Methods { getActiveCell }
    public WorkbookProperties() : this(Name: null) { }
}
public class WorkbookObject<T> : BaseObject<T> where T : new()
{
    public void Method(WorkbookProperties.Methods method, params object?[] pars) =>
        _method = new(method, pars);
    public override object?[] Params => _method == null
        ? base.Params
        : new object?[] { _method.Value.Key.ToString() }.Concat(_method.Value.Value).ToArray();
    public override string? Key => null;
    public override T Properties { get => _xl; protected set => _xl = value; }
    public override T LocalProperties { get => _local; set => _local = value; }
    protected T _local = new T();
    protected T _xl = new T();
    protected KeyValuePair<WorkbookProperties.Methods, object?[]>? _method = null;
}
public class WorkbookParser : BaseParser
{
    static Parser<char, string> XL =>
        Tok("xl").Before(DOT_SEPARATOR);
    public static Parser<char, string> Comment =>
        String("/*")
        .Then(Any.Until(Tok("*/")))
        .Select(s => new string(s.ToArray()));
    public static Parser<char, (string Comment, BaseObject Object)> Parser =>
        Map((s, o) => (s.HasValue ? s.Value : string.Empty, o),
        Comment.Optional(),
        SkipWhitespaces.
            Then(XL.
                Then(OneOf(
                    TableParser.Object,
                    RangeParser.Object,
                    WorksheetParser.Object))));
}