﻿using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
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
        private void SpaltPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
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

            Vector2[] arrowVectors;
            CalculateResultingAmplitude((int)SlitCountNumberBox.Value, GangUnterschiedNumberBox.Value, length, out arrowVectors);

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

                //Geometry geometry = Utils.ConvertXamlValue<Geometry>("M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6-6-6z");
                //Path arrowHead = new Path
                //{
                //    Data = geometry,
                //    Fill = new SolidColorBrush(Colors.Red),
                //    CenterPoint = new Vector3((float)(geometry.Bounds.Width / 2), (float)(geometry.Bounds.Height / 2), 0),
                //    Rotation = 90
                //};
                //Canvas.SetLeft(arrowHead, lastPosition.X);
                //Canvas.SetTop(arrowHead, lastPosition.Y);
                //ZeigerCanvas.Children.Add(arrowHead);
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
        const double chartXStep = 0.02;

        private List<ISeries> ChartSeriesCollection;
        private void InitChart()
        {
            Chart.XAxes = new[]
            {
                new Axis()
                {
                    Labeler = (value) => $"{(decimal)value * (decimal)chartXStep} λ"
                }
            };
            ChartSeriesCollection = new()
            {
                /* multi slit */
                new LineSeries<double>()
                {
                    LineSmoothness = 1,
                    GeometrySize = 0.1
                },
                /* single slit / cover */
                new LineSeries<double>()
                {
                    LineSmoothness = 1,
                    GeometrySize = 0.1,
                    Fill = null
                }
            };
        }

        private async void UpdateChart()
        {
            int spaltCount = (int)SlitCountNumberBox.Value;
            double ratio = spaltCount > 1 ? SlitRatioNumberBox.Value / 100.0 : 1.0;
            
            List<double> valueCollection = new();
            List<double> valueCollection2 = new();
            await Task.Run(() =>
            {
                for (double gangUnterschiedFactor = 0; gangUnterschiedFactor < 5; gangUnterschiedFactor += chartXStep)
                {
                    double singleSlitIntensity = CalculateSingleSlitIntensity(gangUnterschiedFactor, ratio);

                    double intensity = Math.Pow(CalculateResultingAmplitude(spaltCount, gangUnterschiedFactor, length, out _) / length, 2);
                    if (spaltCount > 1)
                        valueCollection.Add(intensity * singleSlitIntensity);

                    valueCollection2.Add(singleSlitIntensity * (valueCollection.Count > 0 ? valueCollection[0] : 100));
                }
            });

            ChartSeriesCollection[0].Values = valueCollection;
            ChartSeriesCollection[1].Values = valueCollection2;
        }
        #endregion

        #region Calculation
        const double length = 40;
        private double CalculateResultingAmplitude(int spaltCount, double gangUnterschiedFactor, double arrowLength, out Vector2[] arrows)
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

        private double CalculateSingleSlitIntensity(double gangUnterschiedFactor, double ratio = 0.25, double baseIntensity = 1.0)
        {
            if (gangUnterschiedFactor == 0)
                return baseIntensity;
            return baseIntensity * Math.Pow(sin(π * ratio * gangUnterschiedFactor) / (π * ratio * gangUnterschiedFactor), 2);
        }
        #endregion
    }
}