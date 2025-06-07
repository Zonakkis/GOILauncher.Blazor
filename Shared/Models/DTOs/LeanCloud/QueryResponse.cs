using System.Text.Json.Serialization;

namespace Shared.Models.DTOs.LeanCloud
{
    public class QueryResponse<T>
    {
        [JsonPropertyName("results")]
        public List<T> Results { get; set; }
    }
}
