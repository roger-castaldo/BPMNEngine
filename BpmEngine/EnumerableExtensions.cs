using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Reddragonit.BpmEngine
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enu, Action<T> action)
        {
            foreach (T item in enu) action(item);
            return enu; // make action Chainable/Fluent
        }

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items,
            Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                childSelector(next).ForEach(child=>stack.Push(child));
            }
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {

            var index = 0;
            foreach (var item in source)
            {
                if (predicate.Invoke(item))
                {
                    return index;
                }
                index++;
            }

            return -1;
        }
    }
}
