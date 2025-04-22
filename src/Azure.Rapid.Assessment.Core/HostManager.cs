using Microsoft.Extensions.Logging;

namespace Azure.Rapid.Assessment.Core
{
    public class HostManager
    {
        internal static ILoggerFactory _loggerFactory;

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
