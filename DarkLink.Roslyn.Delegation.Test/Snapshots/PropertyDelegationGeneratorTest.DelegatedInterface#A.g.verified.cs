//HintName: A.g.cs
partial class A
{
    private static readonly System.Reflection.PropertyInfo ____StringProperty_PropertyInfo = typeof(A).GetProperty(nameof(StringProperty))!;
    public string StringProperty
    {
        get => stringProperty.Get(this, ____StringProperty_PropertyInfo);
        set => stringProperty.Set(this, ____StringProperty_PropertyInfo, value);
    }
}
