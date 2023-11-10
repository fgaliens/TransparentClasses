using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using TransparentClasses.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace TransparentClasses
{
    public static class TransparentClassBuilder
    {
        public static SyntaxNode GenerateClass(ITypeSymbol sourceType, ITypeSymbol targetType, ITypeSymbol baseType)
        {
            var compilationUnitUsings = new SyntaxList<UsingDirectiveSyntax>()
                .Add(UsingDirective(IdentifierName(baseType.ContainingNamespace.Name)));
            var unit = CompilationUnit()
                .WithUsings(compilationUnitUsings);

            var namespaceNode = NamespaceDeclaration(ParseName(targetType.ContainingNamespace.Name));

            var classModifiers = TokenList(
                Token(SyntaxKind.PublicKeyword),
                Token(SyntaxKind.PartialKeyword));

            var baseTypeGenericIdentifier = new SeparatedSyntaxList<TypeSyntax>().Add(IdentifierName(sourceType.GetFullName()));
            var classBase = new SeparatedSyntaxList<BaseTypeSyntax>()
                .Add(SimpleBaseType(GenericName(ParseToken(baseType.Name), TypeArgumentList(baseTypeGenericIdentifier))));

            var classMemberModifiers = TokenList(Token(SyntaxKind.PublicKeyword));

            var classNode = ClassDeclaration(targetType.Name)
                .WithModifiers(classModifiers)
                .WithBaseList(BaseList(classBase));

            var methodsToDeclare = sourceType
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(x => x.MethodKind == MethodKind.Ordinary);

            foreach (var method in methodsToDeclare)
            {
                var parametersList = new SeparatedSyntaxList<ParameterSyntax>();
                foreach (var parameter in method.Parameters)
                {
                    var parameterNode = Parameter(Identifier(parameter.Name))
                        .WithType(ParseTypeName(parameter.Type.GetFullName()));
                    parametersList = parametersList.Add(parameterNode);
                }

                var typeParametersSyntax = method.TypeParameters.Select(x => TypeParameter(x.Name));

                TypeParameterListSyntax? typeParametersList = !method.IsGenericMethod ? null : TypeParameterList(
                    new SeparatedSyntaxList<TypeParameterSyntax>().AddRange(typeParametersSyntax));

                var returnTypeNode = method.ReturnsVoid ? PredefinedType(Token(SyntaxKind.VoidKeyword)) : ParseTypeName(method.ReturnType.GetFullName());

                MethodDeclarationSyntax methodNode = MethodDeclaration(
                    attributeLists: List<AttributeListSyntax>(),
                    modifiers: classMemberModifiers,
                    returnType: returnTypeNode,
                    explicitInterfaceSpecifier: null,
                    identifier: Identifier(method.Name),
                    typeParameterList: typeParametersList,
                    parameterList: ParameterList(parametersList),
                    constraintClauses: List<TypeParameterConstraintClauseSyntax>(),
                    body: Block(GenerateBlockStatetments(method)),
                    expressionBody: null);

                classNode = classNode.AddMembers(methodNode);
            }

            if (!string.IsNullOrEmpty(targetType.ContainingNamespace.Name))
            {
                namespaceNode = namespaceNode.AddMembers(classNode);
                unit = unit.AddMembers(namespaceNode);
            }
            else
            {
                unit = unit.AddMembers(classNode);
            }

            //return namespaceNode.NormalizeWhitespace();
            return unit.NormalizeWhitespace();
        }

        private static IEnumerable<StatementSyntax> GenerateBlockStatetments(IMethodSymbol method)
        {
            var executeMethodParametersList = new SeparatedSyntaxList<ArgumentSyntax>().AddRange(new[]
            {
                Argument(IdentifierName("Object")),
                Argument(LiteralExpression(SyntaxKind.StringLiteralExpression, ParseToken(method.Name.WithQuotes()))),
            });

            if (method.IsGenericMethod)
            {
                var typeofExpressions = new SeparatedSyntaxList<ExpressionSyntax>().AddRange(
                    method.TypeParameters.Select(x => TypeOfExpression(IdentifierName(x.GetFullName()))));

                var arrayInit = ArrayCreationExpression(
                    ArrayType(IdentifierName("System.Type[]".WithGlobalNamespace())),
                    InitializerExpression(SyntaxKind.ArrayInitializerExpression, typeofExpressions));

                executeMethodParametersList = executeMethodParametersList.Add(Argument(arrayInit));
            }
            else
            {
                var typeIdentifier = new SeparatedSyntaxList<TypeSyntax>().Add(IdentifierName("System.Type".WithGlobalNamespace()));

                var invocationExpression = InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression, IdentifierName("System.Array".WithGlobalNamespace()),
                    GenericName(ParseToken("Empty"), TypeArgumentList(typeIdentifier))));

                executeMethodParametersList = executeMethodParametersList.Add(Argument(invocationExpression));
            }

            if (method.Parameters.Length > 0)
            {
                var argumentExpressions = new SeparatedSyntaxList<ExpressionSyntax>().AddRange(
                    method.Parameters.Select(x => IdentifierName(x.Name)));

                var arrayInit = ArrayCreationExpression(
                    ArrayType(IdentifierName("System.Object[]".WithGlobalNamespace())),
                    InitializerExpression(SyntaxKind.ArrayInitializerExpression, argumentExpressions));

                executeMethodParametersList = executeMethodParametersList.Add(Argument(arrayInit));
            }
            else
            {
                var objectIdentifier = new SeparatedSyntaxList<TypeSyntax>().Add(IdentifierName("System.Object".WithGlobalNamespace()));

                var arrayInvocationExpression = InvocationExpression(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression, IdentifierName("System.Array".WithGlobalNamespace()),
                    GenericName(ParseToken("Empty"), TypeArgumentList(objectIdentifier))));

                executeMethodParametersList = executeMethodParametersList.Add(Argument(arrayInvocationExpression));
            }

            var executeMethodInvocation = InvocationExpression(
                IdentifierName("ExecuteMethod"),
                ArgumentList(executeMethodParametersList));

            if (!method.ReturnsVoid)
            {

                yield return ReturnStatement(CastExpression(
                    IdentifierName(method.ReturnType.GetFullName()),
                    executeMethodInvocation));
            }
            else
            {
                yield return ExpressionStatement(executeMethodInvocation);
            }

        }
    }
}
