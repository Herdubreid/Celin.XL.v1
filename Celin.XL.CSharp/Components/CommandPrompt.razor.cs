using BlazorState;
using Microsoft.AspNetCore.Components;

namespace Celin.XL.CSharp.Components;

public partial class CommandPrompt : BlazorStateComponent
{
    [Inject]
    JsService JS { get; set; } = null!;
    AppState State => GetState<AppState>();
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InitCommandPrompt("commandPromptId");
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}
