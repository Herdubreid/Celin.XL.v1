using BlazorState;
using Celin.AIS;
using System.Text;

namespace Celin.XL.Sharp;

public partial class AppState : State<AppState>
{
    public bool Busy { get; set; }
    public string? Command { get; set; }
    public List<string> History { get; } = new List<string>();
    public StringBuilder Output { get; set; } = new StringBuilder();
    public string? CommandError { get; set; }
    public override void Initialize() { }
}
