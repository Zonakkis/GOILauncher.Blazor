using Shared.Models.DTOs;

namespace Shared.Models.Entities
{
    public class Speedrun
    {
        public int Id { get; set; }
        public string Player { get; set; }
        public string Area { get; set; }
        public string Category { get; set; }
        public string Platform { get; set; }
        public float Time { get; set; }
        public DateTime Date { get; set; }
        public string Comment { get; set; }
    }
}
