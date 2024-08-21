using Celin.XL.Sharp.Service;
using Celin.XL.Sharp.Services;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Celin.XL.Sharp.Components;

public partial class ScriptPanelComponent
{
    [Inject]
    public ScriptService Scripts { get; set; } = null!;
    [Inject]
    public SharpService Sharp { get; set; } = null!;
    [Inject]
    public JsService JS { get; set; } = null!;
    [Inject]
    public WriterService Writer { get; set; } = null!;
    void EditCode(KeyValuePair<string, Services.Script> script)
    {
        JS.Editor(script.Key, script.Value.Title!, script.Value.Doc!);
    }
    void CancelRun(Services.Script script)
    {
        script.Cancellation?.Cancel();
        StateHasChanged();
    }
    async void RunCode(Services.Script script)
    {
        script.IsRunning = true;
        script.ErrorMessage = null;
        StateHasChanged();
        script.Cancellation = new CancellationTokenSource();
        script.Stopwatch = new System.Diagnostics.Stopwatch();
        script.Stopwatch.Start();
        Writer.Highlight($"Start {script.Title} [{DateTime.Now.TimeOfDay.ToString()}]");
        try
        {
            await Sharp.Submit(script.Doc!, script.Cancellation.Token);
            script.Stopwatch.Stop();
            Writer.Highlight($"Finised {script.Title} [{script.Stopwatch.Elapsed}]");
        }
        catch (Exception ex)
        {
            script.ErrorMessage = ex.Message;
            StateHasChanged();
        }
        finally
        {
            script.IsRunning = false;
            StateHasChanged();
        }

    }
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Scripts.OnChange += StateHasChanged;
    }
}
