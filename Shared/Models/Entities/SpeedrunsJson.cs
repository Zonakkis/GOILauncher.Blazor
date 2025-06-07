using Shared.Models.DTOs;

namespace Shared.Models.Entities
{
    public class SpeedrunsJson
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public Platform Platform { get; set; }
        public string Json { get; set; }
    }
}
