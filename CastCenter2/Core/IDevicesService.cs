namespace CastManager.Core
{
    using CastManager.Models.Device;
    using CastManager.Templates;
    using CastManager.Models.Core;

    public interface IDevicesService
    {
        Action<ItemAction, DeviceData> OnDevicePropertyChanged { get; set; }

        IList<DeviceData> Devices { get; }
        DeviceData SelectedDevice { get; }

        Task<(bool, string)> SaveConfigToDevicesAsync(ITemplateConfig config);

        void UpdateCheckboxesForDevices(ITemplateConfig config);

        void OnDeviceSelected(DeviceData deviceData);


    }
}
