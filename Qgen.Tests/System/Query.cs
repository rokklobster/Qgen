using Qgen.Contracts.Models;

namespace Qgen.Tests.System
{
    public record Query(
        string[]? Include = null,
        FilterComposition? Filters = null,
        string? SearchQuery = null,
        Ordering[]? OrderBy = null,
        string[]? GroupBy = null,
        uint Skip = 0, uint Take = 2000) : QueryModel;
}
