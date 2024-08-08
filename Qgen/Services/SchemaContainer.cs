using System.Linq.Expressions;
using Qgen.Contracts.Models;
using Qgen.Contracts.Services;
using static System.Linq.Expressions.Expression;
using static Qgen.Services.Expressions;

namespace Qgen.Services;

internal class FieldSchema(Type type)
{
    public Type Type { get; internal set; } = type;

    public Func<ParameterExpression, Expression>? Filter { get; internal set; }
    public Func<ParameterExpression, Expression>? Sort { get; internal set; }
    public Func<ParameterExpression, Expression>? Search { get; internal set; }
    public Func<ParameterExpression, Expression>? Group { get; internal set; }
}

internal class Customizations
{
    public Func<Expression, Expression>? Filter { get; internal set; }
    public Func<Expression, Expression>? Sort { get; internal set; }
    public Func<Expression, Expression>? Search { get; internal set; }
    public Func<Expression, Expression>? Group { get; internal set; }
}

public class SchemaContainer<T> : Schema<T>
{
    internal readonly Dictionary<string, FieldSchema> schemata = new();
    internal readonly Dictionary<string, Customizations> customizations = new();

    internal readonly Dictionary<string, Func<Expression, Expression, Expression>> customSearchTransformers = new();

    private readonly Func<Expression, Expression, Expression> defaultSearch = (s, q) => Call(s, StringContainsMethod, q);

    public Ordering? DefaultOrdering { get; internal set; }

    public IEnumerable<(string, Func<ParameterExpression, Expression>)> GetSearchable() =>
        schemata
            .Where(x => x.Value.Search is not null)
            .Select(x => (x.Key, TryGetFor(Target.Search, x.Key)))
            .Where(x => x.Item2 is not null)!;

    public Func<Expression, Expression, Expression> GetSearchTransformer(string field) =>
        customSearchTransformers.TryGetValue(field, out var transformer)
            ? transformer
            : defaultSearch;

    public Type GetType(string field) => schemata[field].Type;

    public Func<ParameterExpression, Expression>? TryGetFor(Target target, string field)
    {
        if (!schemata.TryGetValue(field, out var fieldSchema))
            return null;

        var transformer = target switch
        {
            Target.Filter => fieldSchema.Filter,
            Target.Search => fieldSchema.Search,
            Target.Sort => fieldSchema.Sort,
            Target.Group => fieldSchema.Group,
            _ => throw new ArgumentOutOfRangeException(nameof(target), "Target is unknown"),
        };

        if (transformer is null) return null;

        if (!customizations.TryGetValue(field, out var c))
            return transformer;

        var t2 = target switch
        {
            Target.Filter => c.Filter,
            Target.Search => c.Search,
            Target.Sort => c.Sort,
            Target.Group => c.Group,
            _ => throw new ArgumentOutOfRangeException("not reachable"),
        };

        return t2 is null ? transformer : (p => t2(transformer(p)));
    }
}
