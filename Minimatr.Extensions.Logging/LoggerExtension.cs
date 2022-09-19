using Microsoft.Extensions.Logging;

namespace Minimatr.Extensions.Logging;

public static class LoggerExtension {
    /// <summary>
    /// Checks if the trace level is enabled.
    /// </summary>
    /// <returns><c>true</c> if enabled.</returns>
    public static bool IsTraceEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Trace);
    /// <summary>
    /// Checks if the debug level is enabled.
    /// </summary>
    /// <returns><c>true</c> if enabled.</returns>
    public static bool IsDebugEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Debug);
    /// <summary>
    /// Checks if the information level is enabled.
    /// </summary>
    /// <returns><c>true</c> if enabled.</returns>
    public static bool IsInformationEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Information);
    /// <summary>
    /// Checks if the warning level is enabled.
    /// </summary>
    /// <returns><c>true</c> if enabled.</returns>
    public static bool IsWarningEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Warning);
    /// <summary>
    /// Checks if the error level is enabled.
    /// </summary>
    /// <returns><c>true</c> if enabled.</returns>
    public static bool IsErrorEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Error);
    /// <summary>
    /// Checks if the critical level is enabled.
    /// </summary>
    /// <returns><c>true</c> if enabled.</returns>
    public static bool IsCriticalEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Critical);
}
