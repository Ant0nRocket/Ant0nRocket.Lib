using System.Collections.Generic;

namespace Ant0nRocket.Lib.Std20.Extensions
{
    /// <summary>
    /// Extensions of <see cref="HashSet{T}"/>.
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Addes <paramref name="value"/> only if same value is not
        /// already in <paramref name="hashSet"/>.
        /// </summary>
        public static bool AddSecure<T>(this HashSet<T> hashSet, T value)
        {
            if (hashSet.Contains(value)) return false;
            return hashSet.Add(value);
        }
    }
}
