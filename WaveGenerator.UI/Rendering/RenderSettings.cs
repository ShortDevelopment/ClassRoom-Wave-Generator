using System.Numerics;

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

        /// <summary>
        /// Offset of all points
        /// </summary>
        public Vector2 Offset { get; set; } = new Vector2(0, 0);

        #region Reflection

        public bool ShowIncomingWave { get; set; } = true;

        public bool ShowReflectedWave { get; set; } = true;

        public bool ShowResultingWave { get; set; } = true;

        #endregion

    }
}
