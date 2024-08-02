using Qgen.Declarations;
using Qgen.Tests.System.Impls;

namespace Qgen.Tests.System.DB;

[RequiresSchema]
public class TestEntityDecl
{
    [EnableAllFeatures(DefaultSorting.Desc)]
    public Guid Id { get; set; }

    [EnableAllFeatures]
    public string? Name { get; set; }

    [EnableFiltering(nameof(Transformers.Abs), typeof(Transformers))]
    [EnableSearching]
    [EnableGrouping]
    public int Code { get; set; }
}
