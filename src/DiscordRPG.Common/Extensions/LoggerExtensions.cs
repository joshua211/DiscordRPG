using System.Runtime.CompilerServices;
using Serilog;

namespace DiscordRPG.Common.Extensions;

public static class LoggerExtensions
{
    public static ILogger Here(this ILogger logger,
        [CallerMemberName] string memberName = "")
    {
        return logger
            .ForContext("Method", memberName);
    }

    public static ILogger WithContext<T>(this ILogger logger)
    {
        return logger.ForContext("SourceContext", typeof(T).Name);
    }

    public static ILogger WithContext(this ILogger logger, Type type)
    {
        return logger.ForContext("SourceContext", type.Name);
    }

    public static ILogger WithContext(this ILogger logger, string context)
    {
        return logger.ForContext("SourceContext", context);
    }
}