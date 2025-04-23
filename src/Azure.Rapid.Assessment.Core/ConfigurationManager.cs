using Microsoft.Extensions.Configuration;

namespace Azure.Rapid.Assessment.Core
{
    public class ConfigurationManager
    {
        internal static IConfiguration _configuration = null!;
        private static ConfigurationRoot _rootConfig = new ConfigurationRoot();

        public static void Initiate(IConfiguration configuration)
        {
            _configuration = configuration;

            configuration.GetSection(nameof(ExportSettings)).Bind(_rootConfig.Export);
            configuration.GetSection(nameof(DatabaseSettings)).Bind(_rootConfig.Database);
            configuration.GetSection(nameof(QuerySettings)).Bind(_rootConfig.Queries);
        }

        public static ConfigurationRoot GetConfiguration()
        {
            return _rootConfig;
        }
    }

    public class ConfigurationRoot
    {
        public DatabaseSettings Database { get; set; } = new DatabaseSettings();
        public ExportSettings Export { get; set; } = new ExportSettings();
        public QuerySettings Queries { get; set; } = new QuerySettings();
    }

    public class ExportSettings
    {
        public string? ExportFolder { get; set; }
        public string? ExportFileName { get; set; }
        public string? ExportFileType { get; set; }
    }

    public class QuerySettings
    {
        public string? QueriesFolder { get; set; } = @".\queries";
        public string? QueryFileExtension { get; set; } = ".query";
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string EnvironmentVariableName { get; set; } = "AZURETRENDSANDINSIGHTS_SQL_CONNECTION_STRING";
    }
}
