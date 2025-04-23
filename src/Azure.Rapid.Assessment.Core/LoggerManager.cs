using Microsoft.Extensions.Logging;

namespace Azure.Rapid.Assessment.Core
{
    public class LoggerManager
    {
        internal static ILoggerFactory _loggerFactory = null!;

        public static void Initiate(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public static ILogger<T> GetLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }
    }
}
