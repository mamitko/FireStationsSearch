using System;
using System.Collections.Generic;
using System.Linq;

namespace FireDepartmentSearch
{
    public static class Extensions
    {
        /// <summary>
        /// Note: It causes boxing for value types
        /// </summary>
        public static T WhereMin<T>(this IEnumerable<T> enumerable, Func<T, IComparable> func)
        {
            IComparable maxValue = null;
            var maxItem = default(T);

            foreach (var item in enumerable)
            {
                var itemValue = func(item);

                if (maxValue == null || maxValue.CompareTo(itemValue) > 0)
                {
                    maxValue = itemValue;
                    maxItem = item;
                }
            }

            if (maxValue == null)
                throw new InvalidOperationException("Sequence is empty. No 'min' element could be found.");

            return maxItem;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> enumerable, T item)
        {
            return enumerable.Where(n => !n.Equals(item));
        }

        public static void ForAll<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }
    }
}