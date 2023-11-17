namespace CastManager.CoreImpl.Lookup.RoundRobin
{
    using System.Collections.Generic;
    using System.Linq;

    public class RoundRobinContainer<T> : IRoundRobinContainer<T>
    {
        List<T> list = new();

        int rrc = 0;

        int IRoundRobinContainer<T>.Count => list.Count;

        T IRoundRobinContainer<T>.NextValue
        {
            get
            {
                if (rrc >= list.Count)
                {
                    rrc = 0;
                }
                var item = list.ElementAt(rrc);
                rrc++;
                return item;
            }
        }

        void IRoundRobinContainer<T>.Add(T obj)
        {
            list.Add(obj);
        }

        void IRoundRobinContainer<T>.Del(T obj)
        {
            list.Remove(obj);
        }
    }
}
