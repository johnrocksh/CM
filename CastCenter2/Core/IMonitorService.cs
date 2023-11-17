namespace CastManager.Core
{
    using CastManager.Models.Device;
    using CastManager.Models.Desktops;

    public interface IMonitorService
    {
        Func<IList<DesktopInfo>> Desktops { get; set; }
        Func<IList<DeviceData>> Devices { get; set; }
        Action<DesktopInfo> OnDesktopChanged { get; set; }
        Action<DesktopInfo> OnDesktopLost { get; set; }
        Action<DeviceData> OnDeviceLost { get; set; }
        void Start();
        void Stop();
    }
}
