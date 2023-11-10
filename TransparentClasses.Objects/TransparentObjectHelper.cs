#nullable enable
using System.Reflection;

namespace TransparentClasses
{
    public static class TransparentObjectHelper
    {
        public static object? ExecuteMethod<T>(T? obj, string methodName, object[] args)
        {
            var getMethodFlags = BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.InvokeMethod
                | BindingFlags.Public
                | BindingFlags.NonPublic;

            var objectType = typeof(T);
            var method = objectType.GetMethod(methodName, getMethodFlags);

            return method!.Invoke(obj, args);
        }
    }
}
