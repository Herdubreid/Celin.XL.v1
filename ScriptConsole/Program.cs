global using Celin.Language;
global using Celin.Language.XL;
global using System;
global using System.Collections.Generic;
global using System.Linq;
using Celin;
using Celin.AIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

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
/*
var fmt = new SheetProperties(Id: "Testing");
var json = JsonSerializer.Serialize<BaseProperties>(fmt);

var d = JsonSerializer.Deserialize<BaseProperties>(json);

Console.WriteLine(d);
*/
//TestParser.Run(logger);

BaseObject<SheetProperties>.SyncAsyncDelegate = Matrix.Sync;
var e1 = new E1([new E1.Host("e1", new Server("demo.steltix.com/v2/", logger))]);

await Script1.Run(e1);
/*


var orgOut = Console.Out;
TextWriter textWriter = new StringWriter();
Console.SetOut(textWriter);
await TestScript.Run(logger);
Console.SetOut(orgOut);
Console.WriteLine(textWriter.ToString());
*/