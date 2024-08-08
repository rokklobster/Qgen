namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EnableGroupingAttribute : EnableFeaturesAttribute
{
    public EnableGroupingAttribute(string? method = null, Type? type = null)
    {
        EnableGrouping = true;
        if (!string.IsNullOrWhiteSpace(method))
            GroupReader = new(type, method!);
    }
}
