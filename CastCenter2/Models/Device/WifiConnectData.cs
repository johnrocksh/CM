namespace CastManager.Client.Device
{
    using System.Runtime.Serialization;

    [DataContract]
    public class WifiConnectData
    {
        [DataMember(Name = "essid")]
        public string Essid { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

    [DataContract]
    public class AccessPointConfig
    {
        [DataMember(Name = "essid")]
        public string Essid { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}
