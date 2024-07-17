Variables["test"] = 100;
LogDebug(Variables["test"].ToString());

Variables["matrix"] = Range("[1,2,3],[4,4,6]");
var m = Variables["matrix"] as IEnumerable<IEnumerable<object>>;
LogDebug(m.ToMatrixString());
