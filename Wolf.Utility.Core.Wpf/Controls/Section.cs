using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

using Wolf.Utility.Core.Wpf.Controls.Resources;
using Wolf.Utility.Core.Wpf.Extensions;

namespace Wolf.Utility.Core.Wpf.Controls
{
    public class Section : Grid
    {
        public delegate void SizeChanging(EventArgs args);

        public event SizeChanging? IsExpanding;
        public event SizeChanging? IsCollapsing;

        private readonly string headerText = string.Empty;
        private Grid hideable = new Grid();

        public bool Collapsed => collapsed;
        public Label HeaderLabel => headerLabel;
        public Button CollapseButton => collapseButton;
        public Button ExpandButton => expandButton;
        public Image ButtonImage => buttonImage;

        private bool collapsed = true;
        private Label headerLabel = new Label();
        private Button collapseButton = new Button();
        private Button expandButton = new Button();
        private Image buttonImage = new Image();


        public Section(string headerText, UIElementCollection uIElements, bool collapsed = true, 
            HorizontalAlignment horizontal = HorizontalAlignment.Stretch, VerticalAlignment vertical = VerticalAlignment.Center)
        {
            this.headerText = headerText;
            this.collapsed = collapsed;

            HorizontalAlignment = horizontal;
            VerticalAlignment = vertical;

            SetupGrids(uIElements);
        }

        private void SetupGrids(UIElementCollection uIElement)
        {
            RowDefinitions.AddAmount(2);

            SetupHeader();

            hideable = new Grid() 
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Collapsed ? Visibility.Collapsed : Visibility.Visible,
            };
            hideable.RowDefinitions.AddAmount(uIElement.Count);

            for (int i = 0; i < uIElement.Count; i++) 
            {
                SetRow(uIElement[i], i);
                hideable.Children.Add(uIElement[i]);
            }
            SetRow(hideable, 1);
            Children.Add(hideable);

            SetVisibility(Collapsed);
        }

        private void SetupHeader()
        {
            headerLabel = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Content = headerText
            };
            SetRow(headerLabel, 0);
            Children.Add(headerLabel);

            buttonImage = new Image()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 25, 0),
                Height = 25,
                Width = 25,
            };
            SetRow(buttonImage, 0);
            Children.Add(buttonImage);

            collapseButton = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0,0,25,0),                
            };
            collapseButton.Click += CollapseButton_Click;
            SetRow(collapseButton, 0);
            Children.Add(collapseButton);

            expandButton = new Button()
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 25, 0)
            };
            expandButton.Click += ExpandButton_Click;
            SetRow(expandButton, 0);
            Children.Add(expandButton);      
        }   

        private void CollapseButton_Click(object sender, RoutedEventArgs e)
        {
            collapsed = !Collapsed;
            SetVisibility(Collapsed);
            IsCollapsing?.Invoke(e);
        }

        private void ExpandButton_Click(object sender, RoutedEventArgs e)
        {
            collapsed = !Collapsed;
            SetVisibility(Collapsed);
            IsExpanding?.Invoke(e);
        }

        private void SetVisibility(bool collapsed) 
        {
            switch (collapsed)
            {
                case false:
                    buttonImage.Source = ImageConverter.ByteToImageSource(Icons.arrowopen);
                    hideable.Visibility = Visibility.Visible;

                    collapseButton.Visibility = Visibility.Visible;
                    expandButton.Visibility = Visibility.Collapsed;
                    break;

                case true:
                    buttonImage.Source = ImageConverter.ByteToImageSource(Icons.arrowclosed);
                    hideable.Visibility = Visibility.Collapsed;

                    collapseButton.Visibility = Visibility.Collapsed;
                    expandButton.Visibility = Visibility.Visible;
                    break;
            }

                
        }
    }
}
