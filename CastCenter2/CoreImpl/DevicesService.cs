namespace CastManager.CoreImpl
{
    using System;
    using System.Reactive.Linq;
    using System.Collections.ObjectModel;
    using CastManager.Logger;
    using CastManager.Models.Device;
    using CastManager.Templates;
    using CastManager.Core;
    using System.Data;
    using Newtonsoft.Json;
    using CastManager.StringsResources;
    using CastManager.Models.Core;

    public class DevicesService : IDevicesService
    {
        public IList<DeviceData> Devices { get; private set; } = new ObservableCollection<DeviceData>();

        public DeviceData SelectedDevice { get; set; }

        public Action<ItemAction, DeviceData> OnDevicePropertyChanged { get; set; }

        private object lockObj = new();

        private Lazy<string> LocalIp = new Lazy<string>(() => Helper.Network.GetLocalIPAddress());


        private IAppConfiguration appConfig;
        public DevicesService(ILookupService lookups, IMonitorService monitorService, IAppConfiguration appConfig)
        {
            lookups.OnDeviceFound += OnDeviceFound;
            monitorService.OnDeviceLost += OnDeviceNotFound;
            monitorService.Devices += () => Devices;

            this.appConfig = appConfig;
        }

        /// <summary>
        /// Add found device in to collection that show at UI
        /// </summary>
        private void OnDeviceFound(DeviceData deviceData)
        {
            var action = ItemAction.Added;
            lock (lockObj)
            {
                deviceData.DownloadConfig();
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    var existDeviceData = Devices.FirstOrDefault(d => d.IsEqual(deviceData));
                    if (existDeviceData == null)
                    {
                        //output just finded device config
                        Logger.WriteLine($" OnDeviceFound Id:{deviceData.Info.Id},config = {deviceData.Config}");
                        Devices.Add(deviceData);
                    }
                    else
                    {
                        Devices.Remove(existDeviceData);
                        Devices.Add(deviceData);
                        action = ItemAction.Updated;
                    }
                });
            }
            RaiseDevicePropertyChanged(action, deviceData);
        }

        /// <summary>
        /// Call`s when device service was lost, remove the Device item from the collection, that show at UI
        /// </summary>
        private void OnDeviceNotFound(DeviceData deviceData)
        {
            if (deviceData != null)
            {
                lock (lockObj)
                {
                    Devices.Remove(deviceData);
                }
                RaiseDevicePropertyChanged(ItemAction.Removed, deviceData);
            }
        }

        /// <summary>
        /// Template config is the combination of URL's builder results: 
        ///     1. Common or origin behavior: The user wants to add only one image on the device, 
        ///         - result is the ITemplateConfig.UrlA;
        ///     2. Template builder behavior: The user wants to add a combination of images on the device, 
        ///         - result is the url endpoint that return the image for that combination;
        /// </summary>
        private string GetResultUrl(ITemplateConfig config)
        {
            if (config.Type == TemplateType.Orign)
            {
                return config.GetUrl(ImageSlot.A);
            }

            var uri = appConfig.HttpServerPrefix.Replace("+", appConfig.LocalIp);
            return $"{uri}{config.Name}";            
        }

        private void RaiseDevicePropertyChanged(ItemAction action, DeviceData deviceData)
        {
            if (OnDevicePropertyChanged != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    OnDevicePropertyChanged.Invoke(action, deviceData);
                });
            }
        }

        /// <summary>
        /// Configs for device represent the list of endpoints(strings) to remote images.
        /// The template config represents that endpoint, and we should check whether to add it to the device config list or not.
        /// It depends on two things: 
        ///     1. the device was selected to add it, and the endpoint not aready present on the device config list, to be added;
        ///     2. if not selected, does the endpoint already exist on the device config list, to be deleted;
        /// </summary>
        async Task<(bool,string)> IDevicesService.SaveConfigToDevicesAsync(ITemplateConfig config)
        {
            await Task.Delay(1);

            if (Devices.Count == 0)
            {
                return (false, Strings.ById("NoneDeviceFound_Str"));
            }

            var imageUrl = GetResultUrl(config);

            /// Next filter the URL at all devices by following conditions:
            ///     1. If the checkbox was set for device 
            ///         a) URL results are found - it's ok
            ///         b) URL results are not found - add the url to the device config list
            ///     1. If the checkbox was NOT set for device 
            ///         a) URL results are NOT found - remove the url from the device config list
            ///         B) URL results are found - it's ok
            var checkedConfigs = Devices.Where(x => x.Info.IsChecked).Select(x => x.Config);
            var uncheckedConfigs = Devices.Where(x => !x.Info.IsChecked).Select(x => x.Config);
            
            if(config.IsCompositeTemplate && checkedConfigs.Count() == 0)
            {
                return (false, Strings.ById("NoneDeviceCheckboxed_Str"));
            }

            checkedConfigs
                .Where(x => !x.IsConfig(imageUrl)).ToList()
                .ForEach(x => x.AddNewDefaultConfig(imageUrl));

            uncheckedConfigs
                .Where(x => x.IsConfig(imageUrl)).ToList()
                .ForEach(x => x.RemoveConfig(imageUrl));

            try
            {
                var tasks = Devices.Select(x => x.UploadConfigAsync());
                await Task.WhenAll(tasks);
                return (true, Strings.ById("ConfigSaved_Str"));
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }
            return (false, Strings.ById("ConfigSaveFail_Str"));
        }

        void IDevicesService.UpdateCheckboxesForDevices(ITemplateConfig config)
        {
            var url = GetResultUrl(config);
            lock (lockObj)
            {
                Devices
                    .ToList()
                    .ForEach(x => x.Info.IsChecked = x.Config.HostConfigs.Any(x => x.ImageUrl == url));
            }

            RaiseDevicePropertyChanged(ItemAction.Updated, null);
        }


        void IDevicesService.OnDeviceSelected(DeviceData deviceData)
        {
            SelectedDevice = deviceData;
        }
    }


}
