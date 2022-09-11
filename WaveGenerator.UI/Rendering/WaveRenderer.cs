using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using WaveGenerator.Generation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.Rendering
{
    public class WaveRenderer
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
                Vector2[] points = wave.data;
                double unit = YUnit;
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2 point = points[i];
                    var pos = new Vector2(
                        (float)(unit * point.X + Settings.Offset.X),
                        (float)((Canvas.ActualHeight / 2) - (unit * point.Y) + Settings.Offset.Y)
                    );
                    session.FillCircle(pos, (float)Settings.Radius, wave.color);
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
}
