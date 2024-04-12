using BlazorState;

namespace Celin;

public partial class AppState : State<AppState>
{
    public CancellationTokenSource? LookupTask { get; set; }
    public bool IsAuthenticated => Context != null && Context?.AuthResponse != null;
    public IEnumerable<Query.Context>? Contexts { get; set; }
    public Query.Context? Context { get; set; }
    public IAction? NextAction { get; set; }
    public IDictionary<string, CancellationTokenSource> ScriptCancels { get; } = new Dictionary<string, CancellationTokenSource>();
    public IDictionary<string, CancellationTokenSource> QueryCancels { get; } = new Dictionary<string, CancellationTokenSource>();
    public override void Initialize()
    {
        Contexts = _config.GetSection("Connections").Get<IEnumerable<Query.Context>>();
        Context = Contexts!.First();
    }
    readonly IConfiguration _config;
    public AppState(IConfiguration config)
    {
        _config = config;
    }

}
