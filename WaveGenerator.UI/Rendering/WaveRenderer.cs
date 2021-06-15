using System;
using System.Collections.Generic;
using System.Numerics;
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
        }

        public void Render(Vector2[] points)
        {
            // Clear canvas
            Canvas.Children.Clear();

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i];

                Ellipse circle = new Ellipse();

                circle.Width = Settings.Radius;
                circle.Height = Settings.Radius;

                double unit = Canvas.ActualHeight / (Settings.YStepCount * 2);
                Canvas.SetLeft(circle, unit * point.X);
                Canvas.SetTop(circle, (Canvas.ActualHeight / 2) - (unit * point.Y));

                circle.Fill = new SolidColorBrush(Colors.Red);
                circle.StrokeThickness = 0;

                Canvas.Children.Add(circle);
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
