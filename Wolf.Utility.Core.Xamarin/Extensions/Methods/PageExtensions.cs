using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;

using Wolf.Utility.Core.Deprecated.Logging.Enum;
using Wolf.Utility.Core.Deprecated.Logging;
using Wolf.Utility.Xamarin.Services;
using Xamarin.Forms;

namespace Wolf.Utility.Xamarin.Extensions.Methods
{
    public static class PageExtensions
    {
        public static void ShowPopup(this Page page, PopupPage popup, bool animated = true)
        {
            Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(popup, animated);
        }

        public static async Task CloseApp(this Page page, int delay = 10)
        {
            await Task.Delay(TimeSpan.FromSeconds(delay));

            var closer = DependencyService.Get<ICloseApplicationService>();
            if (closer == null)
                Logging.Log(LogType.Warning,
                    $"Was suppose to close app, but failed to get service to do so.");
            closer?.CloseApplication();
        }
    }
}
