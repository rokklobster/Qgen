namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EnableFilteringAttribute : EnableFeaturesAttribute
{
    public EnableFilteringAttribute()
    {
        EnableFiltering = true;
    }
}
