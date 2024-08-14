using BlazorState;
using Celin.Language.XL;
using MediatR;
using Microsoft.JSInterop;
using static Celin.XL.Sharp.AppState;

namespace Celin.XL.Sharp;

public class JsService
{
    readonly string LIB = "lib";
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    #region app
    enum app
    {
        init,
        initCommandPrompt,
        openLoginDlg,
        openEditorDlg,
        messageDlg,
        closeDlg,
    }
    string App(app f) => $"{LIB}.{nameof(app)}.{f:g}";
    public ValueTask Init()
        => _js.InvokeVoidAsync(App(app.init), _ref);
    public ValueTask InitCommandPrompt(string id)
        => _js.InvokeVoidAsync(App(app.initCommandPrompt), id);
    public ValueTask Login(string title, string? user)
        => _js.InvokeVoidAsync(App(app.openLoginDlg), title, user);
    public ValueTask Editor(string title, string content)
        => _js.InvokeVoidAsync(App(app.openEditorDlg), title, content);
    public ValueTask MessageDlg(string msg)
        => _js.InvokeVoidAsync(App(app.messageDlg), msg);
    public ValueTask CloseDlg()
        => _js.InvokeVoidAsync(App(app.closeDlg));
    #endregion
    #region Excel
    enum xl
    {
        syncSheet,
        syncRange,
        syncValues,
    }
    string XL(xl f) => $"{LIB}.{nameof(xl)}.{f:g}";
    public ValueTask<ValuesProperties<object?>> syncValues(string? key, ValuesProperties<object?> values)
        => _js.InvokeAsync<ValuesProperties<object?>>(XL(xl.syncValues), key, values.Local);
    public ValueTask<RangeProperties> syncRange(string? key, RangeProperties values)
        => _js.InvokeAsync<RangeProperties>(XL(xl.syncRange), key, values);
    public ValueTask<SheetProperties> syncSheet(string? key, SheetProperties values)
        => _js.InvokeAsync<SheetProperties>(XL(xl.syncSheet), key, values);
    #endregion
    #region invokables
    [JSInvokable]
    public void PromptCommand()
        => _mediator.Send(new PromptCommandAction());
    [JSInvokable]
    public void UpdateScript(string script)
        => _mediator.Send(new UpdateScriptAction { Script = script });
    [JSInvokable]
    public void Authenticate(string username, string password)
        => _mediator.Send(new AuthenticateAction { Username = username, Password = password });
    [JSInvokable]
    public void CancelDlg()
        => _mediator.Send(new CancelDialogAction());
    #endregion
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

    readonly IJSRuntime _js;
    readonly IMediator _mediator;
    readonly DotNetObjectReference<JsService> _ref;
    readonly IStore Store;
    AppState State => Store.GetState<AppState>();
    public JsService(IJSRuntime js, IMediator mediator, IStore store)
    {
        _js = js;
        _mediator = mediator;
        _ref = DotNetObjectReference.Create(this);
        Store = store;
    }
}
