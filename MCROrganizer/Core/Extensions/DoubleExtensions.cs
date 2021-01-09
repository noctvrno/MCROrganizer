using System;

namespace MCROrganizer.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static Boolean IsInRange(this Double x, Double y, Double range = 1.0)
        {
            return (x >= y - range) && (x <= y + range);
        }

        public static Boolean IsBetween(this Double x, Double y, Double z, Double tol = 1e-6)
        {
            return (x >= y - tol) && (x <= z + tol);
        }
    }
}
