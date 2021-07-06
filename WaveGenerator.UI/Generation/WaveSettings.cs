namespace WaveGenerator.UI.Generation
{
    public class WaveSettings
    {
        /// <summary>
        /// Sets the amplitude (ŝ) of the wave
        /// </summary>
        public double Amplitude { get; set; } = 1;

        /// <summary>
        /// Sets the wavelength (λ)
        /// </summary>
        public double WaveLength { get; set; } = 5;

        /// <summary>
        /// Sets the period (T)
        /// </summary>
        public double Period { get; set; } = 6;

        /// <summary>
        /// Sets info about the reflection of the wave. "null" if the wave should ot be reflected.
        /// </summary>
        public WaveReflectionInfo Reflection { get; set; } = null;
    }
}
