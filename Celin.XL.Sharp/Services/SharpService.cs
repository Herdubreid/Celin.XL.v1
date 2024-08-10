using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text.Json;
using Celin.Language.XL;
using Celin.Language;
using Celin.XL.Sharp.Service;

namespace Celin.XL.Sharp.Services;

public class SharpService
{
    public ScriptState? ScriptState { get; protected set; }
    public async Task Submit(string cmd)
    {
        if (ScriptState == null)
            ScriptState = await CSharpScript.RunAsync(cmd, _scriptOptions, _shell);
        else
            ScriptState = await ScriptState.ContinueWithAsync(cmd);
    }
    readonly ScriptShell _shell;
    readonly ILogger _logger;
    readonly JsService _js;
    readonly ScriptOptions _scriptOptions;
    public SharpService(ILogger<SharpService> logger, OutputWriterService writer, JsService js, E1Services e1)
    {
        _shell = new ScriptShell(e1);
        _logger = logger;
        _js = js;
        Console.SetOut(writer);

        BaseObject<ValuesProperties<object?>>.SyncAsyncDelegate = _js.syncValues;
        BaseObject<RangeProperties>.SyncAsyncDelegate = _js.syncRange;
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
