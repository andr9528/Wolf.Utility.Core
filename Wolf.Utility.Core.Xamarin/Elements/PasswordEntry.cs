using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Wolf.Utility.Xamarin.Elements.Resources;
using Xamarin.Forms;

namespace Wolf.Utility.Xamarin.Elements
{
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    public class PasswordEntry : ContentView
    {
        private Entry PasswordHidden;
        private Entry PasswordShown;
        private Image PasswordButtonIcon;
        private Button PasswordButton;

        private Grid InnerGrid;
        private Grid OuterGrid;

        public string Text 
        {
            get => PasswordShown.IsVisible ? PasswordShown.Text : PasswordHidden.Text;
            set
            {
                if (PasswordShown.IsVisible) PasswordShown.Text = value;
                else PasswordHidden.Text = value;
            }
        } 

        private string ShownText => PasswordShown.Text;
        private string HiddenText => PasswordHidden.Text;
        
        public PasswordEntry(string placeholder)
        {
            CreateViewElements(placeholder);

            BindingContext = this;

            var (pathShown, pathHidden) = (nameof(ShownText), nameof(HiddenText));

            PasswordShown.SetBinding(Entry.TextProperty, pathHidden, BindingMode.TwoWay);
            PasswordHidden.SetBinding(Entry.TextProperty, pathShown, BindingMode.TwoWay);

            InnerGrid.Children.Add(PasswordButton, 0, 0);
            InnerGrid.Children.Add(PasswordButtonIcon, 0, 0);
            
            OuterGrid.Children.Add(PasswordHidden, 0, 0);
            OuterGrid.Children.Add(PasswordShown, 0, 0);
            OuterGrid.Children.Add(InnerGrid, 0, 0);

            Content = OuterGrid;
        }

        private void CreateViewElements(string placeholder)
        {
            PasswordHidden = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                IsPassword = true,
                Placeholder = placeholder,
            };

            PasswordShown = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                IsPassword = false,
                IsVisible = false,
                Placeholder = placeholder,
            };

            PasswordButtonIcon = new Image()
            {
                HeightRequest = 25,
                HorizontalOptions = LayoutOptions.Fill,
                InputTransparent = true,
                BackgroundColor = Color.Transparent,
                Source = ImageSource.FromStream(() => new MemoryStream(Icons.showpasswordicon)),
                VerticalOptions = LayoutOptions.Fill,
                WidthRequest = 25
            };

            PasswordButton = new Button()
            {
                BackgroundColor = Color.Transparent,
                Margin = new Thickness(-4, -6, -4, -6),
            };
            PasswordButton.Clicked += PasswordButton_Clicked;

            InnerGrid = new Grid()
            {
                Padding = new Thickness(0, 0, 3, 0),
                WidthRequest = 35,
                HeightRequest = 27,
                IsClippedToBounds = true,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                RowDefinitions = new RowDefinitionCollection() { new RowDefinition() },
                ColumnDefinitions = new ColumnDefinitionCollection() { new ColumnDefinition() }
            };

            OuterGrid = new Grid()
            {
                RowDefinitions = new RowDefinitionCollection() { new RowDefinition() },
                ColumnDefinitions = new ColumnDefinitionCollection() { new ColumnDefinition() }
            };
        }

        private void PasswordButton_Clicked(object sender, EventArgs e)
        {
            // Switch visibility of Password 
            // Entry field and Text Entry fields
            PasswordHidden.IsVisible = !PasswordHidden.IsVisible;
            PasswordShown.IsVisible = !PasswordShown.IsVisible;

            // update the Show/Hide button Icon states 
            if (PasswordShown.IsVisible)
            {
                // Password is not Visible state
                PasswordButtonIcon.Source = ImageSource.FromStream(() => new MemoryStream(Icons.showpasswordicon));

                // Setting up Entry curser focus
                PasswordShown.Focus();
                PasswordShown.Text = PasswordHidden.Text;
            }
            else
            {
                // Password is Visible state
                PasswordButtonIcon.Source = ImageSource.FromStream(() => new MemoryStream(Icons.hidepasswordicon));

                // Setting up Entry curser focus
                PasswordHidden.Focus();
                PasswordHidden.Text = PasswordShown.Text;
            }
        }
    }
}
