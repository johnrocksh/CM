namespace CastManager.CoreImpl.Lookup.Addresses
{
    public interface IAddressesLoop
    {
        string Next { get; }

        bool AddFilter(string address);

        bool IsAtFilter(string address);

        void ClearFilters();

        int Count { get; }
    }
}
