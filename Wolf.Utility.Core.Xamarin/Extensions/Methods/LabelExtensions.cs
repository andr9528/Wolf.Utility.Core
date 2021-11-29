using System;
using System.Collections.Generic;
using System.Text;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace Wolf.Utility.Xamarin.Extensions.Methods
{
    public static class LabelExtensions
    {
        public static void AddTapGestureDescription(this Label label, Page page, PopupPage popup, bool isAnimated = true)
        {
            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (sender, args) =>
            {
                page.ShowPopup(popup); 
            };
            label.GestureRecognizers.Add(tgr);
        }
    }
}
