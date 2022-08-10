namespace Minimatr;

public class MapDeleteAttribute : MapMethodAttribute {
    // private static readonly IEnumerable<string> Methods = new[] {"DELETE"};
    private const string HttpMethod = "DELETE";
    public MapDeleteAttribute(string template) : base(HttpMethod, template) { }

    public MapDeleteAttribute() : base(HttpMethod, "/") { }
}
