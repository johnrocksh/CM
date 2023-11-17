namespace CastManager.CoreImpl
{
    using CastManager.Core;
    using CastManager.Models.Desktops;
    using CastManager.Models.Device;

    /// <summary>
    /// Monitoring all devices and desctops to check if they alive or not,
    /// notify as callback Actions that device/desktop was not found
    /// notify that Desktop status was changed (active or standby)
    /// </summary>
    public class MonitorService : IMonitorService
    {
        public Func<IList<DesktopInfo>> Desktops { get; set; }
        public Func<IList<DeviceData>> Devices { get; set; }
        public Action<DesktopInfo> OnDesktopChanged { get; set; }
        public Action<DesktopInfo> OnDesktopLost { get; set; }
        public Action<DeviceData> OnDeviceLost { get; set; }

        private PingDesktopsService pingDesktopsService;
        private PingDevicesService pingDeviceService;

        public void Start()
        {
            pingDesktopsService = new PingDesktopsService()
            {
                Desktops = Desktops,
                OnDesktopUpdated = OnDesktopChanged,
                OnDesktopNotFound = OnDesktopLost,
            };
            pingDesktopsService.Start();

            pingDeviceService = new PingDevicesService()
            {
                Devices = Devices,
                OnDeviceLost = OnDeviceLost,
            };
            pingDeviceService.Start();
        }

        public void Stop()
        {
            pingDeviceService.Stop();
            pingDesktopsService.Stop();
        }
    }
}
