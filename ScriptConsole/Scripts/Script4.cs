
Console.WriteLine("Starting...");

var rq = QL("f0101 (an8,alph)");

var rs = await E1.RequestAsync<F0101>(rq);

var sub = 
    from row in rs.Rows
    where row.f0101_an8 > 1200
    select row;

foreach (var row in sub) {
    Console.WriteLine(row);
}
