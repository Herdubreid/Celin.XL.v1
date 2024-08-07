using Celin.Language;
using Pidgin.Configuration;

namespace Celin.XL.Sharp;

public class E1Services : E1
{
    class Config
    {
        public string Name { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
    }
    static readonly string SERVICES = "Services";
    static IReadOnlyCollection<Host> hosts(IConfiguration config, ILogger logger)
    {
        var services = config.GetRequiredSection(SERVICES)
            .Get<List<Config>>()
            ?? throw new InvalidOperationException($"'{SERVICES}' is missing in appSettings.json!");
        return services.Select(s => new Host(
            s.Name,
            new AIS.Server(s.BaseUrl, logger)))
            .ToList();
    }
    public E1Services(IConfiguration config, ILogger<E1Services> logger)
        : base(hosts(config, logger))
    {
        Default = Hosts.FirstOrDefault()
            ?? throw new InvalidOperationException($"No '{SERVICES}' configured in appsettings.json");
    }
}
