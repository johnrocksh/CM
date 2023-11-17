namespace CastManager.Templates
{
    public interface IPendingContext
    {
        bool IsElapsed { get; }

        TimeSpan Elapsed { get; }
    }
}
