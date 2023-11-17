namespace CastManager.Core
{
    public interface IAppConfiguration
    {
        string HttpServerPrefix { get; }
        string LocalIp { get; }
        
    }
}
