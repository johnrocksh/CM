namespace CastManager.Templates
{
    using System.Windows.Media;
    using System.Linq;

    using Newtonsoft.Json;
    using CastManager.Models.Desktops;
    using System.Runtime.Serialization;
    using Newtonsoft.Json.Converters;
    using CastManager.Models;


    [DataContract]
    public class TemplateConfig : NotifyPropertyBase, ITemplateConfig
    {
        public TatamiTabloInfo SelectedTablo { get; private set; }

        [DataMember(Name = "saved")]
        public bool IsSaved { get; private set; }

        private bool _invalidated;

        public bool IsUpdated => Slots.Any(x => x.IsUpdated);

        public bool Invalidated
        {
            get
            {
                // fire only once (return true), if was invalidated 
                var r = _invalidated;
                if (r)
                {
                    _invalidated = false;    
                }
                return r;
            }
        }

        private byte[] resultRawData;

        private object _lockObj = new();

        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "type")]
        [JsonConverter(typeof(StringEnumConverter))]

        public TemplateType Type { get; private set; }

        public bool IsCompositeTemplate => Type != TemplateType.Orign;

        [DataMember(Name = "slots")]
        private Dictionary<ImageSlot, ImageSlotData> imageSlots = new();

        public object UrlA => GetSourceObject(ImageSlot.A);
        public object UrlB => GetSourceObject(ImageSlot.B);
        public object UrlC => GetSourceObject(ImageSlot.C);
        public object UrlD => GetSourceObject(ImageSlot.D);

        public ImageSlot SelectedImageSlot { get; set; }

        public string[] urls => imageSlots.Select(i => i.Value.Url).ToArray();

        public IEnumerable<IImageSlotData> Slots => imageSlots.Select(i => i.Value);

        public Action<ImageSlot, IImageSlotData> OnImageSlotChanged { get; set; }

        public Action OnTemplateTypeChanged { get; set; }

        public Action OnTatamiTabloChanged { get; set; }

        private IDictionary<string, IImageSlotData> _slotsPool;

        private readonly IImageSlotData _emptySlot = new ImageSlotData();
        public TemplateConfig(IDictionary<string, IImageSlotData> slotsPool)
        {
            this._slotsPool = slotsPool;
        }

        public void Init(IDictionary<string, IImageSlotData> slotsPool)
        {
            this._slotsPool = slotsPool;

            // reconstruct slot data instance for slot data pool
            imageSlots = imageSlots.ToDictionary(x => x.Key, x => GetSlotData(x.Value.Url) as ImageSlotData);

            InitSlot(ImageSlot.A, nameof(UrlA));
            InitSlot(ImageSlot.B, nameof(UrlB));
            InitSlot(ImageSlot.C, nameof(UrlC));
            InitSlot(ImageSlot.D, nameof(UrlD));

            _invalidated = true;
        }

        void InitSlot(ImageSlot slot, string propertyName)
        {
            var slotData = GetSlotData(slot);
            if (slotData != null)
            {
                slotData.OnUpdated += s =>
                {
                    RaisePropertyChanged(propertyName);
                    OnImageSlotChanged?.Invoke(slot, slotData);
                };
            }
        }

        public string GetUrl(ImageSlot slot) => GetSlotData(slot)?.Url;

        public ImageSource GetImage(ImageSlot slot) => GetSlotData(slot)?.Source;

        public void SetTatamiTablo(TatamiTabloInfo val)
        {
            SelectedTablo = val;
            OnTatamiTabloChanged?.Invoke();
        }

        public void SetTamplateType(TemplateType templateType)
        {
            Type = templateType;
            OnTemplateTypeChanged?.Invoke();
        }

        public void SetUrl(string url) => SetUrl(SelectedImageSlot, url);

        public void SetUrl(ImageSlot slot, string url)
        {
            if (url == null)
            {
                imageSlots.Remove(slot);
                OnImageSlotChanged?.Invoke(slot, null);
            }
            else
            {
                var slotData = GetSlotData(url) as ImageSlotData;
                imageSlots[slot] = slotData ;
                OnImageSlotChanged?.Invoke(slot, slotData);
            }
        }

        public void SetConfigSaved()
        {
            IsSaved = true;
            _invalidated = true;
        }

        public void SetResultRawData(byte[] data)
        {
            lock (_lockObj)
            {
                resultRawData = data;
                Slots.ToList().ForEach(x => x.ReleaseUpdateState());
            }
        }

        public byte[] GetResultRawData()
        {
            lock (_lockObj)
            {
                return resultRawData;
            }
        }

        private IImageSlotData GetSlotData(ImageSlot slot)
        {
            imageSlots.TryGetValue(slot, out var slotData);
            return slotData;
        }

        private IImageSlotData GetSlotData(string url)
        {
            if (url == null)
            {
                return _emptySlot;
            }

            if (!_slotsPool.TryGetValue(url, out var slotData))
            {
                _slotsPool.Add(url, slotData = new ImageSlotData());
                slotData.SetUrl(url);
            }
            return slotData;
        }

        private object GetSourceObject(ImageSlot slot)
        {
            var source = GetImage(slot);
            var url = GetUrl(slot);
            return source == null ? url : source;
        }
    }
}
