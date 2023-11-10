using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

namespace TransparentClasses
{
    [Generator]
    public class TransparentClassGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForPostInitialization(ctx =>
            {
                ctx.AddSource("TransparentObject.g.cs", AdditionalClassTexts.TransparentObject);
            });
            context.RegisterForSyntaxNotifications(() => new MainSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var cancellationToken = context.CancellationToken;

            var syntaxReciver = context.SyntaxReceiver as MainSyntaxReceiver;
            var classes = syntaxReciver.ClassDeclarationSyntaxReceiver.ClassDeclarations;

            var transparentObjectClass = context.Compilation.GetTypeByMetadataName("TransparentClasses.TransparentObject`1");


            foreach (var classDeclaration in classes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var semanticModel = context.Compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var targetType = semanticModel.GetDeclaredSymbol(classDeclaration);

                var baseType = targetType.BaseType;
                var symbolComparer = SymbolEqualityComparer.Default;

                if (baseType is not null && symbolComparer.Equals(baseType.ConstructedFrom, transparentObjectClass))
                {
                    // Inherited from TransparentClasses.TransparentObject`1
                    if (targetType.IsGenericType)
                    {
                        var error = DiagnosticDescriptorHelper.CreateWarning("Target type can not be generic", 1);
                        var location = Location.Create(classDeclaration.SyntaxTree, classDeclaration.Span);
                        context.ReportDiagnostic(Diagnostic.Create(error, location));
                        continue;
                    }

                    //if (string.IsNullOrEmpty(targetType.ContainingNamespace.Name))
                    //{
                    //    var error = DiagnosticDescriptorHelper.CreateWarning("Target type must be in namespace", 2);
                    //    var location = Location.Create(classDeclaration.SyntaxTree, classDeclaration.Span);
                    //    context.ReportDiagnostic(Diagnostic.Create(error, location));
                    //    continue;
                    //}

                    // Check
                    var typeToMakeTransparent = baseType.TypeArguments[0];

                    //Debugger.Launch();

                    var node = TransparentClassBuilder.GenerateClass(typeToMakeTransparent, targetType, baseType);
                    var text = node.ToString();

                    context.AddSource($"{typeToMakeTransparent.Name}.Transparent.g.cs", text);
                }
            }
        }
    }
}
