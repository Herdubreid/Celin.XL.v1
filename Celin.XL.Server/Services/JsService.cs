
using BlazorState;
using MediatR;
using Microsoft.JSInterop;

namespace Celin;
public class JsService
{
    #region app
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    enum app
    {
        init,
        setServers,
        login,
        authenticated,
        updateCql,
        deleteCql,
        setCqlDetails,
        updateCsl,
        setScriptResponse,
        clearScriptResponse,
        setScriptStatus,
    }
    string EditorApp(app f) => $"{LIB}.{nameof(app)}.{f:g}";
    public ValueTask InitEditorApp(string? version)
        => JS.InvokeVoidAsync(EditorApp(app.init), Ref, version);
    public ValueTask SetServers(IEnumerable<Query.Context> ctxs)
        => JS.InvokeVoidAsync(EditorApp(app.setServers), ctxs);
    public ValueTask Login(bool flag = true)
        => JS.InvokeVoidAsync(EditorApp(app.login), flag);
    public ValueTask Authenticated(Query.Context ctx)
        => JS.InvokeVoidAsync(EditorApp(app.authenticated), ctx);
    public ValueTask UpdateCql(object data)
        => JS.InvokeVoidAsync(EditorApp(app.updateCql), data);
    public ValueTask SetCqlDetails(object data, object results)
        => JS.InvokeVoidAsync(EditorApp(app.setCqlDetails), data, results);
    public ValueTask updateCsl(object data)
        => JS.InvokeVoidAsync(EditorApp(app.updateCsl), data);
    public ValueTask SetScriptResponse(object response)
        => JS.InvokeVoidAsync(EditorApp(app.setScriptResponse), response);
    public ValueTask ClearScriptResponse(string id)
        => JS.InvokeVoidAsync(EditorApp(app.clearScriptResponse), id);
    public ValueTask SetScriptStatus(object? status)
        => JS.InvokeVoidAsync(EditorApp(app.setScriptStatus), status);
    #endregion
    #region lookups
    enum lookups
    {
        subjectResponse,
        subjectDemoResponse
    }
    string Lookups(lookups f) => $"{LIB}.{nameof(lookups)}.{f:g}";
    [JSInvokable]
    public void SubjectLookupRequest(string filterText)
        => Mediator.Send(new AppState.SubjectLookupAction { Filter = filterText });
    public ValueTask SubjectLookupResponse(IEnumerable<object> response)
        => JS.InvokeVoidAsync(Lookups(lookups.subjectResponse), response);
    [JSInvokable]
    public void SubjectDemoLookupRequest(string subject)
        => Mediator.Send(new AppState.SubjectDemoLookupAction { Subject = subject });
    public ValueTask SubjectDemoLookupResponse(object response)
        => JS.InvokeVoidAsync(Lookups(lookups.subjectDemoResponse), response);
    #endregion
    #region excel
    enum excel
    {
        paste,
        insert
    }
    string Excel(excel f) => $"{LIB}.{nameof(excel)}.{f.ToString("g")}";
    public async Task Paste(Query.Response response)
        => await JS.InvokeVoidAsync(Excel(excel.paste), response);
    public async Task Insert(Query.Response response)
        => await JS.InvokeVoidAsync(Excel(excel.insert), response);
    #endregion
    #region utils
    enum utils
    {
        notifyError,
        loginMsg,
    }
    string Utils(utils f) => $"{LIB}.{nameof(utils)}.{f.ToString("g")}";
    public ValueTask NotifyError(string detail, string title = "Error", int timeout = 10000)
        => JS.InvokeVoidAsync(Utils(utils.notifyError), title, detail, timeout);
    public ValueTask LoginMsg(string msg)
        => JS.InvokeVoidAsync(Utils(utils.loginMsg), msg);
    #endregion
    #region invokables
    [JSInvokable]
    public void SelectContext(int contextId)
    {
        Mediator.Send(new AppState.SelectContextAction { ContextId = contextId });
    }
    [JSInvokable]
    public void Authenticate(string username, string password)
        => Mediator.Send(new AppState.AuthenticateAction { Username = username, Password = password });
    [JSInvokable]
    public void SubmitCql(string query, string? template = null)
        => Mediator.Send(new AppState.SubmitQueryAction { Query = query, Template = template });
    [JSInvokable]
    public void SubmitCsl(string script, string? template = null, bool validateOnly = false)
        => Mediator.Send(new AppState.SubmitScriptAction { Script = script, Template = template, ValidateOnly = validateOnly });
    [JSInvokable]
    public void CancelScript(string id)
        => Mediator.Send(new AppState.CancelScriptAction { Id = id });
    [JSInvokable]
    public void CancelQuery(string id)
        => Mediator.Send(new AppState.CancelQueryAction { Id = id });
    [JSInvokable]
    public void ClearScriptOutput()
        => Mediator.Send(new AppState.ClearScriptOutputAction());
    #endregion
    readonly IJSRuntime JS;
    readonly IMediator Mediator;
    readonly DotNetObjectReference<JsService> Ref;
    readonly IStore Store;
    AppState State => Store.GetState<AppState>();
    readonly string LIB = "lib";
    public JsService(IJSRuntime js, IMediator mediator, IStore store)
    {
        JS = js;
        Mediator = mediator;
        Ref = DotNetObjectReference.Create(this);
        Store = store;
    }
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
}
