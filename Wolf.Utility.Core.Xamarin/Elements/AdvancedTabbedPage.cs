using Xamarin.Forms;

namespace Wolf.Utility.Xamarin.Elements
{

    public class AdvancedTabbedPage : TabbedPage
    {
        public static readonly BindableProperty IsHiddenProperty = BindableProperty.Create("IsTabsHidden", typeof(bool), typeof(AdvancedTabbedPage), false);

        public bool IsTabsHidden
        {
            set { SetValue(IsHiddenProperty, value); }
            get { return (bool)GetValue(IsHiddenProperty); }
        }
    }
}
