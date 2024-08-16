using BlazorState;
using Celin.XL.Sharp;
using Celin.XL.Sharp.Services;
using Microsoft.AspNetCore.Components;
using static Celin.XL.Sharp.AppState;

namespace Celin.XL.Sharp.Components.Pages;

public partial class Index : BlazorStateComponent
{
    void EditScript(string key) => Mediator.Send(new EditScriptAction {  Key = key });
    void RunScript(string key) => Mediator.Send(new RunScriptAction { Key = key });
    [Inject]
    public ScriptService ScriptService { get; set; } = null!;
    AppState State => GetState<AppState>();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ScriptService.Refresh();
    }
}
