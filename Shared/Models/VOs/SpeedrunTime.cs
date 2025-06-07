namespace Shared.Models.VOs
{
    public class SpeedrunTime(int minutes, int seconds, int milliseconds)
    {
        public int Minutes { get; set; } = minutes + seconds / 60 + milliseconds / 60000;
        public int Seconds { get; set; } = seconds + milliseconds / 1000;
        public int Milliseconds { get; set; } = milliseconds;

        public override string ToString()
        {
            return $"{(Minutes > 0 ? $"{Minutes}分" : "")}{Seconds:D2}.{Milliseconds:D3}秒";
        }
    }
}
