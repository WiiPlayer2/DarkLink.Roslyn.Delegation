using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DarkLink.Delegation
{
    internal interface IPropertyDelegate<T>
    {
        T Get(object? thisRef, PropertyInfo property);

        void Set(object? thisRef, PropertyInfo property, T value);
    }
}
