namespace Minimatr;

public sealed class MapPostAttribute : MapMethodAttribute {
    // private static readonly IEnumerable<string> Methods = new[] {"POST"};
    private const string HttpMethod = "POST";

    public MapPostAttribute(string template) : base(HttpMethod, template) { }

    public MapPostAttribute() : base(HttpMethod, "/") { }
}
