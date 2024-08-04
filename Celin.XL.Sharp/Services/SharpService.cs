using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.Json;
using Celin.Language.XL;
using Celin.Language;

namespace Celin.XL.Sharp.Services;

public class SharpService
{
    public ScriptState? ScriptState { get; protected set; }
    public async Task Submit(string cmd)
    {
        _logger.LogDebug("Submit: {0}", cmd);
        try
        {
            if (ScriptState == null)
                ScriptState = await CSharpScript.RunAsync(cmd, _scriptOptions, _shell);
            else
                ScriptState = await ScriptState.ContinueWithAsync(cmd);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(Submit));
        }
    }
    async ValueTask<RangeProperties> SyncRange(string key, RangeProperties values)
    {
        var s = await _js.syncRange(key, values);
        var p = JsonSerializer.Deserialize<RangeProperties>(s);
        return p;
    }
    async ValueTask<ValuesProperties<object?>> SyncValues(string key, ValuesProperties<object?> values)
    {
        var s = await _js.syncValues(key, values);
        var p = JsonSerializer.Deserialize<IEnumerable<IEnumerable<object?>>>(s);
        return new ValuesProperties<object?>(xl: p);
    }
    readonly ScriptShell _shell;
    readonly ILogger _logger;
    readonly JsService _js;
    readonly ScriptOptions _scriptOptions;
    public SharpService(ILogger<SharpService> logger, JsService js, E1Service e1)
    {
        _shell = new ScriptShell(e1);
        _logger = logger;
        _js = js;

        BaseObject<ValuesProperties<object?>>.SyncAsyncDelegate = SyncValues;
        BaseObject<RangeProperties>.SyncAsyncDelegate = SyncRange;
        BaseObject<SheetProperties>.SyncAsyncDelegate = _js.syncSheet;

        // Create a scripting environment
        _scriptOptions = ScriptOptions.Default
            .AddImports([
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "Celin.Language",
                "Celin.Language.XL"])
            .AddReferences([
                typeof(Program).Assembly]);
    }
}
