namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EnableSearchingAttribute : EnableFeaturesAttribute
{
    public EnableSearchingAttribute()
    {
        EnableSearching = true;
    }
}
