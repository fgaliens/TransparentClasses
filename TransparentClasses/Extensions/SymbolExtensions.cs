using Microsoft.CodeAnalysis;

namespace TransparentClasses.Extensions
{
    public static class SymbolExtensions
    {
        public static string GetFullName(this ITypeSymbol symbol)
        {
            if (string.IsNullOrEmpty(symbol.ContainingNamespace.Name))
            {
                return symbol.Name;
            }
            else
            {
                return $"{symbol.ContainingNamespace}.{symbol.Name}";
            }
        }
    }
}
