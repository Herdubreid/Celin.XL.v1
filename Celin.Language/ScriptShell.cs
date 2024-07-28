using Celin.Language.XL;

namespace Celin.Language;

public class ScriptShell
{
    public static RangeObject Range => RangeObject.Range;
    public QueryObject Query(string query) => QueryObject.Query(_e1, query);
    AIS.Server _e1;
    protected ScriptShell(AIS.Server e1)
    {
        _e1 = e1;
    }
}
