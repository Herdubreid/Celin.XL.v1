using Celin.Language.XL;

namespace Celin.Language;

public class E1
{
    public record Host(string Name, AIS.Server Server);
    public Host Default { get; set; } = null!;
    public IReadOnlyCollection<Host> Hosts { get; }
    public E1(IReadOnlyCollection<Host> hosts)
    {
        Hosts = hosts;
    }
}
public class ScriptShell
{
    public static RangeObject Range(string address) =>
        RangeObject.Range(address);
    public static SheetObject Sheet(string name) =>
        SheetObject.Sheet(name);
    public QueryObject Query(string query) => QueryObject.Query(E1.Default.Server, query);
    public E1 E1 { get; }
    public ScriptShell(E1 e1)
    {
        E1 = e1;
    }
}
