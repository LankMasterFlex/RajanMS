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

        public static bool InRange<T>(this IEnumerable<T> sequence, int value)
        {
            return value >= 0 && value <= sequence.Count();
        }

        public static int ToInt32(this string value)
        {
            return Convert.ToInt32(value);
        }
    }
}
