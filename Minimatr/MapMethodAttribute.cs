namespace Minimatr;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class MapMethodAttribute : Attribute {
    public string Template { get; }

    // public string? Template { get; }
    // public int? Order { get; } = null;
    public string? Name { get; init; }
    public string? GroupName { get; init; }
    public IEnumerable<string> SupportedMethods { get; }

    public MapMethodAttribute(string httpMethod, string template) {
        SupportedMethods = new[] {httpMethod};
        Template = template;
    }

    public MapMethodAttribute(IEnumerable<string> supportedMethods, string template) {
        SupportedMethods = supportedMethods;
        Template = template;
    }
}
