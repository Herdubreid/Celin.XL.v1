global using System;
global using System.Collections.Generic;
global using System.Linq;
global using Celin.Language;
global using Celin.Language.XL;
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

var e1 = new Server("demo.steltix.com/v2/", logger);

BaseObject<SheetObject>.SyncFromDelegate = Matrix.Sync;
RangeObject.SetRangeValue = Matrix.Set;
RangeObject.GetRangeValue = Matrix.Get;

//await Script1.Run(e1);

await TestScript.Run(logger);
//TestParser.Run(logger);
