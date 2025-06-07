using System.Text.Json.Serialization;

namespace Frontend.Models.DTOs.AList.fs.list
{
    public class AListFileListItem
    {
        public string Name { get; set; }
        public int Size { get; set; }
        [JsonPropertyName("is_dir")]
        public bool IsDirectory { get; set; }
    }
}
