namespace CastManager.Models.Device
{
    using System.Runtime.Serialization;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [DataContract]
    public class HostConfig : NotifyPropertyBase
    {
        [DataMember(Name = "image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// How many seconds this image is displayed
        /// </summary>
        [DataMember(Name = "display_seconds")]
        public ulong DisplaySeconds { get; set; }

        /// <summary>
        /// Animation is what kind of animation is used when displaying pictures on the Tablo
        /// </summary>
        [DataMember(Name = "animation")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Animation Animation { get; set; }

        public bool IsMarkedForAction { get; set; }

        public void Update()
        {
            RaisePropertyChanged(nameof(ImageUrl));
        }
    }
}
