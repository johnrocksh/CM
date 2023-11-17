namespace CastManager.Models.Device
{
    public class DeviceInfo : NotifyPropertyBase
    {
        public string Id { get; set; } = "";
        public string IPAddress { get; set; } = "";
        public int Port { get; set; }
        public string Path { get; set; } = "";

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                RaisePropertyChanged();
            }
        }
    }
}
