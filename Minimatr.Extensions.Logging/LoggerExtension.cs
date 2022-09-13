using Microsoft.Extensions.Logging;

namespace Minimatr.Extensions.Logging;

public static class LoggerExtension {
    public static bool IsTraceEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Trace);
    public static bool IsDebugEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Debug);
    public static bool IsInformationEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Information);
    public static bool IsWarningEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Warning);
    public static bool IsErrorEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Error);
    public static bool IsCriticalEnabled(this ILogger logger) => logger.IsEnabled(LogLevel.Critical);
}
