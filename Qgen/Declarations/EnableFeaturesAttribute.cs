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
}
