using Azure.Identity;
using Azure.Rapid.Assessment.Core.Model;
using Azure.Rapid.Assessment.Core;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Azure.Rapid.Assessment.CommandLine
{
    internal class Main
    {
        static ILogger<Main> _logger;

        public Main(ILogger<Main> logger)
        {
            _logger = logger;
        }

        static string _queryfolder = "C:\\Repositories\\GitHub\\martijnvanschie\\azure-rapid-assessment\\queries";

        internal async Task<int> StartAsync(string[] args)
        {
            _logger.LogInformation("Starting Azure Rapid Assessment CLI...");

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
            _logger.LogInformation("Hello, World!");

            List<QueryInfo> queryFiles = await QueryFileHandler.GetAllQueryFilesAsync(new DirectoryInfo(_queryfolder));

            // Authenticate using Azure CLI credentials
            var tokenCredential = new AzureCliCredential();
            var testClient = await ResourceGraphService.CreateAsync(tokenCredential);

            foreach (var item in queryFiles)
            {
                var data = await testClient.PostAsync(item);
                await ParquetHandler.AppendDataAsync(data);
            }

            _logger.LogInformation("Finished executing queries.");
        }
    }
}
