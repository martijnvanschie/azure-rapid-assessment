using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Azure.Rapid.Assessment.Core.Model;

namespace Azure.Rapid.Assessment.Core
{
    public class ResourceGraphService
    {
        private ResourceGraphService() { }

        public string AppUrl { get; private set; }
        public HttpClient Client { get; private set; }


        public static async Task<ResourceGraphService> CreateAsync(AzureCliCredential tokenCredential)
        {
            var instance = new ResourceGraphService();
            await instance.InitializeAsync(tokenCredential);
            return instance;
        }

        private async Task InitializeAsync(AzureCliCredential tokenCredential)
        {
            // Get an access token for the Azure Resource Graph API
            var token = await tokenCredential.GetTokenAsync(
                new TokenRequestContext(new[] { "https://management.azure.com/.default" })
            );

            // Create an HttpClient and set the Authorization header
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            // Define the Azure Resource Graph API endpoint
            AppUrl = "https://management.azure.com/providers/Microsoft.ResourceGraph/resources?api-version=2024-04-01";
        }

        public async Task PostAsync(QueryRequest query)
        {
            // Serialize the query to JSON
            var queryJson = JsonSerializer.Serialize(query);

            var response = await Client.PostAsync(AppUrl, new StringContent(queryJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                {
                    PrintResponse(content);
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
        }

        static void PrintResponse(string content)
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
