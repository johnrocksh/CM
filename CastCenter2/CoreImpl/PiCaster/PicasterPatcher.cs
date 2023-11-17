namespace CastManager.Models
{
    using CastManager.Logger;
    using Windows.Devices.WiFi;
    using Windows.Security.Credentials;
    using CastManager.Client.Device;
    using Windows.Networking.NetworkOperators;
    using System.Diagnostics;

    /// <inheritdoc cref="IPicasterPatcher"/>
    public class PicasterPatcher : IPicasterPatcher
    {
        /// <summary>
        /// Use this class to enumerate local Wi-Fi adapters, 
        /// initiate Wi-Fi scans, enumerate scan results, and to connect or disconnect individual adapters.
        /// </summary>
        private WiFiAdapter WifiAdapter { get; set; }

        /// <inheritdoc />
        public bool IsWifiAdapter => WifiAdapter != null;

        /// <summary>
        /// Constructor
        /// </summary>
        public PicasterPatcher()
        {
            InitAsync();
        }

        /// <summary>
        /// Init asynchronious
        /// </summary>
        async void InitAsync()
        {
            if (WifiAdapter == null)
            {
                WifiAdapter = await GetWifiAdapterAsync();
            }
        }

        /// <summary>
        /// Get first available wifi adapter
        /// </summary>
        /// <returns>Wifi adapter</returns>
        /// <exception cref="Exception"></exception>
        async Task<WiFiAdapter> GetWifiAdapterAsync()
        {
            var access = await WiFiAdapter.RequestAccessAsync();
            if (access == WiFiAccessStatus.Allowed)
            {
                var selector = WiFiAdapter.GetDeviceSelector();
                var result = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(selector);

                if (result.Count > 0)
                {
                    return await WiFiAdapter.FromIdAsync(result[0].Id);
                }
            }
            else
            {
                throw new Exception("WifiAdapter is dinided, and wiFiControl capability in you app manifest");
            }
            return null;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<PicasterWifiItem>> GetPicasterWifiItemsAsync()
        {
            try
            {
                await WifiAdapter.ScanAsync();
                return WifiAdapter.NetworkReport.AvailableNetworks
                    .Where(net => net.Ssid.StartsWith("Picaster_", StringComparison.OrdinalIgnoreCase))
                    .Select(net => new PicasterWifiItem() { Ssid = net.Ssid });
            }
            catch (Exception e)
            {
                Logger.WriteException(e.Message);
            }
            return null;
        }



        /// <inheritdoc />
        public async Task<bool> ConnectToPicasterWifiNetworkAsync(string Ssid)
        {
            try
            {
                var accessPoint = WifiAdapter.NetworkReport.AvailableNetworks.FirstOrDefault(n => n.Ssid.Contains(Ssid));
                if (accessPoint == null)
                {
                    return false;
                }
                var connectionResult = await WifiAdapter.ConnectAsync(accessPoint, WiFiReconnectionKind.Manual, new PasswordCredential()
                {
                    Password = "12345678"
                });
                return connectionResult.ConnectionStatus == WiFiConnectionStatus.Success;
            }
            catch (Exception e)
            {
                Logger.WriteException(e.Message);
            }
            return false;
        }

        public async Task<bool> LookupServiceAvailable(TimeSpan timeout)
        {
            try
            {
                IDeviceClient deviceClient = new DeviceClient(address: "10.0.0.1", port: 8080);
                var sw = new Stopwatch();
                sw.Start();

                while (sw.Elapsed < timeout)
                {
                    var result = await deviceClient.GetVersionAsync().ConfigureAwait(false);

                    if (result != null)
                    {
                        return true;
                    }
                    await Task.Delay(500);

                }
            }
            catch (Exception e)
            {
                Logger.WriteException(e.Message);
            }
            return false;
        }

        /// <inheritdoc />
        public async Task<bool> WriteWifiSettingsToDeviceAsync(string essid, string password)
        {
            try
            {
                var deviceClient = new DeviceClient(address: "10.0.0.1", port: 8080);
                return await deviceClient.ConnectToWifiNetwork(new WifiConnectData { Essid = essid, Password = password });
            }
            catch (Exception e)
            {
                Logger.WriteException(e.Message);
            }
            return false;
        }

    }
}
