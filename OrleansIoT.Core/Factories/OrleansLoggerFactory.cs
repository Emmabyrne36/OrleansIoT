using Microsoft.Extensions.Logging;

namespace OrleansIoT.Core.Factories
{
    public static class OrleansLoggerFactory
    {
        public static ILogger<T> CreateLogger<T>()
        {
            using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace));

            return loggerFactory.CreateLogger<T>();
        }
    }
}
