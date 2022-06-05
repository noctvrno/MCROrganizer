using System;

namespace MCROrganizer.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static Boolean IsLessThan(this Double x, Double y, Double tol = 1e-6)
        {
            return x <= y + tol;
        }

        public static Boolean IsGreaterThan(this Double x, Double y, Double tol = 1e-6)
        {
            return x >= y - tol;
        }

        public static Boolean IsInRange(this Double x, Double y, Double range = 1.0)
        {
            return (x >= y - range) && (x <= y + range);
        }

        public static Boolean IsBetween(this Double x, Double y, Double z, Double tol = 1e-6)
        {
            return (x >= y - tol) && (x <= z + tol);
        }

        public static Double Clamp(this Double x, Double min, Double max, Double tol = 1e-6)
        {
            if (x.IsLessThan(min, tol))
                return min;
            else if (x.IsGreaterThan(max, tol))
                return max;
            else
                return x;
        }
    }
}
