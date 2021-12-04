using System;

namespace WaveGenerator.Generation
{
    public static class MathProxy
    {
        public static readonly double π = Math.PI;
        public static double sin(double a) => Math.Sin(a);
        public static double cos(double a) => Math.Cos(a);
        public static double tan(double a) => Math.Tan(a);

        public static double min(double val1, double val2) => Math.Min(val1, val2);
        public static double max(double val1, double val2) => Math.Max(val1, val2);
    }
}
