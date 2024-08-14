using BlazorState;
using System.Text.Json;

namespace Celin.XL.Sharp.Services;

public record Script(string Title, string Description, string Content);
public class ScriptService
{
    AppState State => _store.GetState<AppState>();
    public void Refresh()
    {
        var list = ListFiles.Select(file =>
        {
            using var r = Open(file);
            return (file, JsonSerializer.Deserialize<Script>(r.ReadToEnd()));
        }) ?? Enumerable.Empty<(string, Script?)>();
        State.Scripts = list.ToDictionary(e => e.file, e => e.Item2!);
    }
    public StreamReader Open(string fname) => File.OpenText(Path.Combine(_path, fname));
    public IEnumerable<string> ListFiles => Directory.EnumerateFiles(_path);
    readonly string _path;
    readonly IStore _store;
    const string KEY = "ScriptPath";
    public ScriptService(IConfiguration config, ILogger<ScriptService> logger, IStore store)
    {
        _store = store;
        _path = config.GetValue<string>(KEY) ?? throw new MissingFieldException($"'{KEY}' is missing in appSettings.json");
    }
}
