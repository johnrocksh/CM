namespace CastManager.Models.Device
{
    using System;
    using CastManager.Logger;
    using System.Net;
    using CastManager.Client;
    using CastManager.Client.Device;
    using Newtonsoft.Json;

    /// <summary>
    /// Contain information about Lookuped Device
    /// </summary>
    public class DeviceData
    {
        public DeviceInfo Info { get; private set; }
        public DeviceConfig Config { get; private set; }

        private Lazy<IDeviceClient> _deviceClient;
        public IDeviceClient Client => _deviceClient.Value;

        public DeviceData(IClientsFactory clientsFactory, DeviceInfo info) 
        {
            Info = info;
            _deviceClient = new Lazy<IDeviceClient>(() => clientsFactory.GetDeviceClient(Info.IPAddress, (ushort)Info.Port));
        }

        /// <summary>
        /// Retrieves configs from the remote device and save them to DeviceConfig
        /// </summary>
        /// <exception cref="Exception">Can`t receive config from device</exception>
        public async void DownloadConfig()
        {
            var config = await Client.GetConfigAsync().ConfigureAwait(false);
            if (config == null)
            {
                throw new Exception($"Can`t receive config from device Id={Info.Id}");
            }
            Config = JsonConvert.DeserializeObject<DeviceConfig>(config);
            if(Config == null)
            {
                Config = new DeviceConfig();
            }
        }

        /// <summary>
        /// Upload config to device
        /// </summary>
        public async Task UploadConfigAsync()
        {
            var config = JsonConvert.SerializeObject(Config);
            await Client.SetConfigAsync(config).ConfigureAwait(false);
        }

        public bool IsEqual(DeviceData deviceData)
        {
            return this.Info.Id == deviceData.Info.Id || this.Info.IPAddress == deviceData.Info.IPAddress;
        }
    }
}
