namespace CastManager.Lookup
{
    using CastManager.Helper;
    using CastManager.RoundRobin;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    class LocalAddresses : IAddressesLoop
    {
        private readonly ISet<string> ExceptAddresses = new SortedSet<string>();

        private readonly IRoundRobin RoundRobinTable = new RoundRobinTable(new int[] { 1, 10, 30, 50, 100, 150, 200, 256 });

        static string SubLocalIp = Network.SubLocalIp;

        string IAddressesLoop.Next
        {
            get
            {
                var i = RoundRobinTable.Next;
                if (i > 0 && i <= 255)
                {
                    var ipAddress = $"{SubLocalIp}.{i}";
                    Debug.WriteLine($"RoundRobinAddress, idx={i}, {ipAddress}");
                    if (ExceptAddresses.Contains(ipAddress) == false)
                        return ipAddress;
                }
                throw new InvalidOperationException("Could`t happens!!!");
            }
        }

        bool IAddressesLoop.AddFilter(string address) => ExceptAddresses.Add(address);

        bool IAddressesLoop.IsAtFilter(string address) => ExceptAddresses.Contains(address);

        void IAddressesLoop.ClearFilters() => ExceptAddresses.Clear();

        int IAddressesLoop.Count => 255;
    }
}
