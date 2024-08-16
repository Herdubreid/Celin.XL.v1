using BlazorState;
using System.Text;

namespace Celin.XL.Sharp;

public partial class AppState : State<AppState>
{
    public bool Busy { get; set; }
    public IAction? NextAction { get; set; }
    public string? ScriptKey { get; set; }
    public string? Command { get; set; }
    public Dictionary<string, Services.Script> Scripts { get; set; } = new Dictionary<string, Services.Script>();
    public List<string> History { get; } = new List<string>();
    public StringBuilder Output { get; set; } = new StringBuilder();
    public string? CommandError { get; set; }
    public override void Initialize() { }
}
