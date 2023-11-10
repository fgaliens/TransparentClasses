namespace TransparentClasses.Extensions
{
    public static class StringExtensions
    {
        public static string WithQuotes(this string text)
        {
            return $"\"{text}\"";
        }

        public static string WithGlobalNamespace(this string text)
        {
            return $"global::{text}";
        }
    }
}
