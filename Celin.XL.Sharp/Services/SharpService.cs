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
    async Task SetRangeValueAsync((string? sheet, string? cells, string? name) address, IEnumerable<IEnumerable<object?>> value)
        => await _js.SetRange(address.sheet, address.cells!, value);
    async Task<IEnumerable<IEnumerable<object?>>> GetRangeValueAsync((string? sheet, string? cells, string? name) address)
    {
        var s = await _js.GetRange(address.sheet, address.cells!);
        var e = JsonSerializer.Deserialize<IEnumerable<IEnumerable<object?>>>(s);
        return e!;
    }
    async Task<SheetProperties> SyncFrom(string key, SheetProperties value, bool fromExcel)
    {
        var result = await _js.SyncFromSheet(key);
        return result;
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

        XLObject<SheetProperties>.SyncFromDelegate = _js.SyncFromSheet;
        XLObject<SheetProperties>.SyncToDelegate = _js.SyncToSheet;
        RangeObject.SetRangeValue = SetRangeValueAsync;
        RangeObject.GetRangeValue = GetRangeValueAsync;

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
