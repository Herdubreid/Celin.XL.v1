using BlazorState;

namespace Celin.XL.Sharp;

public partial class AppState
{
    public class PromptCommandAction : IAction
    {
        public string PromptCommand { get; set; } = string.Empty;
    }
    public class SetErrorAction : IAction
    {
        public string ErrorMsg { get; set; }
    }
}
