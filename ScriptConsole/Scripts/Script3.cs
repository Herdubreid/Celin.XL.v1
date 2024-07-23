/*
@Sheet1!b5:b7 = 
    10,20,30;
    Report, Test
*/
var ce = XL.Range.Cells("a1");
Console.WriteLine(ce.ToString());
ce.Resize(2, 3);
Console.WriteLine(ce.ToString());

var c = XL.Range.Sheet("Sheet1S").Cells("b5:d6");
c.Values = Values("[1,2,3],[4,4,6]");
Variables["test"] = c.Values;

var m = (object?[,])Variables["test"];
Console.WriteLine(m.ToMatrixString());


XL.Range.Cells("b7:b5").Values = Values("12,20,30");
Console.WriteLine(XL.Range.Cells("b7:b5").Values);

var at1 = "C";

//var rs = : f0101 (an8,alph) all(at1 = $at1)
