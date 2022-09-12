using System.Numerics;

namespace WaveGenerator.Rendering
{
    public class RenderSettings
    {
        /// <summary>
        /// Sets how many units fit onto the y-axis
        /// </summary>
        public double YStepCount { get; set; } = 2;

        /// <summary>
        /// Offset of all points
        /// </summary>
        public Vector2 Offset { get; set; } = new(0, 0);        

        #region Reflection

        public bool ShowIncomingWave { get; set; } = true;

        public bool ShowReflectedWave { get; set; } = true;

        public bool ShowResultingWave { get; set; } = true;

        #endregion

    }
}
