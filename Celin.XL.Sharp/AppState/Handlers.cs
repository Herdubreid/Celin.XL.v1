using BlazorState;
using Celin.Language;
using Celin.XL.Sharp.Services;
using Pidgin;
using static Pidgin.Parser<char>;

namespace Celin.XL.Sharp;

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

            await _sharp.Submit(aAction.PromptCommand);
        }
        readonly JsService _js;
        readonly SharpService _sharp;
        public PromptCommandHandler(IStore store, JsService js, SharpService sharp) : base(store)
        {
            _js = js;
            _sharp = sharp;
        }
    }
}
