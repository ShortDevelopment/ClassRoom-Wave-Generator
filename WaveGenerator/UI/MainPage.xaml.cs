using WaveGenerator.UI.Pages;
using Microsoft.UI.Xaml.Controls;

namespace WaveGenerator.UI
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            MainNavigationView.SelectedItem = MainNavigationView.MenuItems[0];
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            switch ((sender.SelectedItem as NavigationViewItem).Tag)
            {
                case "normal_wave":
                    MainContentFrame.Navigate(typeof(DefaultWavePage));
                    break;

                case "reflecting_waves":
                    MainContentFrame.Navigate(typeof(ReflectionWavePage));
                    break;

                case "reflecting_waves_infinite":
                    MainContentFrame.Navigate(typeof(InfiniteReflectionWavePage));
                    break;

                case "interferenz_2_waves":
                    MainContentFrame.Navigate(typeof(InterferencePage));
                    break;

                case "n_fach_spalt":
                    MainContentFrame.Navigate(typeof(SpaltPage));
                    break;

                default:
                    if (args.IsSettingsSelected)
                        MainContentFrame.Navigate(typeof(SettingsPage));
                    else
                        MainContentFrame.Navigate(typeof(ComingSoonPage));

                    break;
            }
        }
    }
}
