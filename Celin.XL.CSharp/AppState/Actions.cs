using BlazorState;

namespace Celin.XL.CSharp;

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
