﻿using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Qgen.Declarations;
using Qgen.Services.Internal;
using static Qgen.Generator.Constants;

namespace Qgen.Generator;

public class AssemblyProcessor
{
    private readonly Compilation compilation;
    private readonly Action<string, string> registerSource;
    private readonly string generatedNamespace;
    private readonly List<string> generatedEntriesFqn = new();

    public AssemblyProcessor(Compilation compilation, Action<string, string> registerSource)
    {
        this.compilation = compilation;
        this.registerSource = registerSource;

        generatedNamespace = compilation.AssemblyName + ".Generated.Schema";
    }

    public void Generate(CancellationToken canx)
    {
        foreach (var st in compilation.SyntaxTrees)
        {
            if (canx.IsCancellationRequested)
                return;

            if (!st.TryGetRoot(out var node))
                continue;

            var allClasses = node.DescendantNodes()
                .Where(RequiresGeneration)
                .OfType<ClassDeclarationSyntax>();

            foreach (var n in allClasses)
            {
                var ns = node.DescendantNodes()
                    .Where(n => n.IsKind(SyntaxKind.NamespaceDeclaration) || n.IsKind(SyntaxKind.FileScopedNamespaceDeclaration))
                    .Select(n => (n as NamespaceDeclarationSyntax)?.Name ?? (n as FileScopedNamespaceDeclarationSyntax)?.Name)
                    .Pipe(c =>
                    {
                        try { return c.Single(); }
                        catch (InvalidOperationException e) when (e.Message.Contains("Sequence contains"))
                        {
                            throw new InvalidOperationException("Generator requires annotated classes to be present in files containing only one namespace", e);
                        }
                    })
                    ?.ToString();
                var usings = node.DescendantNodes().OfType<UsingDirectiveSyntax>().Select(x => x.ToString()).Pipe(string.Join, "\n");

                if (ns is null) continue;

                CreateSchema(n, ns, usings);
            }
        }

        CreateRepo();
    }

    private bool RequiresGeneration(SyntaxNode node)
    {
        return node.IsKind(SyntaxKind.ClassDeclaration)
        && node is ClassDeclarationSyntax cds
        && cds.AttributeLists.Any(x => x.Attributes.Any(a => a.Name.ToString().Contains("RequiresSchema")));
    }

    private bool IsPropertySuitableForRegistering(SyntaxNode node)
    {
        if (node is not PropertyDeclarationSyntax pds)
            return false;
        var flatAttrs = pds.AttributeLists.SelectMany(x => x.Attributes).Select(x => x.GetText().ToString()).ToArray();
        return flatAttrs.Any(x => x.Contains("Enable"));
    }

    private void CreateRepo()
    {
        var sb = new StringBuilder();
        var repoName = compilation.AssemblyName!.Replace(".", "") + "Repo";
        var sb2 = new StringBuilder();
        generatedEntriesFqn.ForEach(e => sb2.AppendFormat("\t\t\tRegister({0}.Schema);\n", e));

        sb.AppendFormat(@"namespace {0} {{
    public partial class {1} : {2} {{
        private static readonly Lazy<{1}> instance = new Lazy<{1}>();
        public static {4} Instance => instance.Value;

        public {1}() {{
{3}
            ManualRegister();
        }}
    }}
}}",
        generatedNamespace, repoName, DefaultRepoTypeName, sb2, SchemaRepoTypeName);

        registerSource(repoName + ".g.cs", sb.ToString());
    }

    /// <summary>
    /// generate schema, save name of schema to list, add file to output
    /// </summary>
    /// <param name="st"></param>
    private void CreateSchema(ClassDeclarationSyntax st, string ns, string usings)
    {
        var sb = new StringBuilder();
        var name = st.Identifier + "SchemaContainer";
        var target = st.Identifier.ToString();

        sb.AppendFormat(@"
{6}
using {5};

namespace {0} {{
    public partial class {1} {{
        private static readonly Lazy<{2}<{4}>> schema = new(Build);
        public static {2}<{4}> Schema => schema.Value;
        
        private static {2}<{4}> Build() {{
            var res = new {3}<{4}>();

",
        generatedNamespace, name, SchemaTFullName, SchemaBuilderFullName, target, ns, usings);

        foreach (var p in st.DescendantNodes().Where(IsPropertySuitableForRegistering).OfType<PropertyDeclarationSyntax>())
        {
            var attrs = p.AttributeLists.SelectMany(x => x.Attributes);
            AddRegisteringExpressions(p, attrs, sb);
        }

        sb.Append(@$"
            ManualRegister(res);
            return res.{SchemaBuilderResult};
        }}

        static partial void ManualRegister({SchemaBuilderFullName}<{target}> builder);
    }}
}}
");

        var fqn = generatedNamespace + "." + name;
        generatedEntriesFqn.Add(fqn);
        registerSource(name + ".g.cs", sb.ToString());
    }

    private void AddRegisteringExpressions(PropertyDeclarationSyntax p, IEnumerable<AttributeSyntax> attrs, StringBuilder sb)
    {
        var attrNames = attrs.Select(a => (a, a.Name.GetText().ToString())).ToArray();
        var propName = p.Identifier.ToString();

        if (GetSingleContaining(attrNames, EnableAllNames) is { } a)
        {
            sb.AppendFormat("\t\t\tres.{0}(t => t.{1}).{2}();\n",
                SbldRegisterField, propName, SbldEnableAll);

            if (a.ArgumentList?.Arguments.Select(x => x.GetText().ToString()).First(x => !x.Contains(nameof(DefaultSorting.None))) is { } s)
                sb.AppendFormat("\t\t\tres.{0}(t => t.{1}, {2});\n",
                AddDefaultSort, propName, s.Contains(nameof(DefaultSorting.Asc)).ToString().ToLower());
        }
        else
        {
            AttributeSyntax?
              filter = GetSingleContaining(attrNames, EnableFilteringNames),
              search = GetSingleContaining(attrNames, EnableSearchingNames),
              sort = GetSingleContaining(attrNames, EnableSortingNames),
              group = GetSingleContaining(attrNames, EnableGroupingNames);

            if (filter is null && search is null && sort is null && group is null) return;

            sb.AppendFormat("\t\t\tres.{0}(t => t.{1})", SbldRegisterField, propName);
            // todo: consider customizations

            if (filter is not null)
                sb.AppendFormat("\n\t\t\t\t.{0}({1})", SbldEnableFilter, GetArgs(filter));

            if (search is not null)
                sb.AppendFormat("\n\t\t\t\t.{0}()", SbldEnableSearch);

            if (sort is not null)
                sb.AppendFormat("\n\t\t\t\t.{0}()", SbldEnableSort);

            if (group is not null)
                sb.AppendFormat("\n\t\t\t\t.{0}()", SbldEnableGroup);

            sb.AppendLine(";");
        }
    }

    private string GetArgs(AttributeSyntax a, params string[] args)
    {
        if (a.ArgumentList?.Arguments.Count > 0)
        {
            // expecting method and type
            var attrArgs = a.ArgumentList.Arguments.Select(x => x.Expression).ToArray();

            var res = attrArgs.Reverse()
                .Select(x =>
                {
                    /*strip all unnecessary*/
                    if (x is TypeOfExpressionSyntax tos)
                        return tos.Type.ToString();
                    else if (x is LiteralExpressionSyntax les)
                        return x.ToString().Trim('"');
                    else if (x is InvocationExpressionSyntax iex && iex.Expression is IdentifierNameSyntax ins && ins.Identifier.Text == "nameof")
                        return iex.ArgumentList.Arguments[0].ToString().Pipe(s => s.Substring(s.LastIndexOf('.') + 1));
                    return x.ToString();
                })
                .Pipe(string.Join, ".");

            return res;
        }

        return "";
    }

    private AttributeSyntax? GetSingleContaining((AttributeSyntax, string)[] attrs, string[] targets) =>
        attrs.SingleOrDefault(p => targets.Contains(p.Item2)).Item1;
}
