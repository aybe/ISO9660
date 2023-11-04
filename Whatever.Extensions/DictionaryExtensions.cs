using System;
using System.Collections.Generic;

namespace Whatever.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            value = valueFactory();

            dictionary.Add(key, value);

            return value;
        }
    }
}