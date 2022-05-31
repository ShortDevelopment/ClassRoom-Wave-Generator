using Microsoft.UI.Xaml;
using WinUI.Interop.NativeWindow;

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
