﻿using System;
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

            // == // Zeiger // == //

            // Add to canvas
            Canvas.Children.Add(circle);

            Line line = new Line();
            line.StrokeThickness = 1;
            line.Stroke = new SolidColorBrush(Colors.Green);

            // Calculate position
            line.X1 = radius + position.X * unit;
            line.Y1 = (Canvas.ActualHeight / 2) + position.Y * unit;
            line.X2 = line.X1 + Math.Cos(angle) * radius + position.X * unit;
            line.Y2 = line.Y1 + Math.Sin(angle) * radius + position.Y * unit;

            // Add to canvas
            Canvas.Children.Add(line);
        }

        private void DrawLine(Vector2 p1, Vector2 p2)
        {
            Line line = new Line();
            line.StrokeThickness = 1;
            line.Stroke = new SolidColorBrush(Colors.Black);

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
            // Base line
            DrawLine(new Vector2(0, (float)(Canvas.ActualHeight / 2)), new Vector2((float)Canvas.ActualWidth, (float)(Canvas.ActualHeight / 2)));
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
