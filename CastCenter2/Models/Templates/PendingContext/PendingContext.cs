namespace CastManager.Templates
{
    using System.Diagnostics;

    public class PendingContext<T> : IPendingContext
    {
        public T Context {  get; }

        public bool IsElapsed => TimeSpan.Compare(Elapsed, pending) > 0;

        public TimeSpan Elapsed => stopwatch.Elapsed;

        private readonly TimeSpan pending;

        private readonly Stopwatch stopwatch = new();
        
        public PendingContext(T context, TimeSpan pending)
            : this(pending)
        {
            Context = context;
        }

        public PendingContext(TimeSpan pending)
        {
            this.stopwatch.Start();
            this.pending = pending;
        }
    }
}
