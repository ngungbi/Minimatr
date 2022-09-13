using System.Reflection;

namespace Minimatr.Configuration;

public sealed class MinimatrConfiguration {
    public Assembly? Assembly { get; set; }
    public bool EnableInferredBinding { get; set; } = true;
}
