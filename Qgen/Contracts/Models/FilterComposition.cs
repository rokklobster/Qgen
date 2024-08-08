namespace Qgen.Contracts.Models;

public class FilterComposition
{
    public FilterComposition(CompositionOp op, FilterComposition[]? compositions = null, Filter[]? filters = null)
    {
        Op = op;
        Compositions = compositions;
        Filters = filters;
    }

    public CompositionOp Op { get; }

    public FilterComposition[]? Compositions { get; }

    public Filter[]? Filters { get; }
}
