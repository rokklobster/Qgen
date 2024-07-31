namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EnableGroupingAttribute : EnableFeaturesAttribute
    {
        public EnableGroupingAttribute()
        {
            EnableGrouping = true;
        }
    }
}
