using BlazorState;
using MediatR;
using Microsoft.JSInterop;
using static Celin.XL.CSharp.AppState;

namespace Celin.XL.CSharp;

public class JsService
{
    readonly string LIB = "lib";
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    #region app
    enum app
    {
        init,
        initCommandPrompt
    }
    string App(app f) => $"{LIB}.{nameof(app)}.{f:g}";
    public ValueTask Init()
        => JS.InvokeVoidAsync(App(app.init), Ref);
    public ValueTask InitCommandPrompt(string id)
        => JS.InvokeVoidAsync(App(app.initCommandPrompt), id);
    #endregion
    #region invokables
    [JSInvokable]
    public void PromptCommand(string prompt)
        => Mediator.Send(new PromptCommandAction { PromptCommand = prompt });
    #endregion
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

    readonly IJSRuntime JS;
    readonly IMediator Mediator;
    readonly DotNetObjectReference<JsService> Ref;
    readonly IStore Store;
    AppState State => Store.GetState<AppState>();
    public JsService(IJSRuntime js, IMediator mediator, IStore store)
    {
        JS = js;
        Mediator = mediator;
        Ref = DotNetObjectReference.Create(this);
        Store = store;
    }
}
