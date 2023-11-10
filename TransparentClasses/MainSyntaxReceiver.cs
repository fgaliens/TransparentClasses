using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace TransparentClasses
{
    internal sealed class MainSyntaxReceiver : ISyntaxReceiver
    {
        private readonly ISyntaxReceiver[] _receivers;

        public MainSyntaxReceiver()
        {
            ClassDeclarationSyntaxReceiver = new();

            _receivers = new ISyntaxReceiver[] 
            { 
                ClassDeclarationSyntaxReceiver
            };
        }
        
        public ClassDeclarationSyntaxReceiver ClassDeclarationSyntaxReceiver { get; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            foreach (var receiver in _receivers)
            {
                receiver.OnVisitSyntaxNode(syntaxNode);
            }
        }
    }
}
