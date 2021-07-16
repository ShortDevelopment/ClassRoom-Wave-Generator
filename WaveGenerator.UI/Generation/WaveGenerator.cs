﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

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

        #region Math Proxy
        const double π = Math.PI;
        Func<double, double> sin = (double a) => Math.Sin(a);
        Func<double, double, double> max = (double val1, double val2) => Math.Max(val1, val2);
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

            return GeneratePointsInternal(count, distance, s);
        }

        public Wave GenerateReflectedWave(double tₒ, int count = 40, double distance = 0.25)
        {
            #region Settings Proxy
            double λ = Settings.WaveLength;
            double ŝ = Settings.Amplitude;
            double T = Settings.Period;
            double l = Settings.Reflection.EndPosition;
            #endregion

            double c = λ / T;

            Func<double, double> s = (double x) =>
            {
                return ŝ * sin(2 * π * 1 / T * max(tₒ - x / c, 0));
            };

            double factor = Settings.Reflection.HasFreeEnd ? 1 : -1;

            Func<double, double> m = (double x) =>
            {
                if (x > l)
                    return 0;
                return s(2 * l - x) * factor;
            };

            return GeneratePointsInternal(count, distance, m);
        }

        public Wave GenerateReflectedWaveBothSides(double tₒ, int count = 40, double distance = 0.25)
        {
            #region Settings Proxy
            double λ = Settings.WaveLength;
            double ŝ = Settings.Amplitude;
            double T = Settings.Period;
            double l = Settings.Reflection.EndPosition;
            #endregion

            double c = λ / T;

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
                double total = s(x, tₒ);
                for (int iteration = 0; iteration < f_s; iteration++)
                {
                    total += s(x, iteration * tₒ);
                }
                for (int iteration = 0; iteration <= f_m; iteration++)
                {
                    total += factor * m(x, iteration * tₒ);
                }
                return total;
            };

            return GeneratePointsInternal(count, distance, g);
        }

        protected Wave GeneratePointsInternal(int count, double distance, Func<double, double> s)
        {
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

        public Wave MirrorWave(Wave wave)
        {
            WaveReflectionInfo reflectionInfo = Settings.Reflection;
            Wave resultWave = new Wave();
            resultWave.color = Colors.Blue;
            // resultWave.RTL = true; ⚡

            resultWave.data = wave.data.Where((p) => p.X >= reflectionInfo.EndPosition).Select((p) => new Vector2((float)(reflectionInfo.EndPosition - (p.X - reflectionInfo.EndPosition)), p.Y)).ToArray();

            if (!reflectionInfo.HasFreeEnd)
                resultWave.data = resultWave.data.Select((p) => new Vector2(p.X, -1 * p.Y)).ToArray();

            return resultWave;
        }

        public Wave MergeWaves(Wave[] waves)
        {
            Wave resultWave = new Wave();
            resultWave.color = Colors.Red;

            Dictionary<float, float> points = new Dictionary<float, float>();

            for (int wi = 0; wi < waves.Length; wi++)
            {
                Wave wave = waves[wi];
                for (int i = 0; i < wave.data.Length; i++)
                {
                    Vector2 p = wave.data[i];
                    if (points.ContainsKey(p.X))
                        points[p.X] += p.Y;
                    else
                        points.Add(p.X, p.Y);
                }
            }

            resultWave.data = points.Select((x) => new Vector2(x.Key, x.Value)).ToArray();

            return resultWave;
        }

    }
}
