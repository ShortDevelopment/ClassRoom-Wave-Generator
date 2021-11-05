using System;
using System.Numerics;
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
        }

        private void ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
        {
            if (ZeigerCanvas == null)
                return;

            // Clear Canvas
            ZeigerCanvas.Children.Clear();

            const double l = 100;
            Vector2 lastPosition = new Vector2(0, 0);
            for (int i = 0; i < SpaltCount.Value; i++)
            {
                Line arrow = new Line();
                arrow.Stroke = new SolidColorBrush(Colors.Black);
                arrow.StrokeThickness = 1;

                arrow.X1 = lastPosition.X;
                arrow.Y1 = lastPosition.Y;

                #region Math
                const double π = Math.PI;
                Func<double, double> sin = (double a) => Math.Sin(a);
                Func<double, double> cos = (double a) => Math.Cos(a);

                double Δs = GangUnterschied.Value;
                double α = -i * Δs * 2 * π;
                lastPosition += new Vector2((float)(cos(α) * l), (float)(sin(α) * l));
                #endregion

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
    }
}
