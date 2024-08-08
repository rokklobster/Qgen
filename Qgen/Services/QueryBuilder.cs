using Qgen.Contracts.Models;
using Qgen.Contracts.Services;
using Qgen.Services.Internal;

namespace Qgen.Services;

public class QueryBuilder<T> where T : class
{
    private readonly SetProvider setProvider;
    private readonly SchemaRepo schemaRepo;

    public QueryBuilder(SetProvider setProvider, SchemaRepo schemaRepo)
    {
        this.setProvider = setProvider;
        this.schemaRepo = schemaRepo;
    }

    /// <summary>
    /// builde query specified by request 
    /// </summary>
    /// <returns><see cref="IQueryable{T}"/> containing expression to execute against your data provider</returns>
    /// <exception cref="NullReferenceException">Schema is not built for requested type</exception>
    public IQueryable<T> GetFrom(QueryModel request)
    {
        var schema = schemaRepo.Get<T>();

        if (schema is null)
        {
            throw new NullReferenceException(nameof(schema));
        }

        var source = setProvider.Get<T>();

        var res = Filter(request, source, schema);
        res = Sort(request, res, schema);
        res = Search(request, res, schema);
        res = Page(request, res);
        //res = Group(request, res, schema);

        return res;
    }

    private IQueryable<T> Filter(QueryModel request, IQueryable<T> source, Schema<T> schema) =>
        source.Where(FilterBuilder.Build(request.Filters, schema));

    private IQueryable<T> Sort(QueryModel request, IQueryable<T> source, Schema<T> schema) =>
        SortBuilder.Build(source, schema, request.OrderBy);

    private IQueryable<T> Search(QueryModel request, IQueryable<T> source, Schema<T> schema) =>
        source.Where(SearchBuilder.Build(request.SearchQuery, request.Include, schema));

    private IQueryable<T> Group(QueryModel request, IQueryable<T> source, Schema<T> schema)
    {
        throw new NotImplementedException();
    }

    private IQueryable<T> Page(QueryModel request, IQueryable<T> source) =>
        source.Skip((int)request.Skip).Take((int)request.Take);
}