using Celin;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

// Logging
var conf = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConfiguration(conf.GetSection("Logging"));
    builder.AddConsole();
});
var logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("Starting");

// Define the scripting API
var globals = new Globals(logger);
globals.OnProcess += (string? msg) =>
{
    logger.LogDebug($"Process: {msg}");
};

// Create a scripting environment
var scriptOptions = ScriptOptions.Default
    .AddImports(["System"])
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
        sb = File.ReadAllText($"Scripts/{ln}.cs");
    }

    // Run the user script
    try
    {
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
