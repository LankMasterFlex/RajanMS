using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RajanMS
{
    static class Extensions
    {
        public static void ForAll<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (T item in sequence)
                action(item);
        }

        public static void InvertedFor<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            for (int i = sequence.Count(); i-- > 0; )
                action(sequence.ElementAt(i));
        }

        public static bool InRange<T>(this IEnumerable<T> sequence, int value)
        {
            return value >= 0 && value <= sequence.Count();
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
