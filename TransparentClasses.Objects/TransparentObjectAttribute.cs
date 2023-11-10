#nullable enable
using System;

namespace TransparentClasses
{
    public class TransparentObjectAttribute : Attribute
    {
        public TransparentObjectAttribute(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public Type Type { get; }
    }

    public class TransparentObjectAttribute<T> : TransparentObjectAttribute
    {
        public TransparentObjectAttribute() : base(typeof(T)) { }
    }
}
