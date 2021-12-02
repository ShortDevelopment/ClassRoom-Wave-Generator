﻿using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WaveGenerator.UI.Generation;
using Windows.UI;

namespace WaveGenerator.UI.Rendering
{
    public class WaveRenderer
    {

        #region Settings

        /// <summary>
        /// Gets undelaying <see cref="Microsoft.UI.Xaml.Controls.Canvas"/>
        /// </summary>
        public Canvas Canvas { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="RenderSettings"/>
        /// </summary>
        public RenderSettings Settings { get; set; }

        #endregion

        public WaveRenderer(Canvas canvas, RenderSettings settings)
        {
            this.Canvas = canvas;
            this.Settings = settings;

            this.Canvas.SizeChanged += Canvas_SizeChanged;
        }

        #region Visuals

        Dictionary<string, ContainerVisual> CanvasVisuals = new();
        ContainerVisual baseContainerVisual = null;
        private ContainerVisual CreateContainerVisual(string id)
        {
            if (CanvasVisuals.ContainsKey(id))
                return CanvasVisuals[id];

            if (baseContainerVisual == null)
            {
                baseContainerVisual = ElementCompositionPreview.GetElementVisual(Canvas).Compositor.CreateContainerVisual();
                ElementCompositionPreview.SetElementChildVisual(Canvas, baseContainerVisual);
            }

            var containerVisual = baseContainerVisual.Compositor.CreateContainerVisual();
            baseContainerVisual.Children.InsertAtTop(containerVisual);

            CanvasVisuals.Add(id, containerVisual);
            return containerVisual;
        }
        #endregion

        private void Canvas_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            Canvas.Clip = new RectangleGeometry()
            {
                Rect = new Windows.Foundation.Rect(0, 0, Canvas.ActualWidth, Canvas.ActualHeight)
            };
        }

        #region Render Methods

        public void ClearCanvas()
        {
            Canvas.Children.Clear();
        }

        public void RenderCoordinateSystem(Color color)
        {
            double xunit = XUnit;
            double yunit = YUnit;

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
                    new Vector2(Settings.Offset.X + (float)x, 0), thickness, color);
            }
            for (double y = (Canvas.ActualHeight / 2) - yunit * (int)((Canvas.ActualHeight / 2 / yunit) + 1); y < Canvas.ActualHeight; y += yunit)
            {
                double thickness = 0.25;
                if (y % (yunit * 5) == 0)
                    thickness = 0.5;

                DrawLine(new Vector2(Settings.Offset.X, (float)y + Settings.Offset.Y),
                new Vector2((float)Canvas.ActualWidth, (float)y + Settings.Offset.Y), thickness, color);
            }
        }

        public void Render(Wave wave)
        {
            ContainerVisual canvasVisual = CreateContainerVisual(wave.color.ToString());
            Visual[] children = canvasVisual.Children.ToArray();

            Vector2[] points = wave.data;
            double unit = YUnit;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 point = points[i];
                Vector2 size = new Vector2((float)Settings.Radius, (float)Settings.Radius);

                ShapeVisual visual = null;
                if (children.Length != points.Length)
                {
                    CompositionEllipseGeometry circle = canvasVisual.Compositor.CreateEllipseGeometry();
                    circle.Center = size / 2;
                    circle.Radius = new Vector2(50, 50);

                    CompositionSpriteShape sprite = canvasVisual.Compositor.CreateSpriteShape(circle);
                    sprite.FillBrush = canvasVisual.Compositor.CreateColorBrush(wave.color);

                    visual = canvasVisual.Compositor.CreateShapeVisual();
                    visual.Shapes.Add(sprite);
                    visual.Size = size;

                    // Add to canvas
                    canvasVisual.Children.InsertAtTop(visual);
                }
                else
                {
                    visual = children[i] as ShapeVisual;
                }

                visual.IsVisible = true;
                visual.Offset = new Vector3((float)(unit * point.X - (size.X / 2) + Settings.Offset.X),
                                            (float)((Canvas.ActualHeight / 2) - (unit * point.Y) - (size.Y / 2) + Settings.Offset.Y),
                                            0);
            }
        }

        public void HideWave(Color color)
        {
            ContainerVisual canvasVisual = CreateContainerVisual(color.ToString());
            foreach (Visual visual in canvasVisual.Children)
                visual.IsVisible = false;
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
                new Vector2((float)Canvas.ActualWidth, height), 1, Colors.Green);
        }

        public void RenderReflectionWall(WaveReflectionInfo reflectionInfo)
        {
            float x = (float)(XUnit * reflectionInfo.EndPosition + Settings.Offset.X);
            DrawLine(new Vector2(x, 0),
                new Vector2(x, (float)Canvas.ActualHeight),
                3, Colors.Red);
        }

        #endregion

        #region Helper Functions

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

        #endregion

        public double XUnit => YUnit;
        public double YUnit => Canvas.ActualHeight / (Settings.YStepCount * 2);

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
