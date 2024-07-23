using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Celin.XL.Sharp.Services;

class Globals { }
public class SharpService
{
    public async Task Submit(string cmd)
    {
        _logger.LogDebug("Submit: {0}", cmd);
        await CSharpScript.RunAsync(cmd, _scriptOptions);
    }
    void Set((string? sheet, string? cells, string? name) address, object?[,] value)
    {
        _js.SetRange(address.sheet, address.cells!, value);
    }
    object?[,] Get((string? sheet, string? cells, string? name) address)
    {
        var res = _js.GetRange(address.sheet, address.cells!);
        if (res.IsCompleted)
        {
            return res.Result;
        }
        return res.AsTask().Result;
    }
    readonly ILogger _logger;
    readonly JsService _js;
    readonly ScriptOptions _scriptOptions;
    public SharpService(ILogger<SharpService> logger, JsService js)
    {
        _logger = logger;
        _js = js;

        Language.XL.XL.RangeObject.SetRangeValue = Set;
        Language.XL.XL.RangeObject.GetRangeValue = Get;

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
