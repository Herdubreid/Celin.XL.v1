using System.Text;

namespace Celin.XL.Sharp.Service;

public class WriterService
{
    public enum WriteTypes
    {
        Highlight,
        Normal,
        Error
    };
    public record Output(WriteTypes Type, string? Text);
    public List<Output> Outputs { get; set; } = new List<Output>();
    public Action? OnChange;
    public void Highlight(string? text)
    {
        Outputs.Add(new Output(WriteTypes.Highlight, text + '\n'));
        NotifyChange();
    }
    public void Normal(string? text)
    {
        Outputs.Add(new Output(WriteTypes.Normal, text));
        NotifyChange();
    }
    public void Error(string? text)
    {
        Outputs.Add(new Output(WriteTypes.Error, text));
        NotifyChange();
    }
    void NotifyChange() => OnChange?.Invoke();
}

public class OutputWriterService(WriterService writer) : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;
    public override void Write(char value) => writer.Normal(value.ToString());
    public override void Write(string? value) => writer.Normal(value);
    public override void WriteLine(string? value) => writer.Normal(value + NewLine);
}
public class ErrorWriterService(WriterService writer) : TextWriter
{
    public override Encoding Encoding => Encoding.UTF8;
    public override void Write(char value) => writer.Error(value.ToString());
    public override void Write(string? value) => writer.Error(value);
    public override void WriteLine(string? value) => writer.Error(value + NewLine);
}
