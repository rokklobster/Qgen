namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EnableFilteringAttribute : EnableFeaturesAttribute
    {
        public EnableFilteringAttribute()
        {
            EnableFiltering = true;
        }
    }
}
