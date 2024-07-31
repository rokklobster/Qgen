namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EnableSortingAttribute : EnableFeaturesAttribute
    {
        public EnableSortingAttribute(DefaultSorting defaultSortingField = DefaultSorting.None)
        {
            EnableSorting = true;
            DefaultSortingDirection = defaultSortingField;
        }
    }
}
