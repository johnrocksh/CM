namespace CastManager.CoreImpl.Lookup.RoundRobin
{
    using System.Diagnostics;

    public class RoundRobin : IRoundRobin
    {
        readonly bool includeTheEnd;

        readonly int From = 0;

        readonly int To = 100;

        int current = -1;

        public int Next
        {
            get
            {
                ++current;
                if (current >= To && !includeTheEnd || current > To)
                {
                    current = From;
                }
                return current;
            }
        }

        public RoundRobin(bool includeTheEnd = false)
        {
            this.includeTheEnd = includeTheEnd;
        }

        public RoundRobin(int a, int b, bool includeTheEnd = false)
            : this(includeTheEnd)
        {
            From = a;
            To = b;
            current = From - 1;
        }
    }
}
