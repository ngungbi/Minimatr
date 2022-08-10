namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public class FromRouteAttribute : BindFromAttribute {
    public FromRouteAttribute() : this(null) { }
    public FromRouteAttribute(string? name) : base(name, BindingSource.Route) { }
}
