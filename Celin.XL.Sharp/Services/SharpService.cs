using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.Json;
using Celin.Language.XL;

namespace Celin.XL.Sharp.Services;

class Globals
{
    public static RangeObject Range
        => RangeObject.Range;
}
public class SharpService
{
    public async Task Submit(string cmd)
    {
        _logger.LogDebug("Submit: {0}", cmd);
        await CSharpScript.RunAsync(cmd, _scriptOptions);
    }
    async Task SetRangeValueAsync((string? sheet, string? cells, string? name) address, IEnumerable<IEnumerable<object?>> value)
        => await _js.SetRange(address.sheet, address.cells!, value);
    async Task<IEnumerable<IEnumerable<object?>>> GetRangeValueAsync((string? sheet, string? cells, string? name) address)
    {
        var s = await _js.GetRange(address.sheet, address.cells!);
        var e = JsonSerializer.Deserialize<IEnumerable<IEnumerable<object?>>>(s);
        return e!;
    }
    readonly ILogger _logger;
    readonly JsService _js;
    readonly ScriptOptions _scriptOptions;
    public SharpService(ILogger<SharpService> logger, JsService js)
    {
        _logger = logger;
        _js = js;

        Language.XL.RangeObject.SetRangeValue = SetRangeValueAsync;
        Language.XL.RangeObject.GetRangeValue = GetRangeValueAsync;

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
