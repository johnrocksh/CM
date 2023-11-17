namespace CastManager.Models
{
    using CastManager.Models.Device;

    /// <summary>
    /// Picaseter is the device that create access wifi points, we searching for all access points.
    /// Next to patch the picaster device we should connet to wifi access point and send http POST request to it.
    /// After success the device will use the wifi settings that we sent to device and after we can see device on orign wifi network.
    /// </summary>
    public interface IPicasterPatcher
    {
        /// <summary>
        /// Is wifi adapter available at system.
        /// </summary>
        bool IsWifiAdapter { get; }

        /// <summary>
        /// Searching all Picaster`s wifi networks in the systmem wifi networks.
        /// </summary>
        /// <returns>A collection of all found Piasters</returns>
        Task<IEnumerable<PicasterWifiItem>> GetPicasterWifiItemsAsync();

        /// <summary>
        /// Connect to other wifi network.
        /// </summary>
        Task<bool> ConnectToPicasterWifiNetworkAsync(string ssid);

        /// <summary>
        /// Polling device at access wifi point endpoint, and lookup if http service is active
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        Task <bool> LookupServiceAvailable(TimeSpan timeout);

        /// <summary>
        /// Send network name and password to device, that device should be reconnected to this wifi nettowork.
        /// </summary>
        Task<bool> WriteWifiSettingsToDeviceAsync(string essid, string password);
    }
}
