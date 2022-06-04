using System;
using WaveGenerator.Rendering;
using WaveGenerator.UI.Pages;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace WaveGenerator.UI
{
    public sealed partial class ZeigerPage : Page
    {
        public SimulationPageBase BasePage { get; set; }

        public ZeigerPage(SimulationPageBase basePage)
        {
            this.BasePage = basePage;

            this.InitializeComponent();
            this.Loaded += ZeigerPage_Loaded;
        }

        DispatcherTimer timer = new();
        private void ZeigerPage_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }

        public double NodePosition { get; set; } = 0;
        private void NodePositionTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                e.Handled = true;
                if (double.TryParse(NodePositionTextBox.Text, out var position))
                    NodePosition = position;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            Generation.WaveGenerator generater = new(BasePage.WaveSettings);
            var angle = generater.CalculateZeigerAngle(NodePosition, BasePage.CurrentAnimationTime / 1000.0);

            WaveRenderer renderer = new(Canvas, BasePage.RenderSettings);
            renderer.ClearCanvas();

            double radius = BasePage.WaveSettings.Amplitude * renderer.YUnit;
            renderer.RenderZeiger(angle, new(0, 0), radius, renderAmplitudeLine: false);
        }

        private void IsWindowTransparentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (IsWindowTransparentCheckBox.IsChecked == true)
                this.Background = new SolidColorBrush(Colors.Transparent);
            else
                this.Background = App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush;
        }
    }
}
