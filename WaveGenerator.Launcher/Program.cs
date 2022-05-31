using ShortDev.Uwp.FullTrust.Core.Xaml;

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

                XamlWindowSubclass.ForWindow(window).CurrentFrameworkView!.Run();
            }
        }
    }
}
