using Celin.XL.Sharp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Celin.XL.Sharp.Components;

public partial class InteractiveComponent
{
    [Inject]
    JsService JS { get; set; } = null!;
    [Inject]
    SharpService _sharp { get; set; } = null!;
    MudBlazor.MudTextField<string> _input = null!;
    string? _error;
    string? _command;
    bool _hasError => !string.IsNullOrEmpty(_error);
    async void _handleKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter" && args.ShiftKey && !string.IsNullOrEmpty(_command))
        {
            _error = null;
            try
            {
                await _sharp.Submit(_command);
                _input?.Clear();
            }
            catch (Exception ex)
            {
                _error = ex.Message;
                StateHasChanged();
            }
        }
    }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InitCommandPrompt("commandPromptId");
            }
            await base.OnAfterRenderAsync(firstRender);
        }
}
