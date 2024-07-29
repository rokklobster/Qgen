namespace Qgen.Contracts.Models
{
    public class FilterComposition
    {
        public CompositionOp Op { get; init; }

        public FilterComposition[]? Compositions { get; init; }

        public Filter[]? Filters { get; init; }
    }
}
