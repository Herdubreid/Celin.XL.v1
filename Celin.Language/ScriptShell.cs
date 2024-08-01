using Celin.Language.XL;

namespace Celin.Language;

public class ScriptShell
{
    public static RangeObject Range(string address) =>
        RangeObject.Range(address);
    public static SheetObject Sheet(string name) =>
        SheetObject.Sheet(name);
    public QueryObject Query(string query) => QueryObject.Query(_e1, query);
    AIS.Server _e1;
    public ScriptShell(AIS.Server e1)
    {
        _e1 = e1;
    }
}
