using System.Text.Json.Serialization;

namespace Azure.Rapid.Assessment.Core.Model
{
    public class QueryResponse
    {
        public QueryResponse() { }

        [JsonPropertyName("totalRecords")]
        public int TotalRecords { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("data")]
        public List<dynamic> Data { get; set; }
    }
}
