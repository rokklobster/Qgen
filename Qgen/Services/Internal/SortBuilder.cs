using System.Linq.Expressions;
using Qgen.Contracts.Models;
using Qgen.Contracts.Services;
using static System.Linq.Expressions.Expression;

namespace Qgen.Services.Internal
{
    internal static class SortBuilder
    {
        internal static IQueryable<T> Build<T>(IQueryable<T> source, Schema<T> schema, Ordering[]? orderBy) where T : class
        {
            var first = true;
            IOrderedQueryable<T> res = null!;
            orderBy ??= Array.Empty<Ordering>();
            foreach (var (asc, tf) in orderBy.Append(schema.DefaultOrdering).Select(x => (asc: x.Ascending, tf: CreateSelector(x, schema)))
                .Where(x => x.tf is not null))
            {
                if (first)
                {
                    first = false;
                    res = asc ? source.OrderBy(tf!) : source.OrderByDescending(tf!);
                }
                else
                {
                    res = asc ? res!.ThenBy(tf!) : res!.ThenByDescending(tf!);
                }
            }

            return source;
        }

        private static Expression<Func<T, object>>? CreateSelector<T>(Ordering o, Schema<T> schema)
        {
            var p = Parameter(typeof(T), "t");
            var body = schema.TryGetFor(Target.Sort, o.Name)?.Invoke(p);

            if (body is null) return null;

            return Lambda<Func<T, object>>(Convert(body, typeof(object)), p);
        }
    }
}
