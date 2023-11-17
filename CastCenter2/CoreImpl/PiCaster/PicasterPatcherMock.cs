namespace CastManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class PicasterPatcherMock : IPicasterPatcher
    {
        public bool IsWifiAdapter => true;

        private readonly int timeout = 1000;

        public async Task<IEnumerable<PicasterWifiItem>> GetPicasterWifiItemsAsync()
        {
            await Task.Delay(timeout).ConfigureAwait(false);

            var networksMock = new List<string>()
            {
                "Netinfo", "PiCaster_10K-Mock", "PiCaster_4RT-Mock"
            };

            return networksMock
                .Where(i => i.StartsWith("Picaster_", StringComparison.OrdinalIgnoreCase))
                .Select(ssid => (new PicasterWifiItem()
                {
                    Ssid = ssid
                }));
        }
      
        public async Task<bool> ConnectToPicasterWifiNetworkAsync(string Ssid)
        {
            await Task.Delay(timeout).ConfigureAwait(false);
            return true;
        }

        async Task<bool> IPicasterPatcher.LookupServiceAvailable(TimeSpan timeout)
        {
            await Task.Delay(timeout);
            return true;
        }

        public async Task<bool> WriteWifiSettingsToDeviceAsync(string essid, string password)
        {
            await Task.Delay(timeout).ConfigureAwait(false);
            return password == "1";
        }
    }
}
