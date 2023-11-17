namespace CastManager.Client.Device
{
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using CastManager.Models.Device;

    internal class DeviceClientMock : IDeviceClient
    {
        string MockId;

        DeviceConfig deviceConfig = new();

        internal DeviceClientMock(string mockId)
        {
            deviceConfig.AddNewDefaultConfig("http://127.0.0.1:9000/notactual_config.png");
            MockId = mockId;
        }

        Task<string> IDeviceClient.GetConfigAsync()
        {
            var config = JsonConvert.SerializeObject(deviceConfig);
            return Task.FromResult(config);
        }

        Task<string> IDeviceClient.GetInfoAsync()
        {
            var info = $"{{\r\n    \"id\": \"Mock_{MockId}\"\r\n}}";
            return Task.FromResult(info);
        }

        Task<string> IDeviceClient.GetVersionAsync()
        {
            var version = $"{{\r\n    \"version\": \"Mock_v1\"\r\n}}";
            return Task.FromResult(version);
        }

        Task IDeviceClient.SetConfigAsync(string config)
        {
            deviceConfig = JsonConvert.DeserializeObject<DeviceConfig>(config);
            return Task.CompletedTask;
        }

        Task IDeviceClient.WifiAccessPointMode(AccessPointConfig accessPointConfig)
        {
            return Task.Delay(2000);
        }

        public bool IsAlive()
        {
            return true;
        }
    }
}
