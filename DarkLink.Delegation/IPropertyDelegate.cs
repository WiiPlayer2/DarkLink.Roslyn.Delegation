using System;
using System.Reflection;

namespace DarkLink.Delegation;

public interface IPropertyDelegate<T>
{
    T Get(object? thisRef, PropertyInfo property);

    void Set(object? thisRef, PropertyInfo property, T value);
}
