using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WaveGenerator.Generation;
using WaveGenerator.UI.Pages;
using Windows.UI;

namespace WaveGenerator.Rendering
{
    public sealed class WaveRenderer
    {
        #region Settings

        /// <summary>
        /// Gets undelaying <see cref="Microsoft.UI.Xaml.Controls.Canvas"/>
        /// </summary>
        public CanvasControl Canvas { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="RenderSettings"/>
        /// </summary>
        public RenderSettings Settings { get; set; }

        #endregion

        public WaveRenderer(CanvasControl canvas, RenderSettings settings)
        {
            this.Canvas = canvas;
            this.Settings = settings;

            this.Canvas.Draw += Canvas_Draw;
        }

        Vector2 CalcWavePointPos(Vector2 point)
        {
            double unit = YUnit;
            return new Vector2(
                (float)(unit * point.X + Settings.Offset.X),
                (float)((Canvas.ActualHeight / 2) - (unit * point.Y) + Settings.Offset.Y)
            );
        }

        private void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var session = args.DrawingSession;

            if (ShowCoordinateSystem)
            {
                double xunit = XUnit;
                double yunit = YUnit;

                // X-Axis
                session.DrawLine(
                    new Vector2(0, (float)(Canvas.ActualHeight / 2) + Settings.Offset.Y),
                    new Vector2((float)Canvas.ActualWidth, (float)(Canvas.ActualHeight / 2) + Settings.Offset.Y),
                    CoordinatesPrimaryColor
                );

                // Y-Axis
                session.DrawLine(
                    new Vector2(Settings.Offset.X, (float)Canvas.ActualHeight),
                    new Vector2(Settings.Offset.X, 0),
                    CoordinatesPrimaryColor
                );

                for (double x = 0; x < Canvas.ActualWidth; x += xunit)
                {
                    float thickness = 0.25f;
                    if (x % (xunit * 5) == 0)
                        thickness = 0.5f;

                    session.DrawLine(
                        new Vector2(Settings.Offset.X + (float)x, (float)Canvas.ActualHeight),
                        new Vector2(Settings.Offset.X + (float)x, 0),
                        CoordinatesSecondaryColor,
                        thickness
                    );
                }
                for (double y = (Canvas.ActualHeight / 2) - yunit * (int)((Canvas.ActualHeight / 2 / yunit) + 1); y < Canvas.ActualHeight; y += yunit)
                {
                    float thickness = 0.25f;
                    if (y % (yunit * 5) == 0)
                        thickness = 0.5f;

                    session.DrawLine(
                        new Vector2(Settings.Offset.X, (float)y + Settings.Offset.Y),
                        new Vector2((float)Canvas.ActualWidth, (float)y + Settings.Offset.Y),
                        CoordinatesSecondaryColor,
                        thickness
                    );
                }
            }

            foreach (var wave in VisibleWaves)
            {
                Vector2[] points = wave.data.Select(CalcWavePointPos).ToArray();

                if (SettingsPage.InterpolateWavePoints)
                {
                    CanvasPathBuilder pathBuilder = new(sender.Device);
                    pathBuilder.BeginFigure(points[0]);

                    BezierSpline.GetCurveControlPoints(points, out var c1s, out var c2s);
                    for (int i = 0; i < points.Length - 1; i++)
                        pathBuilder.AddCubicBezier(c1s[i], c2s[i], points[i + 1]);

                    pathBuilder.EndFigure(CanvasFigureLoop.Open);
                    session.DrawGeometry(CanvasGeometry.CreatePath(pathBuilder), wave.color, 1);
                }                

                for (int i = 0; i < points.Length; i++)
                {
                    var pos = points[i];
                    session.FillCircle(pos, (float)SettingsPage.WavePointRadius, wave.color);

                    if (SettingsPage.LabelWavePoints)
                        session.DrawText(
                            (i + 1).ToString(),
                            pos + new Vector2(0, -1.2f * (float)SettingsPage.WavePointRadius),
                            wave.color,
                            new()
                            {
                                FontSize = 15,
                                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                                VerticalAlignment = CanvasVerticalAlignment.Bottom
                            }
                        );
                }

            }

            foreach (var zeiger in VisibleZeiger)
            {
                double unit = YUnit;

                Vector2 pos1 = new Vector2((float)(zeiger.Radius + zeiger.Position.X * unit), (float)((Canvas.ActualHeight / 2) + zeiger.Position.Y * unit));

                session.DrawCircle(
                    pos1,
                    zeiger.Radius,
                    zeiger.Color
                ); ;

                // == // Zeiger // == //

                session.DrawLine(
                    pos1,
                    new Vector2((float)(pos1.X + Math.Cos(zeiger.Angle) * zeiger.Radius + zeiger.Position.X * unit), (float)(pos1.Y + Math.Sin(zeiger.Angle) * zeiger.Radius + zeiger.Position.Y * unit)),
                    zeiger.Color
                );

                // == // ŝ Renderer // == //
                if (zeiger.ShowAmplitude)
                {
                    float height = (float)(Canvas.ActualHeight / 2) + Settings.Offset.Y + (float)(Math.Sin(zeiger.Angle) * zeiger.Radius);
                    session.DrawLine(
                        new Vector2(0, height),
                        new Vector2((float)Canvas.ActualWidth, height),
                        zeiger.Color
                    );
                }
            }

            foreach (var wall in VisibleWalls)
            {
                float x = (float)(XUnit * wall.EndPosition + Settings.Offset.X);
                session.DrawLine(
                    new Vector2(x, 0),
                    new Vector2(x, (float)Canvas.ActualHeight),
                    wall.Color,
                    3
                );
            }
        }

        public bool ShowCoordinateSystem { get; set; } = false;
        public Color CoordinatesPrimaryColor { get; set; } = Colors.Black;
        public Color CoordinatesSecondaryColor { get; set; } = Colors.Gray;

        public List<Wave> VisibleWaves { get; } = new();
        public List<ZeigerInfo> VisibleZeiger { get; } = new();
        public List<WallInfo> VisibleWalls { get; } = new();

        public void Clear()
        {
            VisibleWaves.Clear();
            VisibleZeiger.Clear();
            VisibleWalls.Clear();
        }

        public void Render()
            => Canvas.Invalidate();

        public double XUnit => YUnit;
        public double YUnit => Canvas.ActualHeight / (Settings.YStepCount * 2);
    }

    // https://www.codeproject.com/Articles/31859/Draw-a-Smooth-Curve-through-a-Set-of-2D-Points-wit

    /// <summary>
    /// Bezier Spline methods
    /// </summary>
    public static class BezierSpline
    {
        /// <summary>
        /// Get open-ended Bezier Spline Control Points.
        /// </summary>
        /// <param name="knots">Input Knot Bezier spline points.</param>
        /// <param name="firstControlPoints">Output First Control points
        /// array of knots.Length - 1 length.</param>
        /// <param name="secondControlPoints">Output Second Control points
        /// array of knots.Length - 1 length.</param>
        /// <exception cref="ArgumentNullException"><paramref name="knots"/>
        /// parameter must be not null.</exception>
        /// <exception cref="ArgumentException"><paramref name="knots"/>
        /// array must contain at least two points.</exception>
        public static void GetCurveControlPoints(Vector2[] knots, out Vector2[] firstControlPoints, out Vector2[] secondControlPoints)
        {
            if (knots == null)
                throw new ArgumentNullException("knots");
            int n = knots.Length - 1;
            if (n < 1)
                throw new ArgumentException
                ("At least two knot points required", "knots");
            if (n == 1)
            { // Special case: Bezier curve should be a straight line.
                firstControlPoints = new Vector2[1];
                // 3P1 = 2P0 + P3
                firstControlPoints[0].X = (2 * knots[0].X + knots[1].X) / 3;
                firstControlPoints[0].Y = (2 * knots[0].Y + knots[1].Y) / 3;

                secondControlPoints = new Vector2[1];
                // P2 = 2P1 – P0
                secondControlPoints[0].X = 2 *
                    firstControlPoints[0].X - knots[0].X;
                secondControlPoints[0].Y = 2 *
                    firstControlPoints[0].Y - knots[0].Y;
                return;
            }

            // Calculate first Bezier control points
            // Right hand side vector
            float[] rhs = new float[n];

            // Set right hand side X values
            for (int i = 1; i < n - 1; ++i)
                rhs[i] = 4 * knots[i].X + 2 * knots[i + 1].X;
            rhs[0] = knots[0].X + 2 * knots[1].X;
            rhs[n - 1] = (8 * knots[n - 1].X + knots[n].X) / 2.0f;
            // Get first control points X-values
            float[] x = GetFirstControlPoints(rhs);

            // Set right hand side Y values
            for (int i = 1; i < n - 1; ++i)
                rhs[i] = 4 * knots[i].Y + 2 * knots[i + 1].Y;
            rhs[0] = knots[0].Y + 2 * knots[1].Y;
            rhs[n - 1] = (8 * knots[n - 1].Y + knots[n].Y) / 2.0f;
            // Get first control points Y-values
            float[] y = GetFirstControlPoints(rhs);

            // Fill output arrays.
            firstControlPoints = new Vector2[n];
            secondControlPoints = new Vector2[n];
            for (int i = 0; i < n; ++i)
            {
                // First control point
                firstControlPoints[i] = new Vector2(x[i], y[i]);
                // Second control point
                if (i < n - 1)
                    secondControlPoints[i] = new Vector2(2 * knots
                        [i + 1].X - x[i + 1], 2 *
                        knots[i + 1].Y - y[i + 1]);
                else
                    secondControlPoints[i] = new Vector2((knots
                        [n].X + x[n - 1]) / 2,
                        (knots[n].Y + y[n - 1]) / 2);
            }
        }

        /// <summary>
        /// Solves a tridiagonal system for one of coordinates (x or y)
        /// of first Bezier control points.
        /// </summary>
        /// <param name="rhs">Right hand side vector.</param>
        /// <returns>Solution vector.</returns>
        static float[] GetFirstControlPoints(float[] rhs)
        {
            int n = rhs.Length;
            float[] x = new float[n]; // Solution vector.
            float[] tmp = new float[n]; // Temp workspace.

            float b = 2.0f;
            x[0] = rhs[0] / b;
            for (int i = 1; i < n; i++) // Decomposition and forward substitution.
            {
                tmp[i] = 1 / b;
                b = (i < n - 1 ? 4.0f : 3.5f) - tmp[i];
                x[i] = (rhs[i] - x[i - 1]) / b;
            }
            for (int i = 1; i < n; i++)
                x[n - i - 1] -= tmp[n - i] * x[n - i]; // Backsubstitution.

            return x;
        }
    }
}
