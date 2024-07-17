using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Logging;

namespace Celin;

static class TestScript
{
    public static async Task Run(ILogger logger)
    {
        // E1
        var e1 = new Celin.AIS.Server("https://demo.steltix.com/jderest/v2/", logger);
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
                "System.Collections.Generic",
                "Celin.Language"])
            .AddReferences([
                typeof(Globals).Assembly]);

        string sb = string.Empty;

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
                    logger.LogInformation("Create script...");
                    var sc = CSharpScript.Create(sb, scriptOptions, globals.GetType());
                    logger.LogInformation("Run...");
                    await sc.RunAsync(globals);
                    logger.LogInformation($"Message: {globals.Message}");
                    //await CSharpScript.RunAsync(userScript, scriptOptions, globals);
                }
                catch (CompilationErrorException e)
                {
                    Console.WriteLine($"Script compilation error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Script execution error: {e.Message}");
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
