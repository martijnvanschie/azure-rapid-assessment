using System.Text.Json.Serialization;

namespace Azure.Rapid.Assessment.Core.Model
{
    public class AzureResource
    {
        public AzureResource() { }

        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("kind")]
        public string Kind { get; set; }
        [JsonPropertyName("subscriptionId")]
        public string SubscriptionId { get; set; }
        [JsonPropertyName("tenantId")]
        public string TenantId { get; set; }
    }
}

