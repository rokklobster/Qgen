namespace Qgen.Declarations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class EnableFeaturesAttribute : Attribute
    {
        public bool EnableFiltering { get; set; }
        public bool EnableSorting { get; set; }
        public bool EnableSearching { get; set; }
        public bool EnableGrouping { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EnableFilteringAttribute : EnableFeaturesAttribute
    {
        public EnableFilteringAttribute()
        {
            EnableFiltering = true;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EnableSortingAttribute : EnableFeaturesAttribute
    {
        /// <summary>
        /// if set to true, all queries will be enriched with ordering on this field. only one property should have this set.
        /// </summary>
        public bool DefaultSortingField { get; set; }

        public EnableSortingAttribute(bool defaultSortingField = false)
        {
            EnableSorting = true;
            DefaultSortingField = defaultSortingField;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EnableSearchingAttribute : EnableFeaturesAttribute
    {
        public EnableSearchingAttribute()
        {
            EnableSearching = true;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class EnableGroupingAttribute : EnableFeaturesAttribute
    {
        public EnableGroupingAttribute()
        {
            EnableGrouping = true;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequiresSchemaAttribute : Attribute
    {

    }
}
