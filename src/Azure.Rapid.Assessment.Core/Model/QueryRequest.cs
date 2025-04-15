using System.Text.Json.Serialization;

namespace Azure.Rapid.Assessment.Core.Model
{
    public class QueryRequest
    {
        public QueryRequest() { }

        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("managementGroups")]
        public List<string>? ManagementGroups { get; set; }

        [JsonPropertyName("subscriptions")]
        public List<string>? Subscriptions { get; set; }
    }
}
