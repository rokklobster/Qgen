namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class SearchWithAttribute : EnableFeaturesAttribute
{
    public SearchWithAttribute(string? method = null, Type? type = null)
    {
        EnableSearching = true;
        if (!string.IsNullOrWhiteSpace(method))
            SearchReader = new(type, method!);
    }
}
