using ShortDev.Uwp.FullTrust.Core.Xaml;
using WinUI.Interop.NativeWindow;

namespace WaveGenerator.Launcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (XamlApplicationWrapper appWrapper = new(() => new App()))
            {
                var window = XamlWindowActivator.CreateNewWindow(new("WaveGenerator")
                {
                    HasWin32Frame = true
                });
                window.Content = new UI.MainPage();

                WindowExtensions.SetIcon(window.CoreWindow.GetHwnd(), "WaveGenerator.Launcher.Logo.ico");

                XamlWindowSubclass.ForWindow(window).CurrentFrameworkView!.Run();
            }
        }
    }
}
