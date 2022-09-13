using System.Reflection;

namespace Minimatr.Configuration;

public sealed class MinimatrConfiguration {
    /// <summary>
    /// Assembly to scan
    /// </summary>
    public Assembly? Assembly { get; set; }
    public bool EnableInferredBinding { get; set; } = true;
    
    /// <summary>
    /// When enabled, endpoint parameters will be built at first request to its endpoint.
    /// This will reduce startup time but increase first time request at endpoint.
    /// </summary>
    public bool BuildOnDemand { get; set; }
}
