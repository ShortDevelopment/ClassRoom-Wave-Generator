using WaveGenerator.UI.Pages;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainContentFrame.Navigate(typeof(DefaultWavePage));
        }

        private void NavigationView_SelectionChanged_1(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            switch ((sender.SelectedItem as Microsoft.UI.Xaml.Controls.NavigationViewItem).Tag)
            {
                case "normal_wave":
                    MainContentFrame.Navigate(typeof(DefaultWavePage));
                    break;

                default:
                    MainContentFrame.Navigate(typeof(SettingsPage));
                    break;
            }
        }
    }
}
