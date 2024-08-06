using BlazorState;
using Celin.XL.Sharp.Services;

namespace Celin.XL.Sharp;

public partial class AppState
{
    public class PromptCommandHandler : ActionHandler<PromptCommandAction>
    {
        AppState State => Store.GetState<AppState>();
        public override async Task Handle(PromptCommandAction aAction, CancellationToken aCancellationToken)
        {
            _logger.LogDebug("Submit: {0}", State.Command);
            if (string.IsNullOrEmpty(State.Command))
                return;

            State.CommandError = string.Empty;
            try
            {
                await _sharp.Submit(State.Command);
                State.History.Add(State.Command);
                State.Command = string.Empty;
            }
            catch (Exception ex)
            {
                State.CommandError = ex.Message;
                _logger.LogError(ex, nameof(Handle));
            }
        }
        readonly ILogger _logger;
        readonly SharpService _sharp;
        public PromptCommandHandler(IStore store, ILogger<PromptCommandHandler> logger, SharpService sharp) : base(store)
        {
            _sharp = sharp;
            _logger = logger;
        }
    }
}
