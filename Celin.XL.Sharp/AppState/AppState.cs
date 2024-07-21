using BlazorState;

namespace Celin.XL.Sharp;

public partial class AppState : State<AppState>
{
    public string? Result { get; set; }
    public string? ErrorMsg { get; set; }
    public Dictionary<string, object> Variables { get; set; } = new Dictionary<string, object>();
    public override void Initialize()
    {
    }
}
