using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace Qgen.Generator
{
    [Generator]
    public class SchemaGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // todo: enumerate types; find ones decorated with RequiresSchema; parse all decorated props;
            // generate schema using SchemaBuilder; generate schema repo derived from DefaultSchemaRepo;
            // in ctor, collect all schemata

            context.AddSource("Test.g.cs", "namespace Test { public static class TestClass { public static void M() { System.Console.WriteLine(\"hi!\") } } }");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // not used
        }
    }
}
