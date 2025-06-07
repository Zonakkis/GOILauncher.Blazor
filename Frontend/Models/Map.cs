using Humanizer;

namespace Frontend.Models
{
    public class Map
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public int Size { get; set; }
        public string Path { get; set; }
        public string FormattedSize => Size.Bytes().Humanize();
    }
}
