namespace WaveGenerator.UI.Rendering
{
    public class RenderSettings
    {
        /// <summary>
        /// Sets radius of each circle in wave display
        /// </summary>
        public double Radius { get; set; } = 6;

        /// <summary>
        /// Sets how many units fit onto the y-axis
        /// </summary>
        public double YStepCount { get; set; } = 2;
    }
}
