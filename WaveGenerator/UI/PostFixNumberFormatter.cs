using Windows.Globalization.NumberFormatting;

namespace WaveGenerator.UI
{
    public class PostFixNumberFormatter : INumberParser, INumberFormatter2
    {
        public string PostFix { get; set; }

        private string PostFixFormatted { get => " " + PostFix; }

        public string FormatDouble(double value) => value.ToString() + PostFixFormatted;

        public string FormatInt(long value) => value.ToString() + PostFixFormatted;

        public string FormatUInt(ulong value) => value.ToString() + PostFixFormatted;

        public double? ParseDouble(string text) => double.Parse(text);

        public long? ParseInt(string text) => long.Parse(text);

        public ulong? ParseUInt(string text) => ulong.Parse(text);
    }
}
