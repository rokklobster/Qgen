namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class EnableSearchingAttribute : EnableFeaturesAttribute
    {
        public EnableSearchingAttribute()
        {
            EnableSearching = true;
        }
    }
}
