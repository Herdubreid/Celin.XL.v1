using BlazorState;
using Celin.Language;
using Pidgin;
using TypeSupport.Extensions;
using static Pidgin.Parser<char>;

namespace Celin.XL.CSharp;

public partial class AppState
{
    public class PromptCommandHandler : ActionHandler<PromptCommandAction>
    {
        AppState State => Store.GetState<AppState>();
        public override Task Handle(PromptCommandAction aAction, CancellationToken aCancellationToken)
        {
            State.ErrorMsg = null;
            State.Result = null;
            try
            {
                var result = PromptCommand.Parser
                        .Before(End).ParseOrThrow(aAction.PromptCommand);

                switch (result.LeftHand.Cmd)
                {
                    case Cmds.variable:
                        var variable = State.Variables
                            .GetValueOrDefault(result.LeftHand.Argument);
                        if (result.RightHand != null)
                        {
                            switch (result.RightHand.Cmd)
                            {
                                case Cmds.value:
                                    State.Variables[result.LeftHand.Argument] =
                                        result.RightHand.Value;
                                    break;
                                default:
                                    State.ErrorMsg = $"'{aAction.PromptCommand}' invalid expression!";
                                    break;
                            }
                        }
                        else
                        {
                            if (variable == null)
                            {
                                State.ErrorMsg = $"'{result.LeftHand.Argument}' is not a known variable!";
                            }
                            else
                            {
                                State.Result = string.Empty;
                                var m = variable as IEnumerable<IEnumerable<object>>;
                                foreach (var r in m)
                                {
                                    State.Result += string.Join(",", r);
                                    State.Result += '\n';
                                }

                            }
                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                State.ErrorMsg = $"'{aAction.PromptCommand}'";
                State.ErrorMsg += '\n';
                State.ErrorMsg += ex.Message;
            }

            return Task.CompletedTask;
        }
        public PromptCommandHandler(IStore store) : base(store) { }
    }
}
