namespace Celin;
class Script1 : ScriptShell
{
    public async Task Run()
    {
        Console.WriteLine("Starting...");
        var range = Range.Cells("b2");
        var v1 = "[100,120],[10]".ToMatrix();
        await range.SetValueAsync(v1);
        var v = await Range.Cells("b2:b3").GetValueAsync();
        Console.WriteLine($"Result: {v.ToMatrix()}");
    }
    Script1(AIS.Server e1) : base(e1) { }
    public static Task Run(AIS.Server e1)
        => new Script1(e1).Run();
}
