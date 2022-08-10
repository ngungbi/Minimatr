namespace Minimatr.ModelBinding;

public interface IParseable {
    bool TryParse(string value, out object result);
}
