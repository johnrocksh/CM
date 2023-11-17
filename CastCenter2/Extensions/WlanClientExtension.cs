namespace CastManager.Helper
{
    using System.Text;
    using NativeWifi;
    static class WlanClientExtension
    {
        /// <summary>
        /// Convert Dot11Ssid to string Encoding ASCII
        /// </summary>
        static public string ToStringV2(this Wlan.Dot11Ssid? ssid)
        {
            if (ssid == null)
            {
                throw new ArgumentNullException(nameof(ssid));
            }
            return Encoding.ASCII.GetString(ssid?.SSID, 0, (int)ssid?.SSIDLength);
        }

    }
}
