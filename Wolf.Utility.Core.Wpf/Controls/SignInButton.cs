using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Wolf.Utility.Core.Authentication.GoogleInteraction;
using Wolf.Utility.Core.Wpf.Controls.Resources;
using Wolf.Utility.Core.Wpf.Core.Enums;
using Wolf.Utility.Core.Wpf.Extensions;

using Xceed.Wpf.Toolkit;

namespace Wolf.Utility.Core.Wpf.Controls
{
    public class SignInButton<T> : IconButton
    {
        private readonly SignInType type;
        private readonly Action<T> onClick;
        private readonly IServiceProvider? service;
        private readonly bool onlyGetProfileInfo;
        private readonly IEnumerable<ValidScopes>? scopes;

        /// <summary>
        /// Create an Sign in button, for signing in using Google.
        /// </summary>
        /// <param name="onClick">The action to perform upon clicking sign in, with the input from whatever sign in is performed.</param>
        /// <param name="service">The provider to retrieve services from used to login with.</param>
        /// <param name="onlyGetProfileInfo">Weather or not to only retrieve a limited set of profile info, or use <paramref name="scopes"/> in a more advanded login attempt.</param>
        /// <param name="scopes">Can be ignored, unless <paramref name="onlyGetProfileInfo"/> is set to <see langword="false"/>, otherwise an ArgumentException is thrown.</param>
        /// <typeparamref name="T">Should match the expected return type from performing a login attempt. Code checks for correct type beforehand, and throws ArgumentException if invalid type for the returned type. Should be of type 'GoogleProfile' if <paramref name="onlyGetProfileInfo"/> is <see langword="true"/>, which it is by default.</typeparamref>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="onClick"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="onlyGetProfileInfo"/> is true while <typeparamref name="T"/>is not 'GoogleProfile'. Thrown if <paramref name="onlyGetProfileInfo"/> is false and <paramref name="scopes"/> is null.</exception>
        public SignInButton(Action<T> onClick, IServiceProvider service, bool onlyGetProfileInfo = true, IEnumerable<ValidScopes>? scopes = null)
        {
            type = SignInType.Google;
            this.onClick = onClick ?? throw new ArgumentNullException(nameof(onClick));
            this.service = service;

            if (onlyGetProfileInfo && typeof(T) != typeof(GoogleProfile)) throw new ArgumentException($"As {nameof(onlyGetProfileInfo)} is true, T is expected to be of type {nameof(GoogleProfile)}, but was of type {typeof(T).FullName} instead.", $"Type parameter T");
            this.onlyGetProfileInfo = onlyGetProfileInfo;

            if (!onlyGetProfileInfo && scopes == null) throw new ArgumentException($"As {nameof(onlyGetProfileInfo)} is false while {nameof(scopes)} is null, an invalid combination has been created. A false {nameof(onlyGetProfileInfo)} indicating that a more special login is required, and therefore the {nameof(scopes)} cannot be null", nameof(scopes));
            else this.scopes = scopes;

            SetupGoogleButton();
        }

        private void SetupGoogleButton()
        {
            Icon = new Image() { Source = ImageConverter.ByteToImageSource(Icons.GoogleSignIn), Stretch = Stretch.Fill };
            Width = Icon.Width;
            ToolTip = new ToolTip() { Content = $"Sign In with Google" };
            //Content = $"<Placeholder>";
            Click += SignInButton_Click;

            //HorizontalContentAlignment = HorizontalAlignment.Stretch;

            //var clearBrush = new SolidColorBrush() { Opacity = 0 };
            //BorderBrush = clearBrush;
            //Background = clearBrush;
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            SignInButton_Clicked(sender, e);
        }

        private async Task SignInButton_Clicked(object sender, RoutedEventArgs e)
        {
            T? input = default;

            switch (type)
            {
                case SignInType.Null:
                    break;

                case SignInType.Homebrew:
                    break;

                case SignInType.Google:
                    input = await SignInGoogle();
                    break;

                default:
                    break;
            }

            onClick.Invoke(input);
        }

        private async Task<T?> SignInGoogle()
        {
            if (service == null) throw new ArgumentNullException($"To sign in with google an instance for {nameof(IServiceProvider)} is needed, but {nameof(service)} field to hold it was null");

            var proxy = service.GetService<GoogleProxy>();
            if (proxy == null) throw new ArgumentNullException($"To sign in with google, the program has to be configured to allow it during startup, as a {nameof(GoogleProxy)} is retrieved via the instance for {nameof(IServiceProvider)}");

            object result = default;

            if (onlyGetProfileInfo)
                result = await proxy.GetProfileInfo();
            else
            {
                await proxy.Login(scopes);
            }
            return (T)result;
        }
    }
}