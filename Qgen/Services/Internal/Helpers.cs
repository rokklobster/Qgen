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

    public static T Pipe<S, T>(this S value, Func<S, T> f) => f(value);
}