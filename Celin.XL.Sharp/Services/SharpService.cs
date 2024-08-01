using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.Json;
using Celin.Language.XL;
using Celin.Language;

namespace Celin.XL.Sharp.Services;

public class SharpService
{
    public async Task Submit(string cmd)
    {
        _logger.LogDebug("Submit: {0}", cmd);
        try
        {
            await CSharpScript.RunAsync(cmd, _scriptOptions, _shell);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(Submit));
        }
    }
    async ValueTask<RangeProperties> GetRange(string key)
    {
        var s = await _js.getRange(key);
        var p = JsonSerializer.Deserialize<RangeProperties>(s);
        return p;
    }
    async ValueTask<RangeProperties> SetRange(string key, RangeProperties values)
    {
        string ps = JsonSerializer.Serialize(values);
        var s = await _js.setRange(key, ps);
        var p = JsonSerializer.Deserialize<RangeProperties>(s);
        return p;
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

        BaseObject<RangeProperties>.SetAsyncDelegate = SetRange;
        BaseObject<RangeProperties>.GetAsyncDelegate = GetRange;
        BaseObject<SheetProperties>.SetAsyncDelegate = _js.setSheet;
        BaseObject<SheetProperties>.GetAsyncDelegate = _js.getSheet;

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
