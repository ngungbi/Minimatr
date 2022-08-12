using System.Reflection;

namespace Minimatr.Configuration;

public class MinimatrConfiguration {
    public Assembly? Assembly { get; set; }
    public bool EnableInferredBinding { get; set; } = true;
}
