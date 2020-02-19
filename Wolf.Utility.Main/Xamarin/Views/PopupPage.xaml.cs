using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wolf.Utility.Main.Xamarin.Views
{
    // Source: https://www.c-sharpcorner.com/article/alert-with-rg-plugins-popuppage/
    // Source: https://github.com/rotorgames/Rg.Plugins.Popup
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        private readonly bool _shouldImplode = false;
        private readonly TimeSpan _lifeTime;

        public PopupPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// if it is set to implode, but no timespan is given, then it defaults to a 2 sec lifetime.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="lifeTime"></param>
        /// <param name="shouldImplode"></param>
        public PopupPage(string message, Color color, TimeSpan lifeTime = default, bool shouldImplode = false)
        {
            InitializeComponent();

            _shouldImplode = shouldImplode;

            if(shouldImplode && lifeTime == default)
                _lifeTime = TimeSpan.FromSeconds(2);
            else
                _lifeTime = lifeTime;

            MessageLabel.Text = message;
            MainStack.BackgroundColor = color;
        }

        private void CloseButton_OnClicked(object sender, EventArgs e)
        {
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(this);
        }

        public void AddCloseButton()
        {
            while (MainGrid.RowDefinitions.Count < 2)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Star)});
            }

            MainGrid.Children.Add(CloseButton, 0, 2 , 1,1);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!_shouldImplode) return;
            Implode();
        }

        private async void Implode()
        {
            await Task.Delay(_lifeTime);
            
            try { await Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(this); }
            catch (InvalidOperationException) { }
        }
    }
}