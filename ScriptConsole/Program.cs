global using Celin.Language;
global using Celin.Language.XL;
global using System;
global using System.Collections.Generic;
global using System.Linq;
using Celin;
using Celin.AIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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

TestParser.Run(logger);
/*
var e1 = new Server("demo.steltix.com/v2/", logger);

//BaseObject<SheetObject>.SyncFromDelegate = Matrix.Sync;

//await Script1.Run(e1);

var orgOut = Console.Out;
TextWriter textWriter = new StringWriter();
Console.SetOut(textWriter);
await TestScript.Run(logger);
Console.SetOut(orgOut);
Console.WriteLine(textWriter.ToString());
*/