
@Sheet1!b5:b7 = 
    10,20,30;
    Report, Test
var ad = new AddressType("Sheet1","b5:b7");
XL.Range(ad).Values = Values("[1,2,3],[4,4,6]");
Variables["test"] = XL.Range(ad).Values;

var m = (object?[,])Variables["test"];
Console.WriteLine(m.ToMatrixString());


XL.Range("b7:b5").Values = Values("12,20,30");
Console.WriteLine(XL.Range("b7:b5").Values);

var at1 = @b4

var rs = :cq f0101 (an8,alph) all(at1 = $at1)
