using System.Linq.Expressions;
using Qgen.Contracts.Models;

namespace Qgen.Contracts.Services
{
    public enum Target
    {
        Filter, Search, Sort, Group
    }

    public interface Schema<T>
    {
        IEnumerable<(string, Func<ParameterExpression, Expression>)> GetSearchable();
        Func<Expression, Expression, Expression> GetSearchTransformer(string field);

        /// <summary>
        /// get type of field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        Type GetType(string field);

        /// <summary>
        /// fetch transformer for field and operation
        /// </summary>
        /// <param name="target"></param>
        /// <param name="field"></param>
        /// <returns>null in case transformer is not registered; otherwise - function of parameter and operation arg (filter value, search query; null for sorting and grouping)</returns>
        Func<ParameterExpression, Expression>? TryGetFor(Target target, string field);

        Ordering DefaultOrdering { get; }
    }

    public interface SchemaRepo
    {
        Schema<T>? Get<T>();
    }
}
