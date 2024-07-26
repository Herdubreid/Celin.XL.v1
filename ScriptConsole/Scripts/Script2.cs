
var x = XL.Range.Cells("b2:c3");
await x.SetValueAsync("[100,120],[10]");
Console.WriteLine((await XL.Range.Cells("b2").GetValueAsync()).ToMatrixString());
//Console.WriteLine((await x.GetValueAsync()).ToMatrixString());
//XL.Range.Cells("b2").Values = "10";
