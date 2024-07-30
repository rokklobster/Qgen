namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EnableFilteringAttribute : EnableFeaturesAttribute
    {
        public EnableFilteringAttribute()
        {
            EnableFiltering = true;
        }
    }
}
