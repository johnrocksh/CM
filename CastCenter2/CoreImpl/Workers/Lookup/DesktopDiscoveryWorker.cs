namespace CastManager.CoreImpl.Lookup
{
    using System;

    using CastManager.Models.Desktops;
    using CastManager.Worker;
    using CastManager.CoreImpl.Lookup.Addresses;
    using CastManager.Client;

    /// <summary>
    /// Worker to discovering devices, all time when it running. 
    /// When an device will be dicovered - notify about that by invoke the action OnDeviceDiscovered.
    /// </summary>
    internal class DesktopDiscoveryWorker : LookupWorker
    {
        /// <summary>
        /// Device discovered result, the callback.
        /// </summary>
        public Action<DesktopInfo> OnDesktopDiscovered { get; set; }

        /// <summary>
        /// Create instance to discovery devices
        /// </summary>
        public DesktopDiscoveryWorker(IClientsFactory clientsFactory, IAddressesLoop addressesLoop)
        : base(clientsFactory, addressesLoop)
        {
            base.OnMakeEndpoint = ip => $"http://{ip}/Te/config/";
            base.OnDiscovered = OnDiscovered;
        }

        /// <summary>
        /// The callback when service discovered
        /// In this method, we directly get DesktopInfo by a specific ip
        /// </summary>
        void OnDiscovered(string ipAddress, string result)
        {
            OnDesktopDiscovered?.Invoke(new DesktopInfo(clientsFactory, ipAddress, result));
        }
    }
}
