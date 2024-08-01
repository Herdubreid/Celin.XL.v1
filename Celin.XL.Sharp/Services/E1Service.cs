namespace Celin.XL.Sharp;

public class E1Service : AIS.Server
{
    public E1Service(ILogger<E1Service> logger)
        : base(string.Empty, logger) { }
}
