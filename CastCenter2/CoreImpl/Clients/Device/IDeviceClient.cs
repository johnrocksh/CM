namespace CastManager.Client.Device
{
    using System.Threading.Tasks;

    public interface IDeviceClient
    {
        Task<string> GetVersionAsync();

        Task<string> GetInfoAsync();

        Task<string> GetConfigAsync();

        Task SetConfigAsync(string config);

        Task WifiAccessPointMode(AccessPointConfig accessPointConfig);

        bool IsAlive();
    }
}
