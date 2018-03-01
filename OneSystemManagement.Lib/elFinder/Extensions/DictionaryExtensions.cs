using System.Collections.Generic;

namespace OneSystemManagement.Lib.elFinder.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return defaultValue;
        }
    }
}