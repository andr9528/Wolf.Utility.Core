using System;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Wolf.Utility.Xamarin.Elements;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Wolf.Utility.Xamarin.Views
{
    // Source: https://www.c-sharpcorner.com/article/alert-with-rg-plugins-popuppage/
    // Source: https://github.com/rotorgames/Rg.Plugins.Popup
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AdvancedPopupPage : Rg.Plugins.Popup.Pages.PopupPage
    {
        private TaskCompletionSource<bool> taskCompletionSource;
        public Task PopupClosedTask => taskCompletionSource.Task;

        private readonly bool _shouldImplode = false;
        private readonly TimeSpan _lifeTime;

        public delegate void CallbackConfirmationDelegate(bool wasSuccessful);
        public CallbackConfirmationDelegate CallbackConfirmationMethod { get; private set; }

        public delegate void CallbackLoginDelegate(bool save, (string username, string password) user);
        public CallbackLoginDelegate CallbackLoginMethod { get; private set; }

        private PasswordEntry passwordEntry;


        /// <summary>
        /// Use this one to display a notification in the top of device.
        /// If it is set to implode, but no timespan is given, then it defaults to a 2 sec lifetime. 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="lifeTime"></param>
        /// <param name="shouldImplode"></param>
        public AdvancedPopupPage(string message, Color color, TimeSpan lifeTime = default, bool shouldImplode = false)
        {
            InitializeComponent();

            _shouldImplode = shouldImplode;

            if(shouldImplode && lifeTime == default)
                _lifeTime = TimeSpan.FromSeconds(2);
            else
                _lifeTime = lifeTime;
            
            MainStack.BackgroundColor = color;

            Animation = new ScaleAnimation()
            {
                PositionIn = MoveAnimationOptions.Top,
                PositionOut = MoveAnimationOptions.Top,
                DurationIn = 400,
                DurationOut = 300,
                EasingIn = Easing.SinOut,
                EasingOut = Easing.SinIn,
                ScaleIn = 1.2,
                ScaleOut = 0.8,
                HasBackgroundAnimation = true
            };

            SetupStack(LayoutOptions.Start, LayoutOptions.CenterAndExpand);
            SetupGrid(1, 1);

            MessageLabel.Text = message;
            MainGrid.Children.Add(MessageLabel, 0,0);
        }

        /// <summary>
        /// Use this one to display a confirmation notification in the center of the screen.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callbackConfirmationAction"></param>
        /// <param name="buttonTexts"></param>
        /// <param name="color"></param>
        public AdvancedPopupPage(string message, CallbackConfirmationDelegate callbackConfirmationAction, (string yes, string no) buttonTexts, Color color = default)
        {
            InitializeComponent();
            var (yes, no) = buttonTexts;
            
            MainStack.BackgroundColor = color;
            CallbackConfirmationMethod = callbackConfirmationAction;

            Animation = new ScaleAnimation()
            {
                PositionIn = MoveAnimationOptions.Center, 
                PositionOut = MoveAnimationOptions.Center,
                DurationIn = 400, 
                DurationOut = 300,
                EasingIn = Easing.SinOut, 
                EasingOut = Easing.SinIn,
                ScaleIn = 1.2, 
                ScaleOut = 0.8, 
                HasBackgroundAnimation = true
            };

            SetupStack(LayoutOptions.Center, LayoutOptions.Center,600);
            SetupGrid(2, 2);

            MessageLabel.Text = message;
            MainGrid.Children.Add(MessageLabel, 0, 2, 0, 1);

            OkButton.Text = yes;
            OkButton.WidthRequest = 150;
            MainGrid.Children.Add(OkButton, 0, 1);

            CancelButton.Text = no;
            CancelButton.WidthRequest = 150;
            MainGrid.Children.Add(CancelButton, 1, 1);
        }
        
        /// <summary>
        /// Use this one to request Login information.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callbackLoginAction"></param>
        /// <param name="successButton"></param>
        /// <param name="saveUserLabel"></param>
        /// <param name="placeholders"></param>
        /// <param name="color"></param>
        public AdvancedPopupPage(string message, CallbackLoginDelegate callbackLoginAction, string successButton,
            string saveUserLabel, (string username, string password) placeholders, Color color = default)
        {
            InitializeComponent();

            var (usernameHolder, passwordHolder) = placeholders;

            MainStack.BackgroundColor = color;
            CallbackLoginMethod = callbackLoginAction;

            Animation = new ScaleAnimation()
            {
                PositionIn = MoveAnimationOptions.Center,
                PositionOut = MoveAnimationOptions.Center,
                DurationIn = 400,
                DurationOut = 300,
                EasingIn = Easing.SinOut,
                EasingOut = Easing.SinIn,
                ScaleIn = 1.2,
                ScaleOut = 0.8,
                HasBackgroundAnimation = true
            };

            SetupStack(LayoutOptions.Center, LayoutOptions.Center, 800);
            SetupGrid(5, 2);

            MessageLabel.Text = message;
            MainGrid.Children.Add(MessageLabel, 0, 0 );
            Grid.SetColumnSpan(MessageLabel, 2);

            UsernameEntry.Placeholder = usernameHolder;
            UsernameEntry.WidthRequest = 500;
            MainGrid.Children.Add(UsernameEntry, 0, 1);
            Grid.SetColumnSpan(UsernameEntry, 2);

            passwordEntry = new PasswordEntry(passwordHolder)
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 500
            };
            MainGrid.Children.Add(passwordEntry, 0,2);
            Grid.SetColumnSpan(passwordEntry, 2);
            
            SaveUserLabel.Text = saveUserLabel;
            SaveUserLabel.WidthRequest = 150;
            MainGrid.Children.Add(SaveUserLabel, 0, 3);

            MainGrid.Children.Add(SaveUserSwitch, 1, 3);

            OkButton.Text = successButton;
            OkButton.WidthRequest = 300;
            MainGrid.Children.Add(OkButton, 0, 4);
            Grid.SetColumnSpan(OkButton, 2);
        }

        private void SetupGrid(int rows, int columns)
        {
            if(rows < 1) throw new ArgumentException($@"There has to exist at least 1 row", nameof(rows));
            if(columns < 1) throw new ArgumentException($@"There has to exist at least 1 column", nameof(columns));

            for (var i = 0; i < columns; i++)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50, GridUnitType.Star) });
            }

            for (var i = 0; i < rows; i++)
            {
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Star) });
            }
        }

        private void SetupStack(LayoutOptions verticalAlignment, LayoutOptions horizontalAlignment,  double request = default)
        {
            MainStack.VerticalOptions = verticalAlignment;
            MainStack.HorizontalOptions = horizontalAlignment;

            if (request != default) MainGrid.WidthRequest = request;
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
            CallbackConfirmationMethod?.Invoke(false);

            Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(this);
        }

        private void OkButton_OnClicked(object sender, EventArgs e)
        {
            CallbackConfirmationMethod?.Invoke(true);
            CallbackLoginMethod?.Invoke(SaveUserSwitch.IsToggled, (UsernameEntry.Text, passwordEntry?.Text));

            Rg.Plugins.Popup.Services.PopupNavigation.Instance.RemovePageAsync(this);
        }

        protected override bool OnBackgroundClicked()
        {
            CallbackConfirmationMethod?.Invoke(false);
            CallbackLoginMethod?.Invoke(SaveUserSwitch.IsToggled, (UsernameEntry.Text, passwordEntry?.Text));
            
            return base.OnBackgroundClicked();
        }
    }
}