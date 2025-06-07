using System.Text.Json.Serialization;

namespace Frontend.Models.DTOs.AList.fs.get
{
    public class AListFileGetData
    {
        [JsonPropertyName("raw_url")]
        public string RawUrl { get; set; }
    }
}
