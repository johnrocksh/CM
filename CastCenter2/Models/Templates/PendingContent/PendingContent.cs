namespace CastManager.Templates
{
    using System.Diagnostics;

    public class PendingContent<T> : IPendingContent
    {
        public T OwnerData {  get; }

        public bool IsElapsed => TimeSpan.Compare(Elapsed, pending) > 0;

        public TimeSpan Elapsed => stopwatch.Elapsed;

        private readonly TimeSpan pending;

        private readonly Stopwatch stopwatch = new();
        
        public PendingContent(T ownerData, TimeSpan pending)
            : this(pending)
        {
            OwnerData = ownerData;
        }

        public PendingContent(TimeSpan pending)
        {
            this.stopwatch.Start();
            this.pending = pending;
        }
    }
}
