using BlazorState;

namespace Celin.XL.Sharp;

public partial class AppState
{
    public class EditScriptAction : IAction
    {
        public string? Key { get; set; }
    }
    public class UpdateScriptAction : IAction
    {
        public string? Script { get; set; }
    }
    public class AuthenticateAction : IAction
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class CancelDialogAction : IAction { }
    public class PromptCommandAction : IAction { }
}
