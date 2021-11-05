using LiveCharts;
using LiveCharts.Uwp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

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

        private void SpaltPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Force render
            UpdateArrowDisplay();
            UpdateChart();
        }
        private void ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            UpdateArrowDisplay();
            UpdateChart();
        }

        #region Arrow Display
        const double chartXStep = 0.01;
        private void UpdateArrowDisplay()
        {
            if (ZeigerCanvas == null)
                return;

            // Clear Canvas
            ZeigerCanvas.Children.Clear();

            Vector2[] arrowVectors;
            CalculateResultingAmplitude((int)SpaltCount.Value, GangUnterschied.Value, length, out arrowVectors);

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

            #region resultArrow
            Line resultArrow = new Line();
            resultArrow.Stroke = new SolidColorBrush(Colors.Red);
            resultArrow.StrokeThickness = 1;

            resultArrow.X1 = 0;
            resultArrow.Y1 = 0;
            resultArrow.X2 = lastPosition.X;
            resultArrow.Y2 = lastPosition.Y;

            ZeigerCanvas.Children.Add(resultArrow);
            #endregion
        }
        #endregion

        #region Chart
        private void InitChart()
        {
            Chart.AxisX.Add(new Axis()
            {
                LabelFormatter = (value) => $"{value * chartXStep} λ"
            });
            Chart.AxisY.Add(new Axis()
            {
                MinValue = 0,
                ShowLabels = false
            });
        }

        private SeriesCollection ChartSeriesCollection = new SeriesCollection();
        private async void UpdateChart()
        {
            ChartSeriesCollection.Clear();

            int spaltCount = (int)SpaltCount.Value;
            List<double> valueCollection = new List<double>();
            await Task.Run(() =>
            {
                for (double gangUnterschiedFactor = 0; gangUnterschiedFactor < 3; gangUnterschiedFactor += chartXStep)
                {
                    double value = Math.Pow(CalculateResultingAmplitude(spaltCount, gangUnterschiedFactor, length, out _) / length, 2);
                    valueCollection.Add(value);
                }
            });

            ChartSeriesCollection.Add(new LineSeries()
            {
                Values = new ChartValues<double>(valueCollection),
                LineSmoothness = 1,
                PointGeometry = null
            });
        }
        #endregion

        #region Calculation
        const double length = 100;
        private float CalculateResultingAmplitude(int spaltCount, double gangUnterschiedFactor, double arrowLength, out Vector2[] arrows)
        {
            List<Vector2> arrowList = new List<Vector2>();

            Vector2 resultingVector = new Vector2(0, 0);
            for (int i = 0; i < spaltCount; i++)
            {
                const double π = Math.PI;
                Func<double, double> sin = (double a) => Math.Sin(a);
                Func<double, double> cos = (double a) => Math.Cos(a);

                double α = -i * gangUnterschiedFactor * 2 * π;

                Vector2 vector = new Vector2((float)(cos(α) * arrowLength), (float)(sin(α) * arrowLength));
                arrowList.Add(vector);
                resultingVector += vector;
            }

            arrows = arrowList.ToArray();
            return resultingVector.Length();
        }
        #endregion
    }
}
