using Celin;
using System;
using System.Text.Json;
using System.Threading.Tasks;

record F0101 : Celin.AIS.FormResponse
{
    // Instead of defining row members, we use the DynamicJsonElment
    //public DynamicFormResult fs_DATABROWSE_F0101 { get; set; } = null!;
    public ObjectFormResult fs_DATABROWSE_F0101 { get; set; } = null!;
}

LogInformation("Starting...");

var rq = QL("f0101 (an8,alph)");

var rs = await E1.RequestAsync<F0101>(rq);
// Simplify the return parameter
var d = rs.fs_DATABROWSE_F0101.data.gridData;
// Dump the query result
Console.WriteLine("Returned {0} records, there are {1}more.", d.summary.records, d.summary.moreRecords ? string.Empty : "no ");
// Note that the 'r' variable is now dynamic
foreach (dynamic r in d.rowset)
{
    // Now we can access the members without concrete class!
    Console.WriteLine("{0,8} {1}", r[0], r[1]);
}

//LogDebug(JsonSerializer.Serialize(rq, new JsonSerializerOptions { WriteIndented = true }));
//LogInformation("Waiting...");
//await Task.Delay(5000);
LogInformation("Done");

Message = "Done!";
