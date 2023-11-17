namespace CastManager.CoreImpl.Lookup.RoundRobin
{
    interface IRoundRobinContainer<T>
    {
        T NextValue { get; }
        void Add(T obj);
        void Del(T obj);
        int Count { get; }
    }
}
