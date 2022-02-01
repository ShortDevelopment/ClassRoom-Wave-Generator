using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace WaveGenerator.UI
{
    public class FrameContentPage : Page
    {
        public FrameContentPage() : base() { }

        #region IsPageVisibleInFrame
        public bool IsPageVisibleInFrame { get; private set; } = true;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            IsPageVisibleInFrame = true;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            IsPageVisibleInFrame = false;
        }
        #endregion
    }
}
