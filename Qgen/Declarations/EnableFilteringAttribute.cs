namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class EnableFilteringAttribute : EnableFeaturesAttribute
{
    public EnableFilteringAttribute(string? method = null, Type? type = null)
    {
        EnableFiltering = true;
        if (!string.IsNullOrWhiteSpace(method))
            FilterReader = new(type, method!);
    }
}
