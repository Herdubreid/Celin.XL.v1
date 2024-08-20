using BlazorState;
using Celin.XL.Sharp.Service;
using Celin.XL.Sharp.Services;
using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Diagnostics;
using static Celin.XL.Sharp.AppState;

namespace Celin.XL.Sharp.Components;

public partial class ScriptComponent : BlazorStateComponent
{
    [Parameter]
    public KeyValuePair<string, Services.Script> Script { get; set; }
    [Inject]
    public WriterService Writer { get; set; } = null!;
    [Inject]
    public SharpService Sharp { get; set; } = null!;
    void EditCode() => Mediator.Send(new EditScriptAction { Key = Script.Key });
    bool _isRunning { get; set; }
    string _runIcon => _isRunning
        ? Icons.Material.Filled.StopCircle
        : Icons.Material.Filled.PlayCircle;
    CancellationTokenSource _cancellation { get; set; } = null!;
    async void Run()
    {
        if (_isRunning)
        {
            if (!_cancellation.IsCancellationRequested)
            {
                Writer.Highlight($"Cancelling...");
                _cancellation.Cancel();
            }
        }
        else
        {
            _isRunning = true;
            StateHasChanged();
            _cancellation = new CancellationTokenSource();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Writer.Highlight($"Start {Script.Value.Title} [{DateTime.Now.TimeOfDay.ToString()}]");
            try
            {
                await Sharp.Submit(Script.Value.Doc, _cancellation.Token);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            finally
            {
                _isRunning = false;
                StateHasChanged();
            }
            stopwatch.Stop();
            Writer.Highlight($"Finised: {stopwatch.Elapsed}");
        }
    }
}
