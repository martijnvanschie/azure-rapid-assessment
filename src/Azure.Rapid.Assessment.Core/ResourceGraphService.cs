using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Azure.Rapid.Assessment.Core.Model;
using Microsoft.Extensions.Logging;

namespace Azure.Rapid.Assessment.Core
{
    public class ResourceGraphService
    {
        private static readonly ILogger<ResourceGraphService> _logger = LoggerManager.GetLogger<ResourceGraphService>();

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

        public async Task<List<AzureResource>> ExecuteQueryAsync(QueryInfo query)
        {
            _logger.LogInformation($"Executing query: {query.Title}");

            QueryRequest request = new QueryRequest()
            {
                Query = query.Query,
            };

            // Serialize the query to JSON
            var queryJson = JsonSerializer.Serialize(request);

            var response = await Client.PostAsync(AppUrl, new StringContent(queryJson, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode == false)
            {
                _logger.LogWarning($"API call failed with status code: {response.StatusCode}");
                _logger.LogWarning(await response.Content.ReadAsStringAsync());
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(content))
            {
                PrintResponse(content);
            }

            var queryResponse = JsonSerializer.Deserialize<QueryResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (queryResponse is null)
            {
                _logger.LogWarning("Failed to deserialize the response.");
                return new List<AzureResource>();
            }

            _logger.LogInformation($"Total Records: {queryResponse.TotalRecords}");
            _logger.LogInformation($"Count: {queryResponse.Count}");

            return queryResponse.Data;
        }

        static void PrintResponse(string content)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                // Deserialize the JSON content into a JsonDocument
                using var jsonDocument = JsonDocument.Parse(content);

                // Serialize it back with indentation for pretty printing
                var prettyJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions { WriteIndented = true });

                _logger.LogTrace("API Response:");
                _logger.LogTrace(prettyJson);
            }
        }
    }
}
