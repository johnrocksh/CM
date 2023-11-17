namespace CastManager.Templates
{
    public interface IPendingContent
    {
        bool IsElapsed { get; }

        TimeSpan Elapsed { get; }
    }
}
