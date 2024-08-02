using Microsoft.CodeAnalysis;

namespace Qgen.Generator;

[Generator]
public class SchemaGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
#if DEBUG
         if (!System.Diagnostics.Debugger.IsAttached)
             System.Diagnostics.Debugger.Launch();
#endif

        var processor = new AssemblyProcessor(context.Compilation, context.AddSource);
        processor.Generate(context.CancellationToken);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        // not used
    }


}
