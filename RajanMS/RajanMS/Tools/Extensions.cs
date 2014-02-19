using System;
using System.Collections.Generic;
using System.Linq;

namespace RajanMS
{
    static class Extensions
    {
        public static void ForAll<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T item in sequence)
                action(item);
        }

        public static T FindOne<T>(this IEnumerable<T> sequence,Func<T,bool> predicate)
        {
            foreach (T item in sequence)
            {
                if (predicate(item)) //returned true
                    return item;
            }

            return default(T);
        }

        public static int ToInt32(this string value)
        {
            return Convert.ToInt32(value);
        }
    }
}
