namespace Minimatr;

public class MapPutAttribute : MapMethodAttribute {
    private const string HttpMethod = "PUT";

    public MapPutAttribute(string template) : base(HttpMethod, template) { }

    public MapPutAttribute() : base(HttpMethod, "/") { }
}
