namespace TransparentClasses
{
    internal static class AdditionalClassTexts
    {
        public static string TransparentObject => """
            using global::System;
            using global::System.Collections.Generic;
            using global::System.Reflection;

            namespace TransparentClasses
            {
                public partial class TransparentObject<T>
                {
                    private static readonly Dictionary<string, MethodInfo> _cachedMethods = new();
                    private T? _object;
                    private bool _initialized;

                    public TransparentObject()
                    { }

                    public TransparentObject(T obj)
                    {
                        Object = obj;
                    }

                    public T Object
                    {
                        get => _initialized ? _object! : throw new InvalidOperationException($"Property '{nameof(Object)}' is not initialized");
                        set
                        {
                            _initialized = value is not null;
                            _object = value;
                        }
                    }

                    protected static object? ExecuteMethod(T? obj, string methodName, Type[] genericTypes, object[] args)
                    {
                        if (_cachedMethods.TryGetValue(methodName, out var cachedMethod))
                        {
                            return cachedMethod.Invoke(obj, args);
                        }

                        var getMethodFlags = BindingFlags.Instance
                            | BindingFlags.Static
                            | BindingFlags.InvokeMethod
                            | BindingFlags.Public
                            | BindingFlags.NonPublic;

                        var objectType = typeof(T);
                        var method = objectType.GetMethod(methodName, getMethodFlags);

                        if (method is null)
                        {
                            throw new InvalidOperationException($"Method '{methodName}' is not found in type '{typeof(T)}'");
                        }

                        if (genericTypes.Length > 0)
                        {
                            method = method.MakeGenericMethod(genericTypes);
                        }
                        else
                        {
                            _cachedMethods.Add(methodName, method);
                        }

                        return method.Invoke(obj, args);
                    }
                }
            }
            """;
    }
}
