namespace Minimatr;

/// <summary>
/// Map request endpoint with GET method
/// </summary>
public sealed class MapGetAttribute : MapMethodAttribute {
    // private static readonly IEnumerable<string> Methods = new[] {"GET"};
    private const string HttpMethod = "GET";
    public MapGetAttribute(string template) : base(HttpMethod, template) { }

    public MapGetAttribute() : base(HttpMethod, "/") { }
}
