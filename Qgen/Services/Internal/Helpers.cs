namespace Qgen.Services.Internal;

public static class Helpers
{
    public static T[] OrEmpty<T>(this T[]? collection) => collection ?? Array.Empty<T>();

    public static IEnumerable<T> IfEmpty<T>(this IEnumerable<T> s, T item) => s.Any() ? s : new[] { item };

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) => source.Where(x => x is not null)!;

    public static V GetOrAdd<K, V>(this IDictionary<K, V> map, K key, Func<V> factory)
    {
        if (map.TryGetValue(key, out V v))
            return v;

        map.Add(key, v = factory());
        return v;
    }

    public static V SafeGet<K, V>(this IDictionary<K, V> map, K key, V fallback) => map.TryGetValue(key, out V v) ? v : fallback;

    public static T Pipe<S, T>(this S value, Func<S, T> f) => f(value);

    public static T Pipe<S, A, T>(this S value, Func<A, S, T> f, A arg) => f(arg, value);

    public static Func<T, bool> Not<T>(Func<T, bool> f) => v => !f(v);

    public static IEnumerable<T> OfTypes<T>(this IEnumerable<T> source, params Type[] types)=>
        source.Where(x => types.Any(t => t.IsAssignableFrom(x.GetType())));

    public static IEnumerable<(T value, int index)> WithIndices<T>(this IEnumerable<T> s) => s.Select((x, i) => (x, i));

    public static T Id<T>(T arg) => arg;

    public static IEqualityComparer<T> ToComparer<T>(Func<T, T, bool> cmp) => new DelegatedComparer<T>(cmp);

    private class DelegatedComparer<T>(Func<T, T, bool> Eq) : IEqualityComparer<T>
    {
        public bool Equals(T x, T y) => Eq(x, y);

        public int GetHashCode(T obj) => obj?.GetHashCode() ?? 0;
    }
}