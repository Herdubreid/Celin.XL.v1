using BlazorState;
using Celin.Language;
using Pidgin;
using static Pidgin.Parser<char>;

namespace Celin.XL.CSharp;

public partial class AppState
{
    public class SetErrorHandler : ActionHandler<SetErrorAction>
    {
        AppState State => Store.GetState<AppState>();

        public override Task Handle(SetErrorAction aAction, CancellationToken aCancellationToken)
        {
            State.ErrorMsg = aAction.ErrorMsg;
            return Task.CompletedTask;
        }
        public SetErrorHandler(IStore store) : base(store) { }
    }
    public class PromptCommandHandler : ActionHandler<PromptCommandAction>
    {
        AppState State => Store.GetState<AppState>();
        void SetResults(object result)
        {
            var m = result as IEnumerable<IEnumerable<object>>;
            foreach (var r in m)
            {
                State.Result += string.Join(",", r);
                State.Result += '\n';
            }
        }
        public override async Task Handle(PromptCommandAction aAction, CancellationToken aCancellationToken)
        {
            State.ErrorMsg = null;
            State.Result = null;
            try
            {
                var result = PromptCommand.Parser
                        .Before(End).ParseOrThrow(aAction.PromptCommand);

                switch (result.LeftHand!.Cmd)
                {
                    case Cmds.help:
                        State.Result = "Help message [TODO]";
                        break;
                    case Cmds.ls:
                        State.Result = "List... [TODO]";
                        break;
                    case Cmds.xlrange:
                        try
                        {
                            var range = await _js.GetRange(
                                result.LeftHand.Address!.sheet,
                                result.LeftHand.Address!.range);
                            if (result.RightHand != null)
                            {

                                switch (result.RightHand.Cmd)
                                {
                                    case Cmds.value:
                                        await _js.SetRange(
                                            result.LeftHand.Address!.sheet,
                                            result.LeftHand.Address!.range,
                                            result.RightHand.Value!);
                                        break;
                                    default:
                                        State.ErrorMsg = $"'{aAction.PromptCommand}' invalid expression!";
                                        break;
                                }
                            }
                            else
                            {
                                State.Result = string.Empty;
                                SetResults(range);
                            }
                        }
                        catch (Exception ex)
                        {
                            State.ErrorMsg = ex.Message;
                        }
                        break;
                    case Cmds.variable:
                        try
                        {

                            var variable = State.Variables
                                .GetValueOrDefault(result.LeftHand.Argument!);
                            if (result.RightHand != null)
                            {
                                switch (result.RightHand.Cmd)
                                {
                                    case Cmds.value:
                                        State.Variables[result.LeftHand.Argument!] =
                                            result.RightHand.Value!;
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
                                    SetResults(variable);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            State.ErrorMsg = ex.Message;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                State.ErrorMsg = $"'{aAction.PromptCommand}'";
                State.ErrorMsg += '\n';
                State.ErrorMsg += ex.Message;
            }
        }
        readonly JsService _js;
        public PromptCommandHandler(IStore store, JsService js) : base(store)
        {
            _js = js;
        }
    }
}
