using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator
{
    sealed partial class App : Application
    {
        public App()
        {
            UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
#if DEBUG
            if (Debugger.IsAttached)
                Debugger.Break();
#endif
            ContentDialog errorDialog = new();
            errorDialog.Title = e.Exception.GetType().FullName;
            errorDialog.Content = e.Exception.Message;
            errorDialog.PrimaryButtonText = "Ok";
            _ = errorDialog.ShowAsync();
        }
    }
}
