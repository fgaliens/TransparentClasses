using Microsoft.CodeAnalysis;

namespace TransparentClasses
{
    public static class DiagnosticDescriptorHelper
    {
        public static DiagnosticDescriptor CreateWarning(string message, int code)
        {
            var id = string.Format("{0:4}", code);

            return new DiagnosticDescriptor(
                id: $"TC{id}",
                title: "Transparent Class",
                messageFormat: $"Transparent class won`t be created. {message}",
                category: "TransparentClasses",
                DiagnosticSeverity.Warning,
                isEnabledByDefault: true);
        }
    }
}
