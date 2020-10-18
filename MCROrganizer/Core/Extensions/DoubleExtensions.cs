using System;

namespace MCROrganizer.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static Boolean IsInRange(this Double x, Double y, Double range = 1.0)
        {
            return (x >= y - range) && (x <= y + range);
        }
    }
}
