using Microsoft.Extensions.Logging;
using Pidgin;
using System.Dynamic;
using static Pidgin.Parser<char>;

namespace Celin;

public delegate void ProcessEvent(string? msg);
public class Globals
{
    public Dictionary<string, object?> Variables { get; } = new Dictionary<string, object?>();
    public record DynamicFormResult : AIS.Form<AIS.FormData<AIS.DynamicJsonElement>>;
    public record ObjectFormResult : AIS.Form<AIS.FormData<IEnumerable<object>>>;
    public AIS.Request QL(string query)
        => Language.QL.Parse(query);
    public void LogInformation(string msg)
        => _logger.LogInformation(msg);
    public void LogDebug(string msg)
        => _logger.LogDebug(msg);
    public string? Message {  get; set; }
    public event ProcessEvent OnProcess = null!;
    public void Process(string? msg)
        => OnProcess?.Invoke(msg);
    public IEnumerable<IEnumerable<object?>> Values(string range)
        => Language.XL.Values.Parser
        .Before(End).ParseOrThrow(range);
    public AIS.Server E1 { get; }
    ILogger _logger { get; }
    public Globals(AIS.Server e1, ILogger logger)
    {
        E1 = e1;
        _logger = logger;
        dynamic d = new ExpandoObject();
        d.x = "X";
    }
}
