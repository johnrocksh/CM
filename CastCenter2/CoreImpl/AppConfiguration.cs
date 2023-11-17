namespace CastManager.CoreImpl
{
    using CastManager.Core;

    internal class AppConfiguration : IAppConfiguration
    {
        public string HttpServerPrefix=> "http://+:80/Te/Caster/";

        private Lazy<string> _localIp = new Lazy<string>(() => Helper.Network.GetLocalIPAddress());

        public string LocalIp => _localIp.Value;
    }
}
