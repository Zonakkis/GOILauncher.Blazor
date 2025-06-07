using System.Text.Json.Serialization;

namespace Shared.Models.DTOs
{
    public class SpeedrunRequest
    {
        [JsonPropertyName("category")]
        public Category Category { get; set; }
        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }
        [JsonPropertyName("dateTo")]
        public DateTime DateTo { get; set; }
        [JsonPropertyName("limit")]
        public int Limit { get; set; }
    }
}
