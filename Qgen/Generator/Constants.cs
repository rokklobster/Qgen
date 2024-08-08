using Qgen.Contracts.Services;
using Qgen.Declarations;
using Qgen.Services;

namespace Qgen.Generator;

public static class Constants
{
    public static readonly string SchemaRepoTypeName = typeof(SchemaRepo).FullName;

    public static readonly Type SchemaTType = typeof(Schema<>);
    public static readonly string SchemaTFullName = SchemaTType.FullName.Substring(0, SchemaTType.FullName.IndexOf('`'));

    public static readonly string DefaultRepoTypeName = typeof(DefaultSchemaRepo).FullName;

    public const string SchemaBuilderResult = nameof(SchemaBuilder<int>.Result);
    public static readonly Type SchemaBuilderType = typeof(SchemaBuilder<>);
    public static readonly string SchemaBuilderFullName = SchemaBuilderType.FullName.Substring(0, SchemaBuilderType.FullName.Length - 2);

    public static readonly string[] SearchWithNames = GetAttributeNames<SearchWithAttribute>();
    public static readonly string[] DefaultOrderingNames = GetAttributeNames<DefaultSortPropertyAttribute>();

    public const string SbldRegisterField = nameof(SchemaBuilder<int>.RegisterField);
    public const string SbldEnableAll = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableAll);
    public const string SbldEnableFilter = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableFiltering);
    public const string SbldEnableSearch = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableSearching);
    public const string SbldEnableSearchWith = nameof(SchemaBuilder<int>.FieldBuilder<int>.UseCustomSearch);
    public const string SbldEnableSort = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableSorting);
    public const string SbldEnableGroup = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableGrouping);
    public const string AddDefaultSort = nameof(SchemaBuilder<int>.AddDefaultOrderingLayer);

    public static readonly string[] EnableAllNames = GetAttributeNames<EnableAllFeaturesAttribute>();

    public static readonly string[] EnableFilteringNames = GetAttributeNames<EnableFilteringAttribute>();

    public static readonly string[] EnableSearchingNames = GetAttributeNames<EnableSearchingAttribute>();

    public static readonly string[] EnableSortingNames = GetAttributeNames<EnableSortingAttribute>();

    public static readonly string[] EnableGroupingNames = GetAttributeNames<EnableGroupingAttribute>();

    public const string DisableMethod = nameof(SchemaBuilder<int>.FieldBuilder<int>.Disable);

    public static readonly Dictionary<string, string> DisablingMapping = new() {
        {nameof(EnableFeaturesAttribute.EnableFiltering), "filtering" },
        {nameof(EnableFeaturesAttribute.EnableSearching), "searching" },
        {nameof(EnableFeaturesAttribute.EnableSorting), "sorting" },
        {nameof(EnableFeaturesAttribute.EnableGrouping), "grouping" },
    };

    private static string[] GetAttributeNames<T>() where T : Attribute
    {
        var lenAttr = nameof(Attribute).Length;

        var parts = typeof(T).FullName.Split('.');
        return parts.Select((_, i) => string.Join(".", parts.Skip(i)))
            .SelectMany(x => new[] { x, x.Substring(0, x.Length - lenAttr) })
            .ToArray();
    }
}
