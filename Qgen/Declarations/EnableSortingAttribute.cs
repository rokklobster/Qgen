namespace Qgen.Declarations
{
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
}
