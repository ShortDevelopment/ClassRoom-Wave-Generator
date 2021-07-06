namespace WaveGenerator.UI.Generation
{
    public class WaveReflectionInfo
    {
        public bool HasFreeEnd { get; set; } = false;

        /// <summary>
        /// Sets the position of the end were the wave should be reflected
        /// </summary>
        public double EndPosition { get; set; } = 10;
    }
}
