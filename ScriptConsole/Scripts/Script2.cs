Variables["test"] = 100;
LogDebug(Variables["test"].ToString());

Variables["matrix"] = Values("[1,2,3,7,8],[,4,4,6,,,11]");
object?[,] m = Variables["matrix"];
int rows = m.GetLength(0);
int cols = m.GetLength(1);
Console.WriteLine($"{rows},{cols}");
for (int row = 0; row < rows; row++)
{
    for (int col = 0; col < cols; col++)
        Console.Write("{0}, ", m[row,col]);
    Console.WriteLine();
}
