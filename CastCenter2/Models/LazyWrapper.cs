using System.Runtime.CompilerServices;

namespace CastManager.Models
{
    public class LazyNameNotUniqueException : Exception
    {
        public LazyNameNotUniqueException() :
            base("Supported only the unique variable name.")
        {
        }
    }
    public class LazyNameShouldBeSetException : Exception
    {
        public LazyNameShouldBeSetException() :
            base("The variable name shoud be provided but not empty or null.")
        {
        }
    }

    /// <summary>
    /// A class providing lazy initialization wrappers for objects, allowing them to be created and stored based on their variable names.
    /// </summary>
    public class LazyWrappers
    {
        /// <summary>
        /// The lazy objects storage
        /// </summary>
        Dictionary<string, object> lzwStorage = new();

        /// <summary>
        /// Initializes a new instance of the T class or get last initialized by 'varName'
        /// </summary>
        public T Get<T>([CallerMemberName] string varName = null)
            where T : class, new()
        {
            if (string.IsNullOrEmpty(varName))
            {
                throw new LazyNameShouldBeSetException();
            }

            Lazy<T> lzw = null;

            if (lzwStorage.TryGetValue(varName, out var value))
            {
                lzw = value as Lazy<T>;

                if (lzw == null)
                {
                    throw new LazyNameNotUniqueException();
                }
            }
            else
            {
                lzw = new Lazy<T>(() => new T());
                lzwStorage.Add(varName, lzw);
            }

            return lzw.Value;
        }
    }

}
