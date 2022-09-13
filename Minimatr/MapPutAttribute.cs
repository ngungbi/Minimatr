namespace Minimatr;

/// <summary>
/// Map request endpoint with PUT method
/// </summary>
public sealed class MapPutAttribute : MapMethodAttribute {
    private const string HttpMethod = "PUT";

    public MapPutAttribute(string template) : base(HttpMethod, template) { }

    public MapPutAttribute() : base(HttpMethod, "/") { }
}
