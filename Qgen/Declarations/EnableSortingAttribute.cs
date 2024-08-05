namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EnableSortingAttribute : EnableFeaturesAttribute
{
    public EnableSortingAttribute(string? method = null, Type? type = null)
    {
        EnableSorting = true;
        if (!string.IsNullOrWhiteSpace(method))
            SortReader = new(type, method!);
    }
}
