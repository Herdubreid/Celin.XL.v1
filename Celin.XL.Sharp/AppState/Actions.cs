using BlazorState;

namespace Celin.XL.Sharp;

public partial class AppState
{
    public class AuthenticateAction : IAction
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class CancelDialogAction : IAction { }
    public class PromptCommandAction : IAction { }
}
