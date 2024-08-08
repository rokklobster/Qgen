namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class DefaultSortPropertyAttribute : EnableFeaturesAttribute
{
    public DefaultSortPropertyAttribute(DefaultSorting defaultSortingField = DefaultSorting.Asc)
    {
        DefaultSortingDirection = defaultSortingField;
    }
}
