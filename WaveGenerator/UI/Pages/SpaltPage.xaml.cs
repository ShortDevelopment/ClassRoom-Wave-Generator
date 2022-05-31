//using LiveChartsCore;
//using LiveChartsCore.SkiaSharpView;
//using LiveChartsCore.SkiaSharpView.UWP;
using LiveCharts;
using LiveCharts.Uwp;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using static WaveGenerator.Generation.MathProxy;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class SpaltPage : Page
    {
        public SpaltPage()
        {
            this.InitializeComponent();
            this.Loaded += SpaltPage_Loaded;

            InitChart();
        }

        bool hasFinishedLoading = false;
        private void SpaltPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Force render
            UpdateArrowDisplay();
            UpdateChart();
            hasFinishedLoading = true;
        }

        private void ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (!hasFinishedLoading)
                return;

            UpdateArrowDisplay();
            UpdateChart();
        }

        private void ValueChanged2(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (!hasFinishedLoading)
                return;

            UpdateArrowDisplay();
        }

        #region Arrow Display        
        private void UpdateArrowDisplay()
        {
            if (ZeigerCanvas == null)
                return;

            // Clear Canvas
            ZeigerCanvas.Children.Clear();

            Vector2[] arrowVectors = GenerateArrows((int)SlitCountNumberBox.Value, GangUnterschiedNumberBox.Value, length);

            Vector2 lastPosition = new Vector2(0, 0);
            foreach (Vector2 arrowVector in arrowVectors)
            {
                Line arrow = new Line();
                arrow.Stroke = new SolidColorBrush(Colors.Black);
                arrow.StrokeThickness = 1;

                arrow.X1 = lastPosition.X;
                arrow.Y1 = lastPosition.Y;

                lastPosition += arrowVector;

                arrow.X2 = lastPosition.X;
                arrow.Y2 = lastPosition.Y;
                ZeigerCanvas.Children.Add(arrow);
            }

            DrawArrow(new(0, 0), lastPosition, Colors.Red);
        }

        private void DrawArrow(Vector2 start, Vector2 end, Color color)
        {
            Line arrow = new Line();
            arrow.Stroke = new SolidColorBrush(color);
            arrow.StrokeThickness = 1;

            arrow.X1 = start.X;
            arrow.Y1 = start.Y;

            arrow.X2 = end.X;
            arrow.Y2 = end.Y;
            ZeigerCanvas.Children.Add(arrow);

            if ((end - start).Length() > 0.01)
            {
                Geometry geometry = Utils.ConvertXamlValue<Geometry>("M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6-6-6z");
                var bounds = geometry.Bounds;
                double rotation = Math.Atan((end.Y - start.Y) / (end.X - start.X)) * 180.0 / Math.PI;
                Path arrowHead = new Path
                {
                    Data = geometry,
                    Fill = new SolidColorBrush(Colors.Red),
                    CenterPoint = new Vector3((float)(bounds.Width / 2), (float)(bounds.Height / 2), 0)
                };
                Canvas.SetLeft(arrowHead, end.X - bounds.Width);
                Canvas.SetTop(arrowHead, end.Y - bounds.Height);
                arrowHead.Rotation = (float)rotation;
                ZeigerCanvas.Children.Add(arrowHead);
            }
        }
        #endregion

        #region Chart
        const decimal chartXStep = 0.01M;

        private SeriesCollection ChartSeriesCollection;
        private void InitChart()
        {
            //Chart.XAxes = new[]
            //{
            //    new Axis()
            //    {
            //        Labeler = (value) => $"{(decimal)value * (decimal)chartXStep} λ"
            //    }
            //};
            //Chart.YAxes = new[]
            //{
            //    new Axis()
            //    {
            //        IsVisible = false
            //    }
            //};
            Chart.AxisX.Add(new Axis()
            {
                LabelFormatter = (value) => $"{value * (double)chartXStep} λ"
            });
            Chart.AxisY.Add(new Axis()
            {
                MinValue = 0,
                ShowLabels = false
            });
            ChartSeriesCollection = new()
            {
                /* multi slit */
                new LineSeries()
                {
                    LineSmoothness = 1,
                    PointGeometry = null
                },
                /* single slit / cover */
                new LineSeries()
                {
                    LineSmoothness = 1,
                    PointGeometry = null,
                    Fill = null
                }
            };
        }

        private async void UpdateChart()
        {
            int slitCount = (int)SlitCountNumberBox.Value;
            decimal ratio = (decimal)(slitCount > 1 ? SlitRatioNumberBox.Value / 100.0 : 1.0);

            ChartValues<double> valueCollection = ChartSeriesCollection[0].Values as ChartValues<double> ?? new();
            valueCollection.Clear();
            ChartValues<double> valueCollection2 = ChartSeriesCollection[1].Values as ChartValues<double> ?? new();
            valueCollection2.Clear();
            await Task.Run(() =>
            {
                decimal intensity0 = -1;
                for (decimal gangUnterschiedFactor = chartXStep; gangUnterschiedFactor < 5; gangUnterschiedFactor += chartXStep)
                {
                    decimal singleSlitIntensity = CalculateSingleSlitIntensityRelative(gangUnterschiedFactor, ratio);

                    decimal intensity = singleSlitIntensity * CalculateSlitIntensity(gangUnterschiedFactor, slitCount);
                    if (intensity0 == -1)
                        intensity0 = intensity;
                    if (slitCount > 1)
                        valueCollection.Add((double)(100 * intensity));

                    valueCollection2.Add((double)(100 * singleSlitIntensity * (slitCount > 1 ? intensity0 : 100)));
                }
            });
            ChartSeriesCollection[0].Values = valueCollection;
            ChartSeriesCollection[1].Values = valueCollection2;
        }
        #endregion

        #region Calculation
        const double length = 40;
        private Vector2[] GenerateArrows(int spaltCount, double gangUnterschiedFactor, double arrowLength)
        {
            List<Vector2> arrowList = new();

            Vector2 resultingVector = new(0, 0);
            for (int i = 0; i < spaltCount; i++)
            {
                double α = -i * gangUnterschiedFactor * 2 * π;

                Vector2 vector = new((float)(cos(α) * arrowLength), (float)(sin(α) * arrowLength));
                arrowList.Add(vector);
                resultingVector += vector;
            }

            return arrowList.ToArray();
        }

        private decimal CalculateSlitIntensity(decimal gangUnterschiedFactor, int slitCount)
        {
            if (gangUnterschiedFactor == 0)
                return CalculateSlitIntensity(1, slitCount);

            return (decimal)Math.Pow(
                    (double)(sin(slitCount * (decimal)π * gangUnterschiedFactor)
                    /
                    sin((decimal)π * gangUnterschiedFactor))
                , 2);
        }

        private decimal CalculateSingleSlitIntensityRelative(decimal gangUnterschiedFactor, decimal ratio = 1.0M)
        {
            if (gangUnterschiedFactor == 0)
                return CalculateSingleSlitIntensityRelative(1, ratio);

            return (decimal)Math.Pow(
                    (double)(sin((decimal)π * ratio * gangUnterschiedFactor)
                    /
                    ((decimal)π * ratio * gangUnterschiedFactor))
                , 2);
        }
        #endregion
    }
}
