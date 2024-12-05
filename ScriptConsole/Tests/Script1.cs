namespace Celin;
class Script1 : ScriptShell
{
    public async Task Run()
    {
        Console.WriteLine("Starting...");
        var range = Range("b2");
        var v1 = "[100,120],[10]".ToMatrix();
        var v = Range("b2:b3");
        Console.WriteLine($"Result: {v1.ToCsv()}");
    }
    Script1(E1 e1) : base(e1) { }
    public static Task Run(E1 e1)
        => new Script1(e1).Run();
}
