using System.Text.Json.Serialization;

namespace Azure.Rapid.Assessment.Core.Model
{
    public class  QueryInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("definition")]
        public string Definition { get; set; }

        [JsonPropertyName("query")]
        public string Query { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}

