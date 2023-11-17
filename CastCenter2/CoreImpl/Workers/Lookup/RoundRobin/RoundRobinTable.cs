namespace CastManager.RoundRobin
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public class RoundRobinTable : IRoundRobin
    {
        /// <summary>
        /// Roundrobins over distribution table
        /// </summary>
        readonly List<IRoundRobin> rrcs = new();

        /// <summary>
        /// Roundrobin over distribution table index
        /// </summary>
        IRoundRobin rrcT;

        /// <summary>
        /// Return roundrobin value from distribution roundrobin table
        /// </summary>
        public int Next
        {
            get
            {
                var i = rrcT.Next;
                var rr = rrcs[i];
                return rr.Next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public RoundRobinTable()
            :this(new int[] { 0, 5, 10})
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public RoundRobinTable(int[] dist_table)
        {
            var segments = dist_table.Length - 1;
            rrcT = new RoundRobin(0, segments);
            rrcs.Clear();
            for (int i = 0; i < segments; i++)
            {
                int a = dist_table[i];
                int b = dist_table[i + 1];
                rrcs.Add(new RoundRobin(a, b));
            }
        }
    }
}
