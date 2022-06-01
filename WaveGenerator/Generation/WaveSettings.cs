using System;

namespace WaveGenerator.Generation
{
    public class WaveSettings
    {
        /// <summary>
        /// Gets the amplitude (ŝ) of the wave (absolute value)
        /// </summary>
        public double Amplitude
            => Math.Abs(MaxElongation);

        /// <summary>
        /// Sets the max elongation (ŝ) of the wave <br/>
        /// May be negative!
        /// </summary>
        public double MaxElongation { get; set; } = 1;

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

        public WaveGenerationMode GenerationMode { get; set; } = WaveGenerationMode.Alles;
    }
}
