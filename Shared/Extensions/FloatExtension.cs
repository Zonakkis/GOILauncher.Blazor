using Shared.Models.Entities;
using Shared.Models.VOs;

namespace Shared.Extensions
{
    public static class FloatExtension
    {
        public static SpeedrunTime FromMilliseconds(this float milliseconds)
        {
            var time = TimeSpan.FromMilliseconds(milliseconds);
            return new SpeedrunTime(0, 0, (int)(milliseconds * 1000));
        }
    }
}
