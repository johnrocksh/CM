namespace CastManager.CoreImpl.Lookup
{
    using System;
    using Newtonsoft.Json;

    using CastManager.Models.Device;
    using CastManager.Worker;
    using CastManager.CoreImpl.Lookup.Addresses;
    using CastManager.Client;

    /// <summary>
    /// Worker to discovering devices, all time when it running. 
    /// When an device will be dicovered - notify about that by invoke the action OnDeviceDiscovered.
    /// </summary>
    internal class DevicesDiscoveryWorker : LookupWorker
    {
        /// <summary>
        /// Device discovered result, the callback.
        /// </summary>
        public Action<DeviceData> OnDeviceDiscovered { get; set; }

        /// <summary>
        /// Create instance to discovery devices
        /// </summary>
        /// <param name="totalWorkers">Count of job workers.</param>
        public DevicesDiscoveryWorker(IClientsFactory clientsFactory, IAddressesLoop addressesLoop, ushort port = 8080)
        : base(clientsFactory, addressesLoop)
        {
            base.Port = port;
            base.OnMakeEndpoint = ip => $"http://{ip}:{port}/info";
            base.OnDiscovered = OnDiscovered;
        }

        /// <summary>
        /// The callback when service discovered
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="result"></param>
        void OnDiscovered(string ipAddress, string result)
        {
            var deviceId = JsonConvert.DeserializeObject<DeviceId>(result);

            if (deviceId != null)
            {
                var deviceInfo = new DeviceInfo()
                {
                    Port = Port,
                    Id = deviceId?.Id ?? "NA",
                    Path = "NA",
                    IPAddress = ipAddress,
                };

                var deviceData = new DeviceData(clientsFactory, deviceInfo);
                OnDeviceDiscovered?.Invoke(deviceData);
            }
        }
    }
}
