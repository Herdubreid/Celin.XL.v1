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
        dummy,
    }
    string App(app f) => $"{LIB}.{nameof(app)}.{f:g}";
    public ValueTask Init()
        => _js.InvokeVoidAsync(App(app.init), _ref);
    public ValueTask InitCommandPrompt(string id)
        => _js.InvokeVoidAsync(App(app.initCommandPrompt), id);
    public ValueTask<string> Dummy()
        => _js.InvokeAsync<string>(App(app.dummy));
    #endregion
    #region Excel
    enum xl
    {
        syncSheet,
        syncRange,
        syncValues,
    }
    string XL(xl f) => $"{LIB}.{nameof(xl)}.{f:g}";
    public ValueTask<string> syncValues(string key, ValuesProperties<object?> values)
        => _js.InvokeAsync<string>(XL(xl.syncValues), key, values.local);
    public ValueTask<string> syncRange(string key, RangeProperties values)
        => _js.InvokeAsync<string>(XL(xl.syncRange), key, values);
    public ValueTask<SheetProperties> syncSheet(string key, SheetProperties values)
        => _js.InvokeAsync<SheetProperties>(XL(xl.syncSheet), key, values);
    #endregion
    #region invokables
    [JSInvokable]
    public void PromptCommand(string prompt)
        => _mediator.Send(new PromptCommandAction { PromptCommand = prompt });
    public void SetError(string msg)
        => _mediator.Send(new SetErrorAction { ErrorMsg = msg });
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
