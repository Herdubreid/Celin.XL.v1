var sh = Sheet("Sheet Numer 2");
await sh.Sync();
Console.WriteLine($"Sheet: {sh.Key}");
sh.Name = "Test2";
await sh.Sync();
Console.WriteLine($"Sheet: {sh.Name}");

var x = Range("c1:h9");
await x.Sync();
Console.WriteLine(x.Properties);

var x = Range("d5:g12");
await x.Sync();
var v = x.Values;
Console.WriteLine(v[0,0].ToString());

var x = Range("d5:g12");
await x.Sync();
var v = x.Values;
v[1,1] = 20;
v[1,2] = "Hello";
await v.Sync();
Console.WriteLine(v.Properties);