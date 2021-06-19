using Microsoft.Toolkit.Win32.UI.XamlHost;
using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI
{
    sealed partial class App : XamlApplication
    {
        public App()
        {
            this.Initialize();
            this.InitializeComponent();
        }

        async void WebViewTest()
        {
            var x = new WebViewBrush();
            x.SourceName = "test";
            var wvp = new Windows.Web.UI.Interop.WebViewControlProcess();
            var wv = await wvp.CreateWebViewControlAsync((long)Process.GetCurrentProcess().MainWindowHandle, new Rect(new Point(0, 0), new Size(10, 10)));
            wv.IsVisible = false;
            wv.Navigate(new Uri("https://bing.com/"));
            //x.SetSource(wv);
            Debugger.Break();
        }

    }
}
