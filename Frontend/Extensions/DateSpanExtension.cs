using Frontend.Models;

namespace Frontend.Extensions
{
    public static class DateSpanExtension
    {
        public static string ToStringWithoutUnderline(this DateSpan dateSpan)
        {
            return dateSpan.ToString()[1..];
        }

        public static char GetUnit(this DateSpan dateSpan)
        {
            return dateSpan.ToString()[^1];
        }

        public static int GetValue(this DateSpan dateSpan)
        {
            return int.Parse(dateSpan.ToStringWithoutUnderline()[..^1]);
        }
    }
}
