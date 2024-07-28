var x = Range.Cells("b2");
await x.SetValueAsync("[100,120],[10]");
var v = await Range.Cells("b2:c3").GetValueAsync();
Console.WriteLine(v.ToMatrix());
//Console.WriteLine((await x.GetValueAsync()).ToMatrixString());
//XL.Range.Cells("b2").Values = "10";
