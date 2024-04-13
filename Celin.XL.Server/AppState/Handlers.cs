using BlazorState;
using Celin.AIS;
using MediatR;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Celin;

public partial class AppState
{
    static StringBuilder FormatErrorMsg(IEnumerable<AIS.ErrorWarning> errors)
        => errors.Aggregate(new StringBuilder(), (s, e) => s.AppendLine(e.DESC));
    static string ErrorMsg(AisException ex)
        => ex.ErrorResponse?.errorDetails is null
        ? ex.ErrorResponse?.message ?? ex.Message
        : FormatErrorMsg(ex.ErrorResponse.errorDetails.errors).ToString();
    static readonly Regex ALIAS = new Regex(@"(\w+)_(\w+)");
    static readonly Regex RMTABLE = new Regex(@"\[\w+\]$");
    public class AuthenticateHandler : ActionHandler<AuthenticateAction>
    {
        AppState State => Store.GetState<AppState>();
        readonly IMediator Mediator;
        readonly Query.E1Service E1;
        readonly JsService JS;
        public override async Task<Unit> Handle(AuthenticateAction aAction, CancellationToken aCancellationToken)
        {
            E1.BaseUrl = State.Context!.BaseUrl;
            E1.AuthRequest.username = aAction.Username ?? State.Context.Username;
            E1.AuthRequest.password = aAction.Password ?? State.Context.Password;

            if (E1.AuthRequest.username is null)
            {
                await JS.Login();
                return Unit.Value;
            }

            try
            {
                await E1.AuthenticateAsync();

                State.Context.AuthResponse = E1.AuthResponse;

                State.Context.Username = E1.AuthRequest.username;
                State.Context.Password = E1.AuthRequest.password;
                await JS.Authenticated(State.Context);

                if (State.NextAction is not null)
                {
                    await Mediator.Send(State.NextAction);
                    State.NextAction = null;
                }
            }
            catch (AIS.AisException ex)
            {
                if (aAction.Username is null)
                {
                    await JS.Login();
                }
                else
                {
                    await JS.LoginMsg(ErrorMsg(ex));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await JS.Login(false);
                await JS.NotifyError(ex.Message);
            }

            return Unit.Value;
        }
        public AuthenticateHandler(IStore store, IMediator mediator, Query.E1Service e1, JsService js) : base(store)
        {
            Mediator = mediator;
            E1 = e1;
            JS = js;
        }
    }
    public class SubjectDemoLookupHander : ActionHandler<SubjectDemoLookupAction>
    {
        readonly IMediator Mediator;
        readonly Query.E1Service E1;
        readonly JsService JS;
        AppState State => Store.GetState<AppState>();
        async Task<object> Response(string subject)
        {
            if (string.IsNullOrEmpty(subject))
            {
                return new
                {
                    error = "Subject missing!"
                };
            }
            bool table = subject.ToUpper()[0] == 'F';
            var rq = new AIS.DatabrowserRequest
            {
                dataServiceType = AIS.DatabrowserRequest.BROWSE,
                targetType = table
                    ? DatabrowserRequest.table
                    : DatabrowserRequest.view,
                targetName = subject.ToUpper(),
                formServiceDemo = "TRUE"
            };

            State.LookupTask?.Cancel();
            State.LookupTask = new CancellationTokenSource();

            E1.BaseUrl = State.Context?.BaseUrl;
            E1.AuthResponse = State.Context?.AuthResponse;
            var rsp = await E1.RequestAsync<JsonElement>(rq, State.LookupTask.Token);

            object? title = default;
            object? list = default;
            object? error = default;
            var it = rsp.EnumerateObject();
            while (it.MoveNext())
            {
                if (it.Current.Name.StartsWith("fs_"))
                {
                    var fm = JsonSerializer.Deserialize<Form<FormData<JsonElement>>>(it.Current.Value.ToString());
                    title = fm!.title;
                    list = fm.data.gridData.columns.Select(c =>
                    {
                        var m = ALIAS.Match(c.Key);
                        try
                        {
                            var col = new
                            {
                                value = table
                                ? m.Groups[2].Value.ToLower()
                                : $"{m.Groups[1].Value}.{m.Groups[2].Value}".ToLower(),
                                alias = $"{m.Groups[1].Value}.{m.Groups[2].Value}",
                                label = table
                                ? $"[{m.Groups[2].Value}] {c.Value}"
                                : $"[{m.Groups[1].Value}.{m.Groups[2].Value}] {RMTABLE.Replace(c.Value, string.Empty)}"
                            };
                            return col;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }

                        return new { value = c.Key, alias = c.Key, label = c.Value };
                    });
                }
                if (it.Current.Name.Equals("sysErrors"))
                {
                    var errs = JsonSerializer.Deserialize<IEnumerable<AIS.ErrorWarning>>(it.Current.Value.ToString());
                    error = FormatErrorMsg(errs!).ToString();
                }
            }
            return new
            {
                list,
                error
            };
        }
        public override async Task<Unit> Handle(SubjectDemoLookupAction aAction, CancellationToken aCancellationToken)
        {
            object? rs = null;
            try
            {
                rs = await Response(aAction.Subject!);
            }
            catch (AisException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden ||
                    ex.HttpStatusCode == (System.Net.HttpStatusCode)444)
                {
                    State.NextAction = aAction;
                    await Mediator.Send(new AuthenticateAction());
                }
                else
                {
                    rs = new
                    {
                        error = ErrorMsg(ex)
                    };
                }
            }
            catch (Exception ex)
            {
                rs = new
                {
                    error = ex.Message
                };
            }

            await JS.SubjectDemoLookupResponse(rs!);

            return Unit.Value;
        }
        public SubjectDemoLookupHander(IStore aStore, IMediator mediator, JsService js, Query.E1Service e1) : base(aStore)
        {
            Mediator = mediator;
            JS = js;
            E1 = e1;
        }
    }
    public class SubjectLookupHandler : ActionHandler<SubjectLookupAction>
    {
        readonly IMediator Mediator;
        readonly Query.E1Service E1;
        readonly JsService JS;
        AppState State => Store.GetState<AppState>();
        public override async Task<Unit> Handle(SubjectLookupAction aAction, CancellationToken aCancellationToken)
        {
            if (State.IsAuthenticated)
            {
                try
                {
                    State.LookupTask?.Cancel();
                    State.LookupTask = new CancellationTokenSource();

                    E1.BaseUrl = State.Context?.BaseUrl;
                    E1.AuthResponse = State.Context?.AuthResponse;
                    var rsp = await E1.RequestAsync<F9860.Response>(new F9860.Request(aAction.Filter!), State.LookupTask.Token);

                    var rs = rsp.fs_DATABROWSE_F9860?.data.gridData.rowset.Select(r
                        => new
                        {
                            value = r.F9860_OBNM?.ToLower(),
                            label = $"{r.F9860_OBNM?.ToLower()} - {r.F9860_MD}"
                        });
                    await JS.SubjectLookupResponse(rs!);
                }
                catch (AIS.AisException ex)
                {
                    if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden ||
                        ex.HttpStatusCode == (System.Net.HttpStatusCode)444)
                    {
                        State.NextAction = aAction;
                        await Mediator.Send(new AuthenticateAction());
                    }
                    else
                    {
                        await JS.NotifyError(ErrorMsg(ex));
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    await JS.NotifyError(ex.Message);
                }
            }
            else
            {
                await JS.Login();
            }

            return Unit.Value;
        }
        public SubjectLookupHandler(IStore aStore, IMediator mediator, JsService js, Query.E1Service e1) : base(aStore)
        {
            Mediator = mediator;
            JS = js;
            E1 = e1;
        }
    }
    public class SubmitQueryHandler : ActionHandler<SubmitQueryAction>
    {
        public override async Task<Unit> Handle(SubmitQueryAction aAction, CancellationToken aCancellationToken)
        {
            if (State.IsAuthenticated)
            {
                E1.BaseUrl = State.Context?.BaseUrl;
                E1.AuthResponse = State.Context?.AuthResponse;

                try
                {
                    var rq = Query.Request.Parse(aAction.Query!.TrimEnd('\n', ' ', ';'));

                    string id = rq.Name ?? "Default";
                    var response = new Query.Response(new Query.ResponseHeader
                    {
                        Id = id,
                        User = E1.AuthRequest.username,
                        Query = aAction.Query,
                        Template = aAction.Template,
                        Submitted = DateTime.Now,
                        Environment = E1.AuthResponse?.environment,
                        Error = string.Empty,
                        Busy = true,
                    },
                        new List<IEnumerable<object?>>()
                    );

                    await JS.UpdateCql(response.Header);

                    try
                    {
                        CancellationTokenSource? cancel;
                        if (!State.QueryCancels.TryGetValue(id, out cancel) || cancel.IsCancellationRequested)
                        {
                            if (cancel != null)
                            {
                                State.QueryCancels.Remove(id);
                            }
                            cancel = new CancellationTokenSource();
                            State.QueryCancels.Add(id, cancel);
                        }

                        var rs = await E1.RequestAsync<JsonElement>(rq.DataRequest, cancel.Token);
                        var dt = Query.ResponseFactory.Parse(response, rq, rs);

                        await JS.SetCqlDetails(response.Header, dt.Rows);
                    }
                    catch (AisException ex)
                    {
                        if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden ||
                            ex.HttpStatusCode == (System.Net.HttpStatusCode)444)
                        {
                            throw;
                        }
                        else
                        {
                            response.Header.Error = ErrorMsg(ex);
                            response.Header.Summary = new Summary(0, false);
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Header.Error = ex.Message;
                        response.Header.Summary = new Summary(0, false);
                    }
                    finally
                    {
                        response.Header.Busy = false;
                        await JS.UpdateCql(response.Header);
                    }
                }
                catch (AisException ex)
                {
                    if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden ||
                        ex.HttpStatusCode == (System.Net.HttpStatusCode)444)
                    {
                        State.NextAction = aAction;
                        await Mediator.Send(new AuthenticateAction());
                    }
                    else
                    {
                        await JS.NotifyError(ErrorMsg(ex));
                    }
                }
                catch (Exception ex)
                {
                    await JS.NotifyError(ex.Message);
                }
            }
            else
            {
                State.NextAction = aAction;
                await Mediator.Send(new AuthenticateAction());
            }

            return Unit.Value;
        }

        AppState State => Store.GetState<AppState>();
        readonly IMediator Mediator;
        readonly Query.E1Service E1;
        readonly JsService JS;
        public SubmitQueryHandler(IStore store, IMediator mediator, Query.E1Service e1, JsService js) : base(store)
        {
            Mediator = mediator;
            E1 = e1;
            JS = js;
        }
    }
    public class CancelQueryHandler : ActionHandler<CancelQueryAction>
    {
        public override Task<Unit> Handle(CancelQueryAction aAction, CancellationToken aCancellationToken)
        {
            CancellationTokenSource? cancel;
            if (State.QueryCancels.TryGetValue(aAction.Id!, out cancel) && !cancel.IsCancellationRequested)
            {
                State.QueryCancels.Remove(aAction.Id!);
                cancel.Cancel();
            }

            return Unit.Task;
        }
        AppState State => Store.GetState<AppState>();
        public CancelQueryHandler(IStore store) : base(store) { }
    }
    public class SubmitScriptHandler : ActionHandler<SubmitScriptAction>
    {
        public override async Task<Unit> Handle(SubmitScriptAction aAction, CancellationToken aCancellationToken)
        {
            if (State.IsAuthenticated)
            {
                E1.BaseUrl = State.Context?.BaseUrl;
                E1.AuthResponse = State.Context?.AuthResponse;
                string id = string.Empty;
                string? error = null;

                try
                {
                    var source = aAction.Script!.Trim('\n', ' ', ';');
                    var rq = Script.Request.Parse(source);

                    id = rq.Name ?? "Default";
                    await JS.updateCsl(new
                    {
                        id,
                        busy = !aAction.ValidateOnly,
                        error = string.Empty,
                        source,
                        template = aAction.Template,
                    });
                    await JS.ClearScriptResponse(id);
                    if (aAction.ValidateOnly)
                    {
                        return Unit.Value;
                    }

                    var frq = rq.FormRequest;

                    CancellationTokenSource? cancel;
                    if (!State.ScriptCancels.TryGetValue(id, out cancel) || cancel.IsCancellationRequested)
                    {
                        if (cancel != null)
                        {
                            State.ScriptCancels.Remove(id);
                        }
                        cancel = new CancellationTokenSource();
                        State.ScriptCancels.Add(id, cancel);
                    }
                    if (frq?.Demo != null)
                    {
                        await JS.SetScriptStatus(new
                        {
                            id,
                            msg = string.Format("{0} Specs", frq.Demo.formName)
                        });
                        var rs = await E1.RequestAsync<JsonElement>(frq.Demo, cancel.Token);
                        var rp = Script.Response.Parse
                            (Pidgin.Maybe.Nothing<IEnumerable<AIS.Form.Output.Type>>(), true, 0, null, rs);
                        await JS.SetScriptResponse(new
                        {
                            id,
                            error = rp.ErrorMsg,
                            msg = string.Format("Specs for {0}", frq.Demo.formName),
                            data = rp.Outputs,
                        });
                    }
                    else
                    {
                        JsonElement rs = default;
                        try
                        {
                            int row = 0;
                            int errors = 0;
                            foreach (var chain in frq!.Chain!)
                            {
                                await JS.SetScriptStatus(new
                                {
                                    id,
                                    row,
                                    of = frq.Chain.Count(),
                                    errors,
                                    msg = chain.open.request.formName
                                });
                                rs = await E1.RequestAsync<JsonElement>(AIS.Form.Make.Open(chain.open.request), cancel.Token);
                                var rp = Script.Response.Parse
                                    (chain.open.outputs, false, row, null, rs);
                                if (rp.ErrorMsg != null) errors++;
                                await JS.SetScriptResponse(new
                                {
                                    id,
                                    error = rp.ErrorMsg,
                                    msg = rp.Msg,
                                    data = rp.Outputs,
                                });
                                if (rp.ErrorMsg != null && frq.BreakOE) break;
                                int action = 0;
                                foreach (var a in chain.execute)
                                {
                                    var ex = AIS.Form.Make.Execute(
                                        JsonSerializer.Deserialize<FormResponse>(rs)!, a.request);
                                    await JS.SetScriptStatus(new
                                    {
                                        id,
                                        msg = ex.actionRequest.formOID
                                    });
                                    rs = await E1.RequestAsync<JsonElement>(ex);
                                    rp = Script.Response.Parse(a.outputs, false, row, ++action, rs);
                                    if (rp.ErrorMsg != null) errors++;
                                    await JS.SetScriptResponse(new
                                    {
                                        id,
                                        error = rp.ErrorMsg,
                                        msg = rp.Msg,
                                        data = rp.Outputs,
                                    });
                                    if (rp.ErrorMsg != null && frq.BreakOE) break;
                                }
                                await E1.RequestAsync<JsonElement>(AIS.Form.Make.Close(
                                                                    JsonSerializer.Deserialize<AIS.FormResponse>(rs)!));
                                row++;
                            }
                            await JS.SetScriptStatus(new
                            {
                                id,
                                row = 0,
                                of = 0,
                                errors,
                                msg = "Done",
                            });
                        }
                        catch (OperationCanceledException)
                        {
                            await JS.SetScriptResponse(new
                            {
                                id,
                                error = "Cancelled!",
                                msg = string.Empty,
                                data = Enumerable.Empty<object>(),
                            });
                        }
                    }
                }
                catch (AisException ex)
                {
                    if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden ||
                        ex.HttpStatusCode == (System.Net.HttpStatusCode)444)
                    {
                        State.NextAction = aAction;
                        await JS.ClearScriptResponse(id);
                        await Mediator.Send(new AuthenticateAction());
                    }
                    else
                    {
                        error = ErrorMsg(ex);
                        await JS.NotifyError(ErrorMsg(ex));
                    }
                }
                catch (Exception ex)
                {
                    error = ex.Message;
                    await JS.NotifyError(ex.Message);
                }
                finally
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        await JS.updateCsl(new
                        {
                            id,
                            error,
                            busy = false,
                        });
                    }
                }
            }
            else
            {
                State.NextAction = aAction;
                await Mediator.Send(new AuthenticateAction());
            }

            return Unit.Value;
        }
        AppState State => Store.GetState<AppState>();
        readonly IMediator Mediator;
        readonly Query.E1Service E1;
        readonly JsService JS;
        public SubmitScriptHandler(IStore store, IMediator mediator, Query.E1Service e1, JsService js) : base(store)
        {
            Mediator = mediator;
            E1 = e1;
            JS = js;
        }
    }
    public class CancelScriptHandler : ActionHandler<CancelScriptAction>
    {

        public override Task<Unit> Handle(CancelScriptAction aAction, CancellationToken aCancellationToken)
        {
            CancellationTokenSource? cancel;
            if (State.ScriptCancels.TryGetValue(aAction.Id!, out cancel) && !cancel.IsCancellationRequested)
            {
                State.ScriptCancels.Remove(aAction.Id!);
                cancel.Cancel();
            }

            return Unit.Task;
        }
        AppState State => Store.GetState<AppState>();
        public CancelScriptHandler(IStore store) : base(store) { }
    }
    public class SelectContextHander : ActionHandler<SelectContextAction>
    {
        AppState State => Store.GetState<AppState>();
        readonly Query.E1Service E1;
        public override Task<Unit> Handle(SelectContextAction aAction, CancellationToken aCancellationToken)
        {
            State.Context = State.Contexts!.First(ctx => ctx.Id == aAction.ContextId);

            E1.AuthRequest.username = State.Context?.AuthResponse?.username;

            return Unit.Task;
        }
        public SelectContextHander(IStore store, Query.E1Service e1) : base(store)
        {
            E1 = e1;
        }
    }
}
