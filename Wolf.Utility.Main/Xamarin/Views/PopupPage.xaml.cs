using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wolf.Utility.Main.Xamarin.Views
{
    // Source: https://www.c-sharpcorner.com/article/alert-with-rg-plugins-popuppage/
    // Source: https://github.com/rotorgames/Rg.Plugins.Popup
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        private TaskCompletionSource<bool> taskCompletionSource;
        public Task PopupClosedTask => taskCompletionSource.Task;

        private readonly bool _shouldImplode = false;
        private readonly TimeSpan _lifeTime;

        public delegate void CallbackDelegate(bool wasSuccessful);
        public CallbackDelegate CallbackMethod { get; private set; }

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

            Animation = new ScaleAnimation()
            {
                PositionIn = MoveAnimationOptions.Top, PositionOut = MoveAnimationOptions.Top, 
                DurationIn = 400, DurationOut = 300, 
                EasingIn = Easing.SinOut, EasingOut = Easing.SinIn, 
                ScaleIn = 1.2, ScaleOut = 0.8, HasBackgroundAnimation = true
            };
        }

        public PopupPage(string message, CallbackDelegate callbackAction, (string yes, string no) buttonTexts, Color color = default)
        {
            InitializeComponent();
            var (yes, no) = buttonTexts;

            MessageLabel.Text = message;
            MainStack.BackgroundColor = color;
            CallbackMethod = callbackAction;

            Animation = new ScaleAnimation()
            {
                PositionIn = MoveAnimationOptions.Center, PositionOut = MoveAnimationOptions.Center,
                DurationIn = 400, DurationOut = 300,
                EasingIn = Easing.SinOut, EasingOut = Easing.SinIn,
                ScaleIn = 1.2, ScaleOut = 0.8, HasBackgroundAnimation = true
            };

            MainStack.VerticalOptions = LayoutOptions.Center;
            MainStack.HorizontalOptions = LayoutOptions.Center;
            MainStack.WidthRequest = 600;

            MainGrid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(50, GridUnitType.Star)});

            OkButton.Text = yes;
            OkButton.WidthRequest = 150;
            MainGrid.Children.Add(OkButton, 0, 1);

            CancelButton.Text = no;
            CancelButton.WidthRequest = 150;
            MainGrid.Children.Add(CancelButton, 1, 1);
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            taskCompletionSource = new TaskCompletionSource<bool>();

            if (!_shouldImplode) return;
            Implode();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            taskCompletionSource.SetResult(true);
        }

        private async void Implode()
        {
            await Task.Delay(_lifeTime);
            
            try { await Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(this); }
            catch (InvalidOperationException) { }
        }

        private void CloseButton_OnClicked(object sender, EventArgs e)
        {
            CallbackMethod?.Invoke(false);

            Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(this);
        }

        private void OkButton_OnClicked(object sender, EventArgs e)
        {
            CallbackMethod?.Invoke(true);

            Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(this);
        }

        protected override bool OnBackgroundClicked()
        {
            CallbackMethod?.Invoke(false);

            return base.OnBackgroundClicked();
        }
    }
}