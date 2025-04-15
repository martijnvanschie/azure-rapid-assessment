using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Azure.Rapid.Assessment.Core;
using Azure.Rapid.Assessment.Core.Model;

namespace Azure.Rapid.Assessment.CommandLine
{
    internal class Program
    {
        static string _queryfolder = "C:\\Repositories\\GitHub\\martijnvanschie\\azure-rapid-assessment\\queries";

        static async Task Main(string[] args)
        {
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


            // Get an access token for the Azure Resource Graph API
            var token = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(new[] { "https://management.azure.com/.default" })
            );

            // Create an HttpClient and set the Authorization header
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            // Define the Azure Resource Graph API endpoint
            var apiUrl = "https://management.azure.com/providers/Microsoft.ResourceGraph/resources?api-version=2024-04-01";

            // Example query to list all resources
            var query = new QueryRequest()  
            {
                Query = "Resources | project name, type, location",
                Subscriptions = new List<string> { "10d42931-b0b2-4442-badc-311acc7edb2f" }
            };

            // Serialize the query to JSON
            var queryJson = JsonSerializer.Serialize(query);

            // Make the POST request
            var response = await httpClient.PostAsync(apiUrl, new StringContent(queryJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                {
                    await PrintResponse(content);
                }

                var queryResponse = JsonSerializer.Deserialize<QueryResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (queryResponse != null)
                {
                    Console.WriteLine($"Total Records: {queryResponse.TotalRecords}");
                    Console.WriteLine($"Count: {queryResponse.Count}");
                    foreach (var item in queryResponse.Data)
                    {
                        Console.WriteLine(JsonSerializer.Serialize(item, new JsonSerializerOptions { WriteIndented = true }));
                    }
                }
                else
                {
                    Console.WriteLine("Failed to deserialize the response.");
                }
            }
            else
            {
                Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }


            Console.WriteLine("Finished executing queries.");
        }

        static async Task PrintResponse(string content)
        {
            // Deserialize the JSON content into a JsonDocument
            using var jsonDocument = JsonDocument.Parse(content);

            // Serialize it back with indentation for pretty printing
            var prettyJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions { WriteIndented = true });

            Console.WriteLine("API Response:");
            Console.WriteLine(prettyJson);
        }
    }
}
