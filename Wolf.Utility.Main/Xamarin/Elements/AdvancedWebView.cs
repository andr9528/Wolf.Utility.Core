﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Wolf.Utility.Main.Logging.Enum;
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
                InjectJavaScript(GetJsClickEvent());

            RegisterCallback("invokeClick", el => {
                var args = new ClickEvent { Element = el };

                Clicked?.Invoke(this, args);
                ClickCommand?.Execute(args);
            });

        }

        public void WriteToField(string fieldId, string value)
        {
            var js = GetJsSetInputField(fieldId, value);

            InjectJavaScript(js);

            Logging.Logging.Log(LogType.Information, $"Injected Javascript => {js}");
        }
        

        #region JS Strings
        private string GetJsClickEvent()
        {
            var builder = new StringBuilder();

            builder.Append("var x = document.body.addEventListener('click', function(e) ");
            builder.Append("{");
            builder.Append("e = e || window.event;");
            builder.Append("var target = e.target || e.srcElement;");
            builder.Append("Native('invokeClick', 'tag='+target.tagName+' id='+target.id+' name='+target.name);");
            builder.Append("}, ");
            builder.Append("true /* to ensure we capture it first*/);");

            return builder.ToString();
        }

        private string GetJsSetInputField(string fieldId, string value)
        {
            var builder = new StringBuilder();

            //builder.Append($"if (document.getElementById(\"{fieldId}\"))");
            //builder.Append("{");
            builder.Append($"var input = document.getElementById(\"{fieldId}\");");
            builder.Append($"input.setAttribute(\"value\", \"{value}\";");
            //builder.Append("}");

            return builder.ToString();
        }
        #endregion

    }
}
