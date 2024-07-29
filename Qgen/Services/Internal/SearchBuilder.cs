using System.Linq.Expressions;
using Qgen.Contracts.Services;
using static System.Linq.Expressions.Expression;

namespace Qgen.Services.Internal
{
    internal static class SearchBuilder
    {
        internal static Expression<Func<T, bool>> Build<T>(string? searchQuery, string[]? include, Schema<T> schema)
        {
            if (string.IsNullOrWhiteSpace(searchQuery)) return t => true;

            var query = Constant(searchQuery);

            Func<(string, Func<ParameterExpression, Expression>), bool> filterSearch =
                include?.Any() == true
                ? t => include.Contains(t.Item1)
                : _ => true;
            var searchFields = schema.GetSearchable().Where(filterSearch);

            var p = Parameter(typeof(T), "t");

            var body = searchFields.Select(t => schema.GetSearchTransformer(t.Item1)(t.Item2(p), query))
                .Aggregate(And);

            return Lambda<Func<T, bool>>(body, p);
        }
    }
}
