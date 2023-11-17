namespace CastManager.Models.Desktops
{
    public class DesktopConfig : NotifyPropertyBase
    {
        public string TatamiId { get; set; } = string.Empty;

        public string Ip { get; set; } = string.Empty;

        public string Mac { get; set; } = string.Empty;

        public string TatamiName { get; set; } = string.Empty;

        public string DesktopVersion { get; set; } = string.Empty;

        public string DisciplineText { get; set; } = string.Empty; // contains a text value in the current language

        public List<TatamiTabloInfo> TatamiConfigUrls { get; set; } = new List<TatamiTabloInfo>();

        public bool IsActive => !IsPassive;

        public bool IsPassive => string.IsNullOrEmpty(TatamiName);

        public bool IsEqualTo(DesktopConfig config)
        {
            var _this = this;
            if (config == null && _this == null)
                return true;
            if (config != null && _this != null)
                return _this.TatamiId == config.TatamiId
                    && _this.TatamiName == config.TatamiName;
            return false;
        }

        public void Update()
        {
            TatamiConfigUrls.ForEach(x => x.Udate());
            RaisePropertyChanged(nameof(TatamiConfigUrls));
        }
    }
}