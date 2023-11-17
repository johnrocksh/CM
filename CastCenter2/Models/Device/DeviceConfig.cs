namespace CastManager.Models.Device
{
    using System.Collections.Generic;    
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;

    [DataContract]
    public class DeviceConfig : NotifyPropertyBase
    {
        /// <summary>
        /// Contait all configs(info) of selected device
        /// </summary>   
        [DataMember(Name = "images")]
        public IList<HostConfig> HostConfigs { get; private set; } = new ObservableCollection<HostConfig>();

        public HostConfig GetConfig(string imageUrl)
        {
            return HostConfigs.FirstOrDefault(hostConfig => hostConfig.ImageUrl == imageUrl);
        }
        
        public bool IsConfig(string imageUrl) => GetConfig(imageUrl) != null;

        public HostConfig RemoveConfig(string imageUrl)
        {
            var item = GetConfig(imageUrl);
            if (item != null)
            {
                HostConfigs.Remove(item);
            }
            return item;
        }

        public HostConfig AddNewDefaultConfig(string imageUrl)
        {
            var item = GetConfig(imageUrl);
            if (item == null)
            {
                item = new HostConfig()
                {
                    ImageUrl = imageUrl,
                    Animation = Animation.Blank,
                    DisplaySeconds = 8
                };
                HostConfigs.Add(item);
            }
            return item;
        }

        /// <summary>
        /// Removes configurations that are marked, retaining unmarked configurations.
        /// </summary>
        public void RemoveMarkedConfigs()
        {
            HostConfigs = new ObservableCollection<HostConfig>(HostConfigs.Where(x => !x.IsMarkedForAction));
        }

        /// <summary>
        /// Removes configurations that ImageUrl is not set.
        /// </summary>
        public void RemoveEmptyConfigs()
        {
            HostConfigs = new ObservableCollection<HostConfig>(HostConfigs.Where(x => !string.IsNullOrEmpty(x.ImageUrl)));
        }

        public void Update()
        {
            HostConfigs.ToList().ForEach(x => x.Update());
            RaisePropertyChanged(nameof(HostConfigs));
        }

    }
}
