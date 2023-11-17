namespace CastManager.Templates
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    [DataContract]
    public class ImageSlotData : IImageSlotData
    {
        public ImageSource Source => source;

        public string Hash => _hash;

        public string Url => _url;

        [DataMember(Name ="url")]
        private string _url;

        private string _hash;

        private ImageSource source;

        public Action<IImageSlotData> OnUpdated { get; set; }

        public bool IsUrl => _url != null;
        
        public bool IsValid => _url != null && source != null && _hash != null;
        
        public bool IsUpdated
        {
            get
            {
                lock (_lockObj)
                {
                    return _isUpdated;
                }
            }
            private set
            {
                lock (_lockObj)
                {
                    _isUpdated = value;
                }
            }
        }

        private bool _isUpdated = false;

        private object _lockObj = new();

        public void Update(IImageSlotData slot)
        {
            if (slot != this)
            {
                this.source = slot.Source;
                this._hash = slot.Hash;
            }
            OnUpdated?.Invoke(this);
            IsUpdated = true;
        }

        public void ReleaseUpdateState()
        {
            IsUpdated = false;
        }

        public void SetUrl(string url)
        {
            this._url = url;
        }

        public void SetSource(ImageSource source)
        {
            this.source = source;
        }

        public void SetSourceChecksum(string hash)
        {
            this._hash = hash;
        }
    }
}
