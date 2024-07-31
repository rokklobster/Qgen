using Microsoft.CodeAnalysis;

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

#if DEBUG_GEN
            if (!Debugger.IsAttached)
                Debugger.Launch();
#endif

            var processor = new AssemblyProcessor(context.Compilation, context.AddSource);
            processor.Generate(context.CancellationToken);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // not used
        }


    }
}
