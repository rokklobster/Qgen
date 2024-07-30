namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EnableSearchingAttribute : EnableFeaturesAttribute
    {
        public EnableSearchingAttribute()
        {
            EnableSearching = true;
        }
    }
}
