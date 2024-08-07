using BlazorState;
using Celin.AIS;
using Celin.XL.Sharp.Services;
using MediatR;
using System.Text;

namespace Celin.XL.Sharp;

public partial class AppState
{
    public class AuthenticateHandler : ActionHandler<AuthenticateAction>
    {
        public override async Task Handle(AuthenticateAction aAction, CancellationToken aCancellationToken)
        {
            var host = _e1.Default.Server;

            if (string.IsNullOrEmpty(aAction.Username) &&
                string.IsNullOrEmpty(host.AuthRequest.password))
            {
                await _js.Login(_e1.Default.Name, host.AuthRequest.username);
                return;
            }

            try
            {
                host.AuthRequest.username = aAction.Username;
                host.AuthRequest.password = aAction.Password;
                await host.AuthenticateAsync();
            }
            catch (AisException ex)
            {
                if (string.IsNullOrEmpty(aAction.Username))
                {
                    host.AuthRequest.password = null;
                    await _js.Login(_e1.Default.Name, host.AuthRequest.username);
                }
                else
                    await _js.MessageDlg(ErrorMsg(ex));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                await _js.CloseDlg();
            }
        }
        readonly E1Services _e1;
        readonly JsService _js;
        public AuthenticateHandler(IStore store, E1Services e1, JsService js)
            : base(store)
        {
            _e1 = e1;
            _js = js;
        }
    }
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
            catch (AisException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden ||
                    ex.HttpStatusCode == (System.Net.HttpStatusCode)444)
                {
                    await _mediator.Send(new AuthenticateAction());
                }
                else
                {
                    State.CommandError = ErrorMsg(ex);
                    _logger.LogError(ex, nameof(Handle));
                }
            }
            catch (Exception ex)
            {
                State.CommandError = ex.Message;
                _logger.LogError(ex, nameof(Handle));
            }
        }
        readonly ILogger _logger;
        readonly SharpService _sharp;
        readonly IMediator _mediator;
        public PromptCommandHandler(IStore store, ILogger<PromptCommandHandler> logger, SharpService sharp, IMediator mediator)
            : base(store)
        {
            _sharp = sharp;
            _logger = logger;
            _mediator = mediator;
        }
    }
    static StringBuilder FormatErrorMsg(IEnumerable<ErrorWarning> errors)
         => errors.Aggregate(new StringBuilder(), (s, e) => s.AppendLine(e.DESC));
    static string ErrorMsg(AisException ex)
        => ex.ErrorResponse?.errorDetails is null
        ? ex.ErrorResponse?.message ?? ex.Message
        : FormatErrorMsg(ex.ErrorResponse.errorDetails.errors).ToString();
}
