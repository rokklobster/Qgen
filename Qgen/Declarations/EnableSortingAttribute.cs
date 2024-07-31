namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EnableSortingAttribute : EnableFeaturesAttribute
    {
        public EnableSortingAttribute(DefaultSorting defaultSortingField = DefaultSorting.None)
        {
            EnableSorting = true;
            DefaultSortingDirection = defaultSortingField;
        }
    }
}
