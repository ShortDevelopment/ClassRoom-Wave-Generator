using Windows.UI.Xaml.Markup;

namespace WaveGenerator.UI
{
    public class Utils
    {
        public static T ConvertXamlValue<T>(object value) => (T)XamlBindingHelper.ConvertValue(typeof(T), value);
    }
}
