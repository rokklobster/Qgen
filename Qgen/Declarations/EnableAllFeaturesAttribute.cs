namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EnableAllFeaturesAttribute : EnableFeaturesAttribute
{
    public EnableAllFeaturesAttribute(DefaultSorting defaultSortingDirection = DefaultSorting.None)
    {
        EnableFiltering = true;
        EnableGrouping = true;
        EnableSearching = true;
        EnableSorting = true;
        DefaultSortingDirection = defaultSortingDirection;
    }
}
