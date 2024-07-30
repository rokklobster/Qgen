namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EnableGroupingAttribute : EnableFeaturesAttribute
    {
        public EnableGroupingAttribute()
        {
            EnableGrouping = true;
        }
    }
}
