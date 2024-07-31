using Qgen.Declarations;

namespace Qgen.Tests.System
{
    [RequiresSchema]
    public class TestEntityDecl
    {
        [EnableAllFeatures(DefaultSorting.Desc)]
        public Guid Id { get; set; }

        [EnableAllFeatures]
        public string? Name { get; set; }

        [EnableFiltering]
        [EnableSearching]
        [EnableGrouping]
        public int Code { get; set; }
    }
}
