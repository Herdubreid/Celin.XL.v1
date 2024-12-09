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
    public static FillObject Fill(string? address = null) =>
        FillObject.Fill(address);
    public static FormatObject Format(string? address = null) =>
        FormatObject.Format(address);
    public static RangeObject Range(string? address = null) =>
        RangeObject.Range(address);
    public static WorksheetObject Sheet(string? name = null) =>
        WorksheetObject.Sheet(name);
    public QueryObject Query(string query) => QueryObject.Query(E1.Default.Server, query);
    public E1 E1 { get; }
    public CancellationToken Cancel { get; set; }
    public ScriptShell(E1 e1)
    {
        E1 = e1;
    }
}
