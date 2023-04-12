using System;
using System.Reflection;

namespace DarkLink.Delegation;

public class LazyDelegate<T> : IPropertyDelegate<T>
{
    private readonly Lazy<T> lazy;

    public LazyDelegate(Func<T> getValue)
    {
        lazy = new Lazy<T>(getValue);
    }

    public T Get(object? thisRef, PropertyInfo property) => lazy.Value;

    public void Set(object? thisRef, PropertyInfo property, T value) { }
}
