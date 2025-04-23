using Azure.Identity;
using Azure.Rapid.Assessment.Core.Model;
using Azure.Rapid.Assessment.Core;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Azure.Rapid.Assessment.CommandLine
{
    internal class Main
    {
        static ILogger<Main> _logger;
        static ConfigurationRoot _configuration = ConfigurationManager.GetConfiguration();
        static DateTime _runDate = DateTime.UtcNow;

        public Main(ILogger<Main> logger)
        {
            _logger = logger;
        }

        internal async Task<int> StartAsync(string[] args)
        {
            _logger.LogInformation("Azure Rapid Assessment CLI started.");
            _logger.LogInformation($"Run date set to: [{_runDate}]");
            _logger.LogDebug($"Current directory: {Environment.CurrentDirectory}");

            // Check if the 'debug' argument is set
            _logger.LogTrace(
                args.Length > 0 && args[0].Equals("debug", StringComparison.OrdinalIgnoreCase)
                    ? "Debugging mode enabled."
                    : "Debugging mode not enabled."
            );

            // If debugging mode is enabled, launch the debugger
            if (args.Length > 0 && args[0].Equals("debug", StringComparison.OrdinalIgnoreCase))
            {
                Debugger.Launch();
            }

            await Run(args);
            _logger.LogInformation("Async method completed!");

            return 0;
        }

        static async Task Run(string[] args)
        {
            _logger.LogDebug($"Query folder from config: [{_configuration.Queries.QueriesFolder}]");
            _logger.LogDebug($"Query folder full path: [{Path.GetFullPath(_configuration.Queries.QueriesFolder!)}]");

            List<QueryInfo> queryFiles = await QueryFileHandler.GetAllQueryFilesAsync(new DirectoryInfo(_configuration.Queries.QueriesFolder!));

            // Authenticate using Azure CLI credentials
            var tokenCredential = new AzureCliCredential();
            var testClient = await ResourceGraphService.CreateAsync(tokenCredential);

            foreach (var queryFile in queryFiles)
            {
                var data = await testClient.ExecuteQueryAsync(queryFile);

                var assessmentResults = new List<AssessmentResult>();

                foreach (var resource in data)
                {
                    var assessmentResult = new AssessmentResult
                    {
                        Id = resource.Id,
                        Name = resource.Name,
                        Type = resource.Type,
                        SubscriptionId = resource.SubscriptionId,
                        Kind = resource.Kind,
                        TenantId = resource.TenantId,
                        Category = queryFile.Category,
                        Definition = queryFile.Definition,
                        RunDateTimeUtc = _runDate,
                    };

                    assessmentResults.Add(assessmentResult);
                }

                await ParquetHandler.AppendDataAsync(assessmentResults);
            }

            _logger.LogInformation("Finished executing queries.");
        }
    }
}
