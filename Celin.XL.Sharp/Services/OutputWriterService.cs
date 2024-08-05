using BlazorState;
using System.Text;
using static System.Formats.Asn1.AsnWriter;

namespace Celin.XL.Sharp.Service;

public class OutputWriterService : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;
    public override void Write(char value)
        => State.Output.Append(value);
    public override void Write(string? value)
        => State.Output.Append(value);
    public override void WriteLine(string? value)
        => State.Output.AppendLine(value);
    AppState State => _store.GetState<AppState>();
    readonly IStore _store;
    public OutputWriterService(IStore store)
    {
        _store = store;
    }
}
