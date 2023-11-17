namespace CastManager.Templates
{
    using System.Windows.Media;

    public interface IImageSlotData
    {
        string Url { get; }
        string Hash { get; }
        ImageSource Source { get; }

        Action<IImageSlotData> OnUpdated { get; set; }

        bool IsUrl { get; }
        bool IsValid { get; }
        bool IsUpdated { get; }

        void Update(IImageSlotData slot);

        void ReleaseUpdateState();

        void SetUrl(string url);
        void SetSource(ImageSource source);
        void SetSourceChecksum(string hash);
    }
}
