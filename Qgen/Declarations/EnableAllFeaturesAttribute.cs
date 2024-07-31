namespace Qgen.Declarations
{
    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
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
}
