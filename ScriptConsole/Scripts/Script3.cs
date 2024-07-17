var ad = new AddressType("Sheet1","b5:b7");
XL.Range(ad).Values = Values("[1,2,3],[4,4,6]");
Variables["test"] = XL.Range(ad).Values;

var m = Variables["test"] as IEnumerable<IEnumerable<object>>;
LogDebug(m.ToMatrixString());
