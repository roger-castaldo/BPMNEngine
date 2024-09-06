namespace BPMNEngine
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enu, Action<T> action)
        {
            foreach (T item in enu) action(item);
            return enu; // make action Chainable/Fluent
        }

        public static async Task<IEnumerable<T>> ForEachAsync<T>(this IEnumerable<T> enu, Func<T,Task> action)
        {
            await Task.WhenAll(enu.Select(item => action(item)));
            return enu; // make action Chainable/Fluent
        }

        public static async ValueTask<IEnumerable<T>> ForEachAsync<T>(this IEnumerable<T> enu, Func<T, ValueTask> action)
            => await ForEachAsync<T>(enu, item => action(item).AsTask());

        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items,
            Func<T, IEnumerable<T>> childSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Count>0)
            {
                var next = stack.Pop();
                yield return next;
                childSelector(next).ForEach(child => stack.Push(child));
            }
        }

        public static int IndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            var grp = source.Select((v, i) => new { value = v, index = i })
                .FirstOrDefault(g => predicate.Invoke(g.value));
            return grp==null ? -1 : grp.index;
        }

        public static async Task<bool> AllAsync<TSource>(this IEnumerable<TSource> source,Func<TSource,Task<bool>> predicate)
        {
            var results = (await Task.WhenAll<bool>(source.Select(item => predicate(item)))).ToArray();
            return Array.TrueForAll<bool>(results, b => b);
        }

        public static async ValueTask<bool> AllAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate)
            => await AllAsync<TSource>(source,item=> predicate(item).AsTask());

        public static async Task<bool> AnyAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicate)
        {
            foreach(var item in source)
            {
                if (await predicate(item))
                    return true;
            }
            return false;
        }

        public static async ValueTask<bool> AnyAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate)
            => await AllAsync<TSource>(source, item => predicate(item).AsTask());

        public static async Task<TSource?> FirstOrDefaultAsync<TSource>(this IEnumerable<TSource> source,Func<TSource,Task<bool>> predicate)
        {
            foreach(var item in source)
            {
                if (await predicate(item))
                    return item;
            }
            return default;
        }

        public static async ValueTask<TSource?> FirstOrDefaultAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate)
            => await FirstOrDefaultAsync<TSource>(source, item => predicate(item).AsTask());

        public static async Task<IEnumerable<TSource>> WhereAsync<TSource>(this IEnumerable<TSource> source,Func<TSource,Task<bool>> predicate)
        {
            var results = await Task.WhenAll(source.Select(async item =>
            new {
                Item=item,
                Result = await predicate(item)
            }));
            return results.Where(res => res.Result).Select(res => res.Item);
        }

        public static async ValueTask<IEnumerable<TSource>> WhereAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, ValueTask<bool>> predicate)
            => await WhereAsync<TSource>(source,item=>predicate(item).AsTask());

        public static async Task<IEnumerable<IGrouping<TKey,TSource>>> GroupByAsync<TKey,TSource>(this IEnumerable<TSource> source, Func<TSource, Task<TKey>> predicate)
        {
            var results = await Task.WhenAll(source.Select(async item =>
            new {
                Item = item,
                Result = await predicate(item)
            }));
            return results.GroupBy(item => item.Result,item=>item.Item);
        }

        public static async ValueTask<IEnumerable<IGrouping<TKey, TSource>>> GroupByAsync<TKey,TSource>(this IEnumerable<TSource> source, Func<TSource, ValueTask<TKey>> predicate)
            => await GroupByAsync<TKey,TSource>(source, item => predicate(item).AsTask());

    }
}
