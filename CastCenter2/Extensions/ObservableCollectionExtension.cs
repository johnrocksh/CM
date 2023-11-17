namespace CastManager.Extentions
{
    using System.Collections.ObjectModel;
    public static class ObservableCollectionExtension
    {

        static public void AddRange<T>(this ObservableCollection<T> items, IEnumerable<T> collection)
            where T : class
        {
            foreach (var i in collection)
            {
                items.Add(i);
            }
        }
    }
}
