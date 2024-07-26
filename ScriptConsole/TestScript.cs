using Celin.Language.XL;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace Celin;

class RangeValue
{
    static readonly Dictionary<(string? address, string? cells, string? name), IEnumerable<IEnumerable<object?>>> _values
        = new Dictionary<(string? address, string? cells, string? name), IEnumerable<IEnumerable<object?>>>();
    public static Task Set((string? sheet, string? cells, string? name) address, IEnumerable<IEnumerable<object?>> values)
    {
        _values[address] = values;
        return Task.CompletedTask;
    }
    public static Task<IEnumerable<IEnumerable<object?>>> Get((string? sheet, string? cells, string? name) address)
        => Task.FromResult(_values[address]);
}

static class TestScript
{
    public static async Task Run(ILogger logger)
    {
        XL.RangeObject.SetRangeValue = RangeValue.Set;
        XL.RangeObject.GetRangeValue = RangeValue.Get;

        // E1
        var e1 = new AIS.Server("https://demo.steltix.com/jderest/v2/", logger);
        e1.AuthRequest.username = "DEMO";
        e1.AuthRequest.password = "DEMO";
        //await e1.AuthenticateAsync();

        /*
        var rq = Celin.Language.QL.Parse("f0101 (an8,alph)");
        var rs = await e1.RequestAsync<F0101>(rq);
        // Simplify the return parameter
        var d = rs.fs_DATABROWSE_F0101.data.gridData;
        // Dump the query result
        Console.WriteLine("Returned {0} records, there are {1}more.", d.summary.records, d.summary.moreRecords ? string.Empty : "no ");
        // Note that the 'r' variable is now dynamic

        foreach (dynamic r in d.rowset)
        {
            // Now we can access the members without concrete class!
            Console.WriteLine("{0,8} {1}", r.f0101_an8, r.f0101_alph);
        }*/

        // Define the scripting API
        var globals = new Globals(e1, logger);
        globals.OnProcess += (string? msg) =>
        {
            logger.LogDebug($"Process: {msg}");
        };

        // Create a scripting environment
        var scriptOptions = ScriptOptions.Default
            .AddImports([
                "System",
                "System.Collections.Generic",
                "System.Linq",
                "Celin.Language",
                "Celin.Language.XL"])
            .AddReferences([
                typeof(Globals).Assembly]);

        // Run the Init script
        string sb = File.ReadAllText("../../../Scripts/init.cs");
        var state = await CSharpScript.RunAsync(sb, scriptOptions, globals);

        while (true)
        {
            var ln = Console.ReadLine();
            if (string.IsNullOrEmpty(ln))
                break;
            else
            {
                try
                {
                    sb = File.ReadAllText($"../../../Scripts/{ln}.cs");

                    // Run the user script
                    logger.LogInformation("Run...");
                    state = await state.ContinueWithAsync(sb);
                    //var sc = CSharpScript.Create(sb, scriptOptions, typeof(Globals));
                    //await sc.RunAsync(globals);
                    //logger.LogInformation($"Message: {globals.Message}");
                    //await CSharpScript.RunAsync(userScript, scriptOptions, globals);
                }
                catch (CompilationErrorException ex)
                {
                    logger.LogError(ex, nameof(CompilationErrorException));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, nameof(Exception));
                }
            }
        }

        _ = e1.LogoutAsync();

        /*
        record FormResult : Celin.AIS.Form<Celin.AIS.FormData<Celin.AIS.DynamicJsonElement>>;
        record F0101 : Celin.AIS.FormResponse
        {
            // Instead of defining row members, we use the DynamicJsonElment
            public FormResult fs_DATABROWSE_F0101 { get; set; } = null!;
        };*/
    }
}
