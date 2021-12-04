using Microsoft.UI.Xaml;
using static WaveGenerator.UI.Interop.WindowIconInterop;

namespace WaveGenerator
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            Title = "Wave Generator";
            this.SetIcon("WaveGenerator.waves.ico");
        }
    }
}
