using System;
using System.Runtime.InteropServices;
using WaveGenerator.Rendering;
using WaveGenerator.UI.Pages;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using WinUI.Interop.CoreWindow;

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

            var hwnd = Window.Current.GetHwnd();
            PostMessage(hwnd, 0x270, 0, 1);
        }

        [DllImport("User32.Dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        DispatcherTimer timer = new();
        private void ZeigerPage_Loaded(object sender, RoutedEventArgs e)
        {
            _renderer = new(Canvas, BasePage.RenderSettings);

            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(25);
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

        WaveRenderer _renderer;
        private void Timer_Tick(object sender, object e)
        {
            Generation.WaveGenerator generater = new(BasePage.WaveSettings);
            var angle = generater.CalculateZeigerAngle(
                NodePosition,
                BasePage.CurrentAnimationTime / 1000.0,
                useReflectedWave: UseReflectedWaveCheckBox.IsChecked == true
            );
            
            _renderer.Clear();

            double radius = BasePage.WaveSettings.Amplitude * _renderer.YUnit;
            _renderer.VisibleZeiger.Add(new(angle, new(0, 0), (float)radius)
            {
                ShowAmplitude = false
            });

            _renderer.Render();
        }

        private void IsWindowTransparentToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {

            if (IsWindowTransparentToggleSwitch.IsOn)
                this.Background = new SolidColorBrush(Colors.Transparent);
            else
                this.Background = App.Current.Resources["ApplicationPageBackgroundThemeBrush"] as Brush;
        }
    }
}
