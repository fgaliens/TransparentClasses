#nullable enable
using System;
using System.Reflection;
using TransparentClasses.Objects;

namespace TransparentClasses
{
    public partial class TransparentObject<T> : ITransparentObject<T>
    {
        public TransparentObject(T obj)
        {
            Object = obj ?? throw new ArgumentNullException(nameof(obj));
        }

        public T Object { get; }

        private static object? ExecuteMethod(T? obj, string methodName, object[] args)
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
