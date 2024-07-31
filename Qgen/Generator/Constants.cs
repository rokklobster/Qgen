using System;
using System.Linq;
using Qgen.Contracts.Services;
using Qgen.Declarations;
using Qgen.Services;

namespace Qgen.Generator;

public static class Constants
{
    public static readonly Type SchemaTType = typeof(Schema<>);
    public static readonly string SchemaTFullName = SchemaTType.FullName.Substring(0, SchemaTType.FullName.IndexOf('`'));

    public static readonly string DefaultRepoTypeName = typeof(DefaultSchemaRepo).FullName;

    public static readonly string SchemaBuilderResult = nameof(SchemaBuilder<int>.Result);
    public static readonly Type SchemaBuilderType = typeof(SchemaBuilder<>);
    public static readonly string SchemaBuilderFullName = SchemaBuilderType.FullName.Substring(0, SchemaBuilderType.FullName.Length - 2);

    public static readonly string SbldRegisterField = nameof(SchemaBuilder<int>.RegisterField);
    public static readonly string SbldEnableAll = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableAll);
    public static readonly string SbldEnableFilter = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableFiltering);
    public static readonly string SbldEnableSearch = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableSearching);
    public static readonly string SbldEnableSort = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableSorting);
    public static readonly string SbldEnableGroup = nameof(SchemaBuilder<int>.FieldBuilder<int>.EnableGrouping);
    public static readonly string AddDefaultSort = nameof(SchemaBuilder<int>.AddDefaultOrderingLayer);

    public static readonly string[] EnableAllNames = GetAttributeNames<EnableAllFeaturesAttribute>();

    public static readonly string[] EnableFilteringNames = GetAttributeNames<EnableFilteringAttribute>();

    public static readonly string[] EnableSearchingNames = GetAttributeNames<EnableSearchingAttribute>();

    public static readonly string[] EnableSortingNames = GetAttributeNames<EnableSortingAttribute>();

    public static readonly string[] EnableGroupingNames = GetAttributeNames<EnableGroupingAttribute>();

    private static string[] GetAttributeNames<T>() where T : Attribute
    {
        var lenAttr = nameof(Attribute).Length;

        var parts = typeof(T).FullName.Split('.');
        return parts.Select((_, i) => string.Join(".", parts.Skip(i)))
            .SelectMany(x => new[] { x, x.Substring(0, x.Length - lenAttr) })
            .ToArray();
    }
}
