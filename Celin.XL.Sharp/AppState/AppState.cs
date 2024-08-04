using BlazorState;

namespace Celin.XL.Sharp;

public partial class AppState : State<AppState>
{
    public string? Result { get; set; }
    public string? ErrorMsg { get; set; }
    public StringWriter Output { get; set; } = new StringWriter();
    public override void Initialize()
    {
        Console.SetOut(Output);
    }
}
