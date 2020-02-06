using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Wolf.Utility.Main.Xamarin.Elements.Events;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace Wolf.Utility.Main.Xamarin.Elements
{
    // Source : https://stackoverflow.com/questions/30397090/xamarin-forms-handle-clicked-event-on-webview

    public class AdvancedWebView : HybridWebView
    {
        public event EventHandler<ClickEvent> Clicked;

        public static readonly BindableProperty ClickCommandProperty =
            BindableProperty.Create("ClickCommand", typeof(ICommand), typeof(AdvancedWebView), null);

        public ICommand ClickCommand
        {
            get { return (ICommand)GetValue(ClickCommandProperty); }
            set { SetValue(ClickCommandProperty, value); }
        }

        public AdvancedWebView()
        {
            LoadFinished += (sender, e) =>
                InjectJavaScript(@"var x = 
            document.body.addEventListener('click', function(e) {
                e = e || window.event;
                var target = e.target || e.srcElement;
                Native('invokeClick', 'tag='+target.tagName+' id='+target.id+' name='+target.name);
            }, true /* to ensure we capture it first*/);
        ");

            RegisterCallback("invokeClick", el => {
                var args = new ClickEvent { Element = el };

                Clicked?.Invoke(this, args);
                ClickCommand?.Execute(args);
            });
        }
    }
}
