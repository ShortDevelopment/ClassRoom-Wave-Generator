using System;
using System.Collections.Generic;
using System.Timers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WaveGenerator.UI.Pages
{
    public sealed partial class WaveMediumSimulationPage : Page
    {
        public WaveMediumSimulationPage()
        {
            this.InitializeComponent();

            this.Loaded += WaveMediumSimulationPage_Loaded;
        }

        DispatcherTimer timer;
        private void WaveMediumSimulationPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            timer = new();
            timer.Interval = deltaTime;
            timer.Tick += Tick;

            RegenerateNodes();
        }

        double D = 10; // N / m;
        double m = 0.1; // kg;
        int nodesCount = 10;
        private void ResetAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                nodesCount = int.Parse(NodesCountTextBox.Text);
                D = double.Parse(FederHärteTextBox.Text);
                m = double.Parse(MassTextBox.Text);
                RegenerateNodes();
            }
            catch (Exception ex)
            {
            }
        }

        List<Node> nodes = new();
        void RegenerateNodes()
        {
            timer.Stop();
            nodes.Clear();
            NodesContainer.Children.Clear();

            for (int i = 0; i < nodesCount; i++)
            {
                Node node = new();

                nodes.Add(node);

                NodesContainer.Children.Add(node.Slider);
            }
            timer.Start();
        }

        TimeSpan deltaTime = TimeSpan.FromMilliseconds(100);
        void Tick(object sender, object e)
        {
            double lastElogation = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                var currentNode = nodes[i];
                double currentElongation = currentNode.Elongation;

                if (i != 0)
                {
                    double Δs = lastElogation - currentElongation;
                    double a = D * Δs / m; // m / s^2

                    currentNode.Velocity = a * deltaTime.TotalSeconds;
                    currentNode.Elongation += currentNode.Velocity * deltaTime.TotalSeconds;
                }

                lastElogation = currentElongation;
            }
        }

        class Node
        {
            public Slider Slider { get; } = new()
            {
                Orientation = Orientation.Vertical,

                Minimum = -100,
                Maximum = 100,
                Value = 0
            };

            public double Velocity { get; set; }
            public double Elongation
            {
                get => Slider.Value;
                set => Slider.Value = value;
            }
        }
    }
}
