namespace CastManager.Models
{
    using System;
    using System.Collections.Generic;
    using CastManager.Logger;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    using SimpleWifi;
    using SimpleWifi.Win32;
    using CastManager.Client.Device;
    using System.Diagnostics;

    internal class PicasterSimpleWifi : IPicasterPatcher
    {

        [DataContract]
        public class ConnectData
        {
            [DataMember(Name = "essid")]
            public string Essid { get; set; }

            [DataMember(Name = "password")]
            public string Password { get; set; }
        }

        private Wifi wifi = new Wifi();

        Dictionary<string, AccessPoint> accessPoints = new();

        public bool IsWifiAdapter => wifi.NoWifiAvailable == false;

        public PicasterSimpleWifi()
        {
            if (IsWifiAdapter)
            {
                Task.Run(
                    () =>
                    {
                        var wlanClient = new WlanClient();
                      foreach (WlanInterface wlanIface in wlanClient.Interfaces)
                        {
                            wlanIface.Scan();
                        }
                    });
            }
        }

        public async Task<bool> ConnectToPicasterWifiNetworkAsync(string ssid)
        {
            if (accessPoints.TryGetValue(ssid, out var ap))
            {
                var authRequest = new AuthRequest(ap)
                {
                    Password = "12345678"
                };
                return await Task.Run(() => ap.Connect(authRequest));
            }
            return false;
        }

        public async Task<IEnumerable<PicasterWifiItem>> GetPicasterWifiItemsAsync()
        {
            await this.doRefreshWifiListAsync();
            return this.accessPoints.Select(x => new PicasterWifiItem()
            {
                Ssid = x.Key
            });
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

        public async Task<bool> WriteWifiSettingsToDeviceAsync(string essid, string password)
        {
            try
            {
                var deviceClient = new DeviceClient(address: "10.0.0.1", port: 8080);
                var rqData = new WifiConnectData
                {
                    Essid = essid,
                    Password = password
                };
                return await deviceClient.ConnectToWifiNetwork(rqData);
            }
            catch (Exception e)
            {
                Logger.WriteException(e.Message);
            }
            return false;
        }

        private async Task doRefreshWifiListAsync()
        {
            var list = await Task.Run(() =>
            {
                Task.Delay(500);
                return wifi.GetAccessPoints();
            });

            this.accessPoints = list
                    .Where(x => x.Name.Contains("picaster", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(x => x.Name);
        }
    }
}
