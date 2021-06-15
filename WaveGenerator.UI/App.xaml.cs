using Microsoft.Toolkit.Win32.UI.XamlHost;

namespace WaveGenerator.UI
{
    sealed partial class App : XamlApplication
    {
        public App()
        {
            this.Initialize();
            this.InitializeComponent();
        }

    }
}
