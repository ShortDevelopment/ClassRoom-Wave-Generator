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
        /// Sets wether the coordinate system is rendered in the background
        /// </summary>
        public bool ShowCoordinateSystem { get; set; } = true;

        /// <summary>
        /// Offset of all points
        /// </summary>
        public Vector2 Offset { get; set; } = new Vector2(0, 0);

        /// <summary>
        /// Sets if the renderer should stick all points to the right side
        /// </summary>
        public bool RTL { get; set; } = false;
    }
}
