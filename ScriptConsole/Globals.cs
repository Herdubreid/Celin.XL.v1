using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Celin;

public delegate void ProcessEvent(string? msg);
public class Globals
{
    public event ProcessEvent OnProcess = null!;
    public void Process(string? msg)
        => OnProcess?.Invoke(msg);
    public string? Message {  get; set; }
    public AIS.Server E1 { get; }
    public Globals(ILogger logger)
    {
        E1 = new AIS.Server("https://demo.steltix.com/", logger);
    }
}
