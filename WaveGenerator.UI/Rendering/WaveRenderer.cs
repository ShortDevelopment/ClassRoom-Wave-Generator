using System;
using System.Collections.Generic;
using System.Numerics;
using WaveGenerator.UI.Generation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace WaveGenerator.UI.Rendering
{
    public class WaveRenderer
    {
        /// <summary>
        /// Gets undelaying <see cref="Windows.UI.Xaml.Controls.Canvas"/>
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="RenderSettings"/>
        /// </summary>
        public RenderSettings Settings { get; set; }

        public WaveRenderer(Canvas canvas, RenderSettings settings)
        {
            this.Canvas = canvas;
            this.Settings = settings;

            this.Canvas.SizeChanged += Canvas_SizeChanged;
        }

        private void Canvas_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            Canvas.Clip = new RectangleGeometry()
            {
                Rect = new Windows.Foundation.Rect(0, 0, Canvas.ActualWidth, Canvas.ActualHeight)
            };
        }

        public void Render(Wave wave)
        {
            Vector2[] points = wave.data;

            // Clear canvas
            Canvas.Children.Clear();

            double unit = YUnit;

            if (Settings.ShowCoordinateSystem)
                RenderCoordinateSystem(unit, unit);

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i];

                Ellipse circle = new Ellipse();

                // Set radius
                circle.Width = Settings.Radius;
                circle.Height = Settings.Radius;

                // Calculate position                
                Canvas.SetLeft(circle, unit * point.X - (Settings.Radius / 2) + Settings.Offset.X);
                Canvas.SetTop(circle, (Canvas.ActualHeight / 2) - (unit * point.Y) - (Settings.Radius / 2) + Settings.Offset.Y);

                // Set style
                circle.Fill = new SolidColorBrush(Colors.Red);
                circle.StrokeThickness = 0;

                // Add to canvas
                Canvas.Children.Add(circle);
            }
        }

        public void RenderZeiger(double angle, Vector2 position, double radius)
        {
            double unit = YUnit;

            Ellipse circle = new Ellipse();
            circle.Fill = new SolidColorBrush(Colors.Transparent);
            circle.Stroke = new SolidColorBrush(Colors.Green);
            circle.StrokeThickness = 1;

            // Set radius
            circle.Width = radius * 2;
            circle.Height = radius * 2;

            // Calculate position
            Canvas.SetTop(circle, (Canvas.ActualHeight / 2) - radius);

            // Add to canvas
            Canvas.Children.Add(circle);

            // == // Zeiger // == //

            Vector2 pos1 = new Vector2((float)(radius + position.X * unit), (float)((Canvas.ActualHeight / 2) + position.Y * unit));
            DrawLine(pos1,
                new Vector2((float)(pos1.X + Math.Cos(angle) * radius + position.X * unit), (float)(pos1.Y + Math.Sin(angle) * radius + position.Y * unit)), 1, Colors.Green);

            // == // ŝ Renderer // == //

            float height = (float)(Canvas.ActualHeight / 2) + Settings.Offset.Y + (float)(Math.Sin(angle) * radius);
            DrawLine(new Vector2(0, height),
                new Vector2((float)Canvas.ActualWidth, height), 1, Colors.Gray);
        }

        private void DrawLine(Vector2 p1, Vector2 p2, double thickness = 1)
        {
            DrawLine(p1, p2, thickness, Colors.Black);
        }

        private void DrawLine(Vector2 p1, Vector2 p2, double thickness, Color color)
        {
            Line line = new Line();
            line.StrokeThickness = thickness;
            line.Stroke = new SolidColorBrush(color);

            // Calculate position
            line.X1 = p1.X;
            line.Y1 = p1.Y;
            line.X2 = p2.X;
            line.Y2 = p2.Y;

            // Add to canvas
            Canvas.Children.Add(line);
        }

        private void RenderCoordinateSystem(double xunit, double yunit)
        {
            // X-Axis
            DrawLine(new Vector2(0, (float)(Canvas.ActualHeight / 2) + Settings.Offset.Y),
                new Vector2((float)Canvas.ActualWidth, (float)(Canvas.ActualHeight / 2) + Settings.Offset.Y));

            // Y-Axis
            DrawLine(new Vector2(Settings.Offset.X, (float)Canvas.ActualHeight),
                new Vector2(Settings.Offset.X, 0));

            for (double x = 0; x < Canvas.ActualWidth; x += xunit)
            {
                double thickness = 0.25;
                if (x % (xunit * 5) == 0)
                    thickness = 0.5;

                DrawLine(new Vector2(Settings.Offset.X + (float)x, (float)Canvas.ActualHeight),
                    new Vector2(Settings.Offset.X + (float)x, 0), thickness);
            }
            for (double y = 0; y < Canvas.ActualHeight; y += yunit)
            {
                double thickness = 0.25;
                if (y % (yunit * 5) == 0)
                    thickness = 0.5;

                DrawLine(new Vector2(Settings.Offset.X, (float)y + Settings.Offset.Y),
                new Vector2((float)Canvas.ActualWidth, (float)y + Settings.Offset.Y), thickness);
            }
        }

        public double YUnit
        {
            get
            {
                return Canvas.ActualHeight / (Settings.YStepCount * 2);
            }
        }

        public static Vector2[] GenerateTestData(int count = 10)
        {
            var random = new Random(DateTime.Now.Millisecond);
            List<Vector2> list = new List<Vector2>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new Vector2(i, random.Next(-10, 10)));
            }
            return list.ToArray();
        }

    }
}
