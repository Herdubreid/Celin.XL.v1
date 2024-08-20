﻿using Celin.Language;
using Celin.Language.XL;
using Celin.XL.Sharp.Service;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Text;

namespace Celin.XL.Sharp.Services;

public class SharpService
{
    public ScriptState? ScriptState { get; protected set; }
    public async Task Submit(string cmd, CancellationToken cancel = default)
    {
        _shell.Cancel = cancel;
        if (ScriptState == null)
            ScriptState = await CSharpScript.RunAsync(cmd, _scriptOptions, _shell, typeof(ScriptShell), cancel);
        else
            ScriptState = await ScriptState.ContinueWithAsync(cmd, _scriptOptions, cancel);
    }
    public void Validate(string cmd)
    {
        var script = CSharpScript.Create(cmd, _scriptOptions, typeof(ScriptShell));
        var diag = script.Compile();
        if (diag.Any(d => d.Severity == DiagnosticSeverity.Error))
        {
            var s = diag.Aggregate(new StringBuilder(), (c, n) => c.AppendLine(n.ToString()));
            throw new Exception(s.ToString());
        }
    }
    readonly ScriptShell _shell;
    readonly JsService _js;
    readonly ScriptOptions _scriptOptions;
    public SharpService(OutputWriterService ow, ErrorWriterService ew, JsService js, E1Services e1)
    {
        _shell = new ScriptShell(e1);
        _js = js;
        Console.SetOut(ow);
        Console.SetError(ew);

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
