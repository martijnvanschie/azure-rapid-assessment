using Azure.Identity;
using Azure.Rapid.Assessment.Core.Model;
using Azure.Rapid.Assessment.Core;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Azure.Rapid.Assessment.CommandLine
{
    internal class Main
    {
        ILogger<Main> _logger;

        public Main(ILogger<Main> logger)
        {
            _logger = logger;
        }

        static string _queryfolder = "C:\\Repositories\\GitHub\\martijnvanschie\\azure-rapid-assessment\\queries";

        internal async Task<int> StartAsync(string[] args)
        {
            _logger.LogInformation("Starting Azure Rapid Assessment CLI...");

            Console.WriteLine("Starting async method...");

            // Check if the 'debug' argument is set
            Console.WriteLine(
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
            Console.WriteLine("Async method completed!");

            return 0;
        }

        static async Task Run(string[] args)
        {
            Console.WriteLine("Hello, World!");

            // Check if the query folder exists
            if (!Directory.Exists(_queryfolder))
            {
                Console.WriteLine($"The query folder '{_queryfolder}' does not exist.");
                return;
            }

            // iterate over all files in the query folder and return only files with extension .query, including subdirectories
            var files = Directory.GetFiles(_queryfolder, "*.query", SearchOption.AllDirectories);

            // Check if there are any files in the query folder
            if (files.Length == 0)
            {
                Console.WriteLine($"The query folder '{_queryfolder}' does not contain any .query files.");
                return;
            }

            // Authenticate using Azure CLI credentials
            var tokenCredential = new AzureCliCredential();
            var testClient = await ResourceGraphService.CreateAsync(tokenCredential);

            foreach (var item in files)
            {
                string fileContent = File.ReadAllText(item);

                var q2 = new QueryRequest()
                {
                    Query = fileContent,
                    //Subscriptions = new List<string> { "10d42931-b0b2-4442-badc-311acc7edb2f" }
                };

                await testClient.PostAsync(q2);
            }

            Console.WriteLine("Finished executing queries.");
        }
    }
}
