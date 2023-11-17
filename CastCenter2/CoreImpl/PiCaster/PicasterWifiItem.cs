namespace CastManager.Models
{
    public class PicasterWifiItem
    {
        public string Id { get; set; } = string.Empty;
        public string SsidText { get; set; } = string.Empty;
        public string PicasterImg { get; set; } = "/images/picaster.png";

        private string ssid;
        public string Ssid
        {
            get
            {
                return ssid;
            }
            set
            {
                ssid = value;
                SsidText = value;

                if (SsidText != null)
                {
                    SsidText = SsidText.Replace("PiCaster_", "");
                }
            }
        }
    }
}
