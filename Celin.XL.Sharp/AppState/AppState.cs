using BlazorState;
using System.Text;

namespace Celin.XL.Sharp;

public partial class AppState : State<AppState>
{
    public string? Command { get; set; }
    public StringBuilder Output { get; set; } = new StringBuilder();
    public string? CommandError { get; set; }
    public override void Initialize() { }
}
