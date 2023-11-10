using Microsoft.CodeAnalysis;

namespace TransparentClasses.Extensions
{
    public static class SeparatedSyntaxListExtensions
    { 
        public static SeparatedSyntaxList<T> AddNode<T>(this ref SeparatedSyntaxList<T> values, T node) where T : SyntaxNode
        {
            values = values.Add(node);
            return values;
        }

        public static SeparatedSyntaxList<T> AddRange<T>(this ref SeparatedSyntaxList<T> values, params T[] nodes) where T : SyntaxNode
        {
            values = values.AddRange(nodes);
            return values;
        }
    }
}
