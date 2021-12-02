using System;
using System.Threading.Tasks;
using WaveGenerator.UI.Generation;
using WaveGenerator.UI.Rendering;

namespace WaveGenerator.UI.Pages
{
    public abstract class SimulationPageBase : Interop.FrameContentPage
    {

        public SimulationPageBase CurrentInstance { get => this; }

        public SimulationPageBase()
        {
            // this.NavigationCacheMode = Microsoft.UI.Xaml.Navigation.NavigationCacheMode.Required;
            this.Loaded += SimulationPageBase_Loaded;
        }

        #region Load

        private void SimulationPageBase_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Task.Run(RenderLoop);

            OnLoaded();

            this.Loaded -= SimulationPageBase_Loaded;
        }

        protected abstract void OnLoaded();

        #endregion

        #region State Variables

        /// <summary>
        /// Gets wether animation is running
        /// </summary>
        public bool IsRunning { get; set; } = false;

        /// <summary>
        /// Time in ms
        /// </summary>
        public int CurrentAnimationTime { get; set; } = 0;

        #endregion

        #region Settings

        public WaveSettings WaveSettings { get; protected set; } = new WaveSettings();
        public RenderSettings RenderSettings { get; protected set; } = new RenderSettings();

        #endregion

        /// <summary>
        /// Handles rendering calls and time
        /// </summary>
        protected abstract void RenderLoop();

        #region Functions

        public void InvokeSingleStep()
        {
            CurrentAnimationTime += (int)Math.Round((1f / 16f) * WaveSettings.Period * 1000);
        }
        public void InvokeSingleStepReverse()
        {
            CurrentAnimationTime -= (int)Math.Round((1f / 16f) * WaveSettings.Period * 1000);
        }

        #endregion
    }
}
