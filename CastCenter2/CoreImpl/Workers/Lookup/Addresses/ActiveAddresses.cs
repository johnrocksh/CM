namespace CastManager.CoreImpl.Lookup.Addresses
{
    using CastManager.CoreImpl.Lookup.RoundRobin;
    using System.Collections.Generic;

    public class ActiveAddresses : IAddressesLoop
    {
        readonly ISet<string> ExceptAddresses = new SortedSet<string>();

        readonly IRoundRobinContainer<string> RoundRobin = new RoundRobinContainer<string>();

        private ActiveAddresses()
        {

        }

        public ActiveAddresses(IEnumerable<string> addresses)
        {
            foreach (var address in addresses)
                RoundRobin.Add(address);
        }

        bool IAddressesLoop.AddFilter(string address) => ExceptAddresses.Add(address);

        bool IAddressesLoop.IsAtFilter(string address) => ExceptAddresses.Contains(address);

        void IAddressesLoop.ClearFilters() => ExceptAddresses.Clear();

        string IAddressesLoop.Next
        {
            get
            {
                for (int i = 0; i < RoundRobin.Count; i++)
                {
                    var ip = RoundRobin.NextValue;
                    if (ExceptAddresses.Contains(ip) == false)
                        return ip;
                }
                throw new ActiveAddressesException("Ups, all active addresses was filtered");
            }
        }

        int IAddressesLoop.Count => RoundRobin.Count;
    }
}
