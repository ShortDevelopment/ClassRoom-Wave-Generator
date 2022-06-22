using Windows.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static WaveGenerator.Generation.MathProxy;

namespace WaveGenerator.Generation
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

        #region Global Calculation Vars & Functions

        #region Settings Proxy
        double λ { get => Settings.WaveLength; }
        double ŝ { get => Settings.MaxElongation; }
        double T { get => Settings.Period; }
        double l { get => Settings.Reflection.EndPosition; }
        #endregion

        double c { get => λ / T; }

        public bool CanNodeMove(double x, double tₒ, bool checkReflectionEnd = false)
        {
            if (x < 0)
                return false;

            if (checkReflectionEnd && Settings.Reflection != null && x > Settings.Reflection.EndPosition)
                return false;

            double distanceToFirstMovingPart = c * tₒ - x;
            if (Settings.GenerationMode == WaveGenerationMode.Einzelstörung)
            {
                if (distanceToFirstMovingPart >= λ / 2)
                    return false;
            }
            else if (Settings.GenerationMode == WaveGenerationMode.Doppelstörung)
            {
                if (distanceToFirstMovingPart >= λ)
                    return false;
            }

            return true;
        }

        #region Basic Calculation
        double s(double x, double tₒ)
        {
            if (!CanNodeMove(x, tₒ))
                return 0;

            return ŝ * sin(2 * π * 1 / T * max(tₒ - x / c, 0));
        }

        double m(double x, double tₒ)
        {
            if (x > l)
                return 0;

            double factor = Settings.Reflection.HasFreeEnd ? 1 : -1;

            return s(2 * l - x, tₒ) * factor;
        }
        #endregion

        #endregion

        /// <summary>
        /// Generate wave at current time
        /// </summary>
        /// <param name="tₒ">Current time in seconds</param>
        /// <param name="count">Number of part in wave in units</param>
        /// <param name="distance">Distance between each part in units</param>
        /// <returns></returns>
        public Wave Generate(double tₒ, int count = 40, double distance = 0.25)
        {
            Func<double, double> s = (double x) => this.s(x, tₒ);

            return GeneratePointsInternal(tₒ, count, distance, s);
        }

        public Wave GenerateReflectedWave(double tₒ, int count = 40, double distance = 0.25)
        {
            Func<double, double> m = (double x) => this.m(x, tₒ);

            var wave = GeneratePointsInternal(tₒ, count, distance, m);
            wave.color = Colors.Blue;
            return wave;
        }

        public Wave GenerateReflectedWaveBothSides(double tₒ, int count = 40, double distance = 0.25)
        {
            double maxtime = l / c;

            // default wave function
            Func<double, double, double> s = (double x, double time) =>
            {
                return ŝ * sin(2 * π * 1 / T * max(time - x / c, 0));
            };

            // mirror wave
            Func<double, double, double> m = (double x, double time) =>
            {
                if (x > l)
                    return 0;
                return s(2 * l - x, time);
            };

            // multiply by -1 if it is NOT a free end
            double factor = Settings.Reflection.HasFreeEnd ? 1 : -1;

            // count of reflection
            double x_a = (c * tₒ);
            double x_b = x_a % l;
            double i = (x_a - x_b) / l + 1;

            // total wave
            Func<double, double> g = (double x) =>
            {
                int f_s = (int)((i + 1) / 2);
                int f_m = (int)(i / 2);
                double total = 0;
                for (int iteration = 0; iteration < f_s; iteration++)
                {
                    var newx = s(x, tₒ - iteration * 2 * maxtime);
                    total += newx;
                    //if(iteration == 1)
                    //    Debugger.Break();
                }
                for (int iteration = 0; iteration < f_m; iteration++)
                {
                    total += factor * m(x, tₒ - iteration * 2 * maxtime);
                }
                return total;
            };

            return GeneratePointsInternal(tₒ, count, distance, g);
        }

        protected Wave GeneratePointsInternal(double tₒ, int count, double distance, Func<double, double> s)
        {
            List<Vector2> list = new();
            for (double x = 0; x < count; x += distance)
                list.Add(new Vector2((float)x, (float)s(x)));

            return new Wave(list.ToArray(), tₒ);
        }

        public Wave MergeWaves(Wave[] waves)
        {
            Wave resultWave = new();
            resultWave.color = Colors.Red;

            Dictionary<float, float> points = new Dictionary<float, float>();

            for (int wi = 0; wi < waves.Length; wi++)
            {
                Wave wave = waves[wi];
                for (int i = 0; i < wave.data.Length; i++)
                {
                    Vector2 p = wave.data[i];
                    float y = p.Y;
                    if (!CanNodeMove(p.X, wave.time, checkReflectionEnd: true))
                        y = 0;
                    if (points.ContainsKey(p.X))
                        points[p.X] += y;
                    else
                        points.Add(p.X, y);
                }
            }

            resultWave.data = points.Select((x) => new Vector2(x.Key, x.Value)).ToArray();

            return resultWave;
        }

        /// <summary>
        /// Calculates the angle of the "Zeiger"
        /// </summary>
        public double CalculateZeigerAngle(double x, double t, bool useReflectedWave = false)
        {
            if (useReflectedWave && Settings.Reflection != null)
            {
                if (!Settings.Reflection.HasFreeEnd)
                    t += π;
                x = Settings.Reflection.EndPosition * 2 - x;
            }

            if (!CanNodeMove(x, t))
                return 0;

            double T = Settings.Period;
            double ω = 2 * π * 1 / T;

            return -ω * (t - x / c);
        }
    }
}
