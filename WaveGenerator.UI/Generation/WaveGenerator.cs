using System;
using System.Collections.Generic;
using System.Numerics;

namespace WaveGenerator.UI.Generation
{
    public class WaveGenerator
    {
        /// <summary>
        /// Gets or sets <see cref="WaveSettings"/>
        /// </summary>
        public WaveSettings Settings { get; set; }

        public WaveGenerator(WaveSettings settings)
        {
            this.Settings = settings;
        }

        /// <summary>
        /// Generate wave at current time
        /// </summary>
        /// <param name="tₒ">Current time in seconds</param>
        /// <param name="count">Number of part in wave in units</param>
        /// <param name="distance">Distance between each part in units</param>
        /// <returns></returns>
        public Wave Generate(double tₒ, int count = 40, double distance = 0.25)
        {
            #region Math Proxy
            const double π = Math.PI;
            Func<double, double> sin = (double a) => Math.Sin(a);
            Func<double, double, double> max = (double val1, double val2) => Math.Max(val1, val2);
            #endregion

            #region Settings Proxy
            double λ = Settings.WaveLength;
            double ŝ = Settings.Amplitude;
            double T = Settings.Period;
            #endregion

            double c = λ / T;

            Func<double, double> s = (double x) =>
            {
                return ŝ * sin(2 * π * 1 / T * max(tₒ - x / c, 0));
            };

            List<Vector2> list = new List<Vector2>();
            for (double x = 0; x < count; x += distance)
            {
                list.Add(new Vector2((float)x, (float)s(x)));
            }
            return new Wave(list.ToArray());
        }

        /// <summary>
        /// Calculates the angle of the "Zeiger"
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double CalculateZeigerAngle(double t)
        {
            #region Math Proxy
            const double π = Math.PI;
            #endregion

            double T = Settings.Period;
            double ω = 2 * π * 1 / T;

            return -ω * t;
        }

    }
}
