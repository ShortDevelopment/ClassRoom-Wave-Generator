﻿using WaveGenerator.UI.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI.Controls
{
    public sealed partial class SimulationCommandBarControl : UserControl
    {
        public SimulationCommandBarControl()
        {
            this.InitializeComponent();
        }

        public SimulationPageBase BasePage { get; set; }

        private void PlayAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BasePage.IsRunning = true;

            PlayAppBarButton.IsEnabled = false;
            PauseAppBarButton.IsEnabled = true;

            ResetAppBarButton.IsEnabled = true;
        }

        private void PauseAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BasePage.IsRunning = false;

            PlayAppBarButton.IsEnabled = true;
            PauseAppBarButton.IsEnabled = false;
        }


        private void ResetAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BasePage.CurrentAnimationTime = 0;

            if (!BasePage.IsRunning)
                ResetAppBarButton.IsEnabled = false;
        }


        private void StepAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BasePage.InvokeSingleStep();
        }

        private void StepBackAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            BasePage.InvokeSingleStepReverse();
        }


        private void ZeigerAppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var coreAppView = FullTrustApplication.CreateNewView(new("Zeiger")
            {
                IsTopMost = true,
                HasTransparentBackground = true                
            });
            _ = coreAppView.CoreWindow.Dispatcher.RunIdleAsync(_ =>
            {
                var window = Window.Current;
                window.Content = new ZeigerPage(BasePage);
                window.Activate();
            });
        }
    }
}
