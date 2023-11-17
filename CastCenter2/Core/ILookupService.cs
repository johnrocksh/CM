namespace CastManager.Core
{
    using CastManager.Models.Desktops;
    using CastManager.Models.Device;
    public interface ILookupService
    {
        Action<DeviceData> OnDeviceFound { get; set; }
        Action<DesktopInfo> OnDesktopFound { get; set; }
        void Start();
        void Stop();
        void Wait();
        bool AddIpAddreess(string ip);
        void StartLookupDesktops();
        void StartLookupDevices();
    }
}
