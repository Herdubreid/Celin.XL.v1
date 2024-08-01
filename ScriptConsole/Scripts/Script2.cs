var sh = Sheet("Sheet Numer 2");
await sh.GetAsync();
Console.WriteLine($"Sheet: {sh.Key}");
sh.Name = "Test2";
await sh.SetAsync();
Console.WriteLine($"Sheet: {sh.Name}");

var x = Range("c1:h9");
await x.GetAsync();
Console.WriteLine(x.Properties);
