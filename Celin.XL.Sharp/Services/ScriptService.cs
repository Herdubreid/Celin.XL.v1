using Microsoft.CodeAnalysis.Scripting;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Celin.XL.Sharp.Services;

public class Script
{
    public string? Title { get; set; }
    public string? Doc { get; set; }
    public string? Description { get; set; }
    [JsonIgnore]
    public string? ErrorMessage { get; set; }
    [JsonIgnore]
    public Stopwatch? Stopwatch { get; set; }
    [JsonIgnore]
    public bool IsRunning { get; set; }
    [JsonIgnore]
    public CancellationTokenSource? Cancellation { get; set; }
}
public class ScriptService
{
    public Dictionary<string, Services.Script> Scripts { get; set; } = new Dictionary<string, Script>();
    public Action? OnChange;
    public void Refresh()
    {
        var list = ListFiles.Select(file =>
        {
            using var r = Open(file);
            return (file, JsonSerializer.Deserialize<Script>(r.ReadToEnd()));
        }) ?? Enumerable.Empty<(string, Script?)>();
        Scripts = list.ToDictionary(e => e.file, e => e.Item2!);
        NotifyChange();
    }
    public void SaveScript(string fname, Script sc)
    {
        try
        {
            using (StreamWriter sw = Create(fname))
            {
                sw.Write(JsonSerializer.Serialize(sc));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(SaveScript));
        }
    }
    IEnumerable<string> ListFiles => Directory.EnumerateFiles(_path);
    StreamReader Open(string fname) => File.OpenText(Path.Combine(_path, fname));
    StreamWriter Create(string fname) => File.CreateText(Path.Combine(_path, fname));
    void NotifyChange() => OnChange?.Invoke();
    readonly string _path;
    readonly ILogger<ScriptService> _logger;
    const string KEY = "ScriptPath";
    public ScriptService(IConfiguration config, ILogger<ScriptService> logger)
    {
        _path = config.GetValue<string>(KEY) ?? throw new MissingFieldException($"'{KEY}' is missing in appSettings.json");
        _logger = logger;
        Refresh();
    }
}
