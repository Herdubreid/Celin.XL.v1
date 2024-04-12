using BlazorState;

namespace Celin;

public partial class AppState
{
    public class SubjectLookupAction : IAction
    {
        public string? Filter { get; set; }
    }
    public class SubjectDemoLookupAction : IAction
    {
        public string? Subject { get; set; }
    }
    public class AuthenticateAction : IAction
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
    public class SubmitQueryAction : IAction
    {
        public string? Query { get; set; }
        public string? Template { get; set; }
    }
    public class SubmitScriptAction : IAction
    {
        public string? Script { get; set; }
        public string? Template { get; set; }
        public bool ValidateOnly { get; set; }
    }
    public class CancelQueryAction : IAction
    {
        public string? Id { get; set; }
    }
    public class CancelScriptAction : IAction
    {
        public string? Id { get; set; }
    }
    public class ClearScriptOutputAction : IAction { }
    public class SelectContextAction : IAction
    {
        public int ContextId { get; set; }
    }
}
