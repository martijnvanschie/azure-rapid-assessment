using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;

namespace Azure.Rapid.Assessment.Core
{
    public class ResourceGraphService
    {
        private ResourceGraphService()
        {
            
        }

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
    }
}
