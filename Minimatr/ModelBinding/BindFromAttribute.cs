namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class BindFromAttribute : Attribute, IModelNameProvider {
    public string? Name { get; init; }
    public BindingSource Source { get; init; }

    public BindFromAttribute(BindingSource source) : this(null, source) { }

    public BindFromAttribute(string? name, BindingSource source) {
        Name = name;
        Source = source;
    }
}
