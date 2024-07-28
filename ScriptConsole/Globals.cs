using Celin.Language;
using System.Dynamic;

namespace Celin;

public delegate void ProcessEvent(string? msg);
public class Globals : ScriptShell
{
    public Dictionary<string, object?> Variables { get; } = new Dictionary<string, object?>();
    public record DynamicFormResult : AIS.Form<AIS.FormData<AIS.DynamicJsonElement>>;
    public record ObjectFormResult : AIS.Form<AIS.FormData<IEnumerable<object>>>;
    public record RecordFormResult<T> : AIS.Form<AIS.FormData<T>>;
    public event ProcessEvent OnProcess = null!;
    public void Process(string? msg)
        => OnProcess?.Invoke(msg);
    public Globals(AIS.Server e1) : base(e1)
    {
        dynamic d = new ExpandoObject();
        d.x = "X";
    }
}
