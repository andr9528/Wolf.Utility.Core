using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Wpf.Controls.Model;
using Wolf.Utility.Core.Wpf.Controls.Resources;
using Wolf.Utility.Core.Wpf.Extensions;

using Xceed.Wpf.Toolkit;

namespace Wolf.Utility.Core.Wpf.Controls
{
    public class NavigationWindow : Window
    {
        private readonly IEnumerable<NavigationInfo> navigations;
        private readonly IEnumerable<NavigationInfo> orderedNavigations;
        private readonly NavigationLocation location;
        private bool compacted = true;

        private Grid mainGrid = new Grid();
        private Grid navigationGrid = new Grid();
        private Grid hideableGrid = new Grid();
        private Frame pageViewer = new Frame();
        private IconButton burger = new IconButton();
        private IconButton hide = new IconButton();

        public enum NavigationLocation { Null, Top, Right, Bottom, Left}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="location">Where the navigation bar is located. 
        /// Top/Bottom doesn't support expanded version, and instead toggles between showing/hideing icons</param>
        /// <param name="compacted"></param>
        /// <exception cref="NullReferenceException"></exception>
        public NavigationWindow(IEnumerable<NavigationInfo> navigations, NavigationLocation location = NavigationLocation.Left, bool compacted = true)
        {
            if (location == NavigationLocation.Null)
                throw new NullReferenceException($"{nameof(location)} was set to null location which is invalid.");
            this.navigations = navigations;
            this.location = location;

            if (location == NavigationLocation.Top || location == NavigationLocation.Bottom)
                compacted = true;

            this.compacted = compacted;

            orderedNavigations = navigations.OrderBy(x => x.Desired);

            BuildWindow();
        }

        private void BuildWindow()
        {          
            SetupControls();
            SetControlPositions();
        }       

        private void SetupControls()
        {
            InitializeMainGrid();
            InitializeNavigationGrid();
            InitializeHideableGrid();

            pageViewer = new Frame()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Content = orderedNavigations.ToList().First().Content
            };

            burger = new IconButton()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Icon = new Image()
                {
                    Source = ImageConverter.ByteToImageSource(Icons.burgerIcon)
                }
            };
            burger.Click += Burger_Click;

            hide = new IconButton()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Icon = new Image()
                {
                    Source = ImageConverter.ByteToImageSource(Icons.showpasswordicon)
                }
            };
            hide.Click += Hide_Click;

        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            hideableGrid.Visibility = hideableGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        private void Burger_Click(object sender, RoutedEventArgs e)
        {
            compacted = !compacted;

            ResetupGrids();
        }

        private void ResetupGrids() 
        {
            SetupMainGridFromLocation();
            SetupNavigationGridFromLocation();
            SetupHideableGridFromLocation();
        }

        private void SetControlPositions()
        {
            switch (location)
            {
                case NavigationLocation.Top:
                    PlaceHorizontally();
                    break;
                case NavigationLocation.Right:
                    PlaceVertically();
                    break;
                case NavigationLocation.Bottom:
                    PlaceHorizontally();
                    break;
                case NavigationLocation.Left:
                    PlaceVertically();
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetControlPositions)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists");
            }
        }              

        private void PlaceHorizontally()
        {
            throw new NotImplementedException();
        }

        private void PlaceVertically()
        {
            throw new NotImplementedException();
        }

        #region Main Grid Setup
        private void InitializeMainGrid()
        {
            mainGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
            };

            SetupMainGridFromLocation();
        }
        private void SetupMainGridFromLocation()
        {
            switch (location)
            {
                case NavigationLocation.Top:
                    SetupHorizontalMainGrid(false);
                    break;
                case NavigationLocation.Right:
                    SetupVerticalMainGrid(false);
                    break;
                case NavigationLocation.Bottom:
                    SetupHorizontalMainGrid(true);
                    break;
                case NavigationLocation.Left:
                    SetupVerticalMainGrid(true);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupMainGridFromLocation)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists");
            }
        }
        private void SetupVerticalMainGrid(bool isLeftNavigation)
        {
            switch (isLeftNavigation)
            {
                case true:
                    switch (compacted)
                    {
                        case false:
                            SetupMainGrid(true, 15, 85);                           
                            break;
                        case true:
                            SetupMainGrid(true, 5, 95);
                            break;
                    }                    
                    break;
                case false:
                    switch (compacted)
                    {
                        case false:
                            SetupMainGrid(true, 85, 15);
                            break;
                        case true:
                            SetupMainGrid(true, 95, 5);
                            break;
                    }                    
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupVerticalMainGrid)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists"); ;
            }
        }
        private void SetupHorizontalMainGrid(bool isBottomNavigation)
        {
            switch (isBottomNavigation)
            {
                case true:
                    SetupMainGrid(false, 95, 5);
                    break;
                case false:
                    SetupMainGrid(false, 5, 95);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupHorizontalMainGrid)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists"); ;
            }
        }
        private void SetupMainGrid(bool isVertical, int sizeOne, int sizeTwo)
        {
            ClearDefinitions(isVertical, mainGrid);

            switch (isVertical)
            {
                case true:
                    mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(sizeOne, GridUnitType.Star) });
                    mainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(sizeTwo, GridUnitType.Star) });
                    break;
                case false:
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(sizeOne, GridUnitType.Star) });
                    mainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(sizeTwo, GridUnitType.Star) });
                    break;
            }
        }
        #endregion

        #region Navigation Grid Setup
        private void InitializeNavigationGrid()
        {
            navigationGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
            };

            SetupNavigationGridFromLocation();
        }
        private void SetupNavigationGridFromLocation()
        {
            switch (location)
            {
                case NavigationLocation.Top:
                    SetupHorizontalNavigationGrid(false);
                    break;
                case NavigationLocation.Right:
                    SetupVerticalNavigationGrid(false);
                    break;
                case NavigationLocation.Bottom:
                    SetupHorizontalNavigationGrid(true);
                    break;
                case NavigationLocation.Left:
                    SetupVerticalNavigationGrid(true);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupNavigationGridFromLocation)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists");
            }
        }
        private void SetupVerticalNavigationGrid(bool isLeftNavigation)
        {
            switch (isLeftNavigation)
            {
                case true:
                    switch (compacted)
                    {
                        case false:
                            SetupNavigationGrid(true, 15, 85);
                            break;
                        case true:
                            SetupNavigationGrid(true, 5, 95);
                            break;
                    }
                    break;
                case false:
                    switch (compacted)
                    {
                        case false:
                            SetupNavigationGrid(true, 85, 15);
                            break;
                        case true:
                            SetupNavigationGrid(true, 95, 5);
                            break;
                    }
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupVerticalNavigationGrid)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists"); ;
            }
        }
        private void SetupHorizontalNavigationGrid(bool isBottomNavigation)
        {
            switch (isBottomNavigation)
            {
                case true:
                    SetupNavigationGrid(false, 95, 5);
                    break;
                case false:
                    SetupNavigationGrid(false, 5, 95);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupHorizontalNavigationGrid)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists"); ;
            }
        }
        private void SetupNavigationGrid(bool isVertical, int sizeOne, int sizeTwo)
        {
            ClearDefinitions(isVertical, navigationGrid);

            switch (isVertical)
            {
                case true:
                    navigationGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(sizeOne, GridUnitType.Star) });
                    navigationGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(sizeOne, GridUnitType.Star) });
                    navigationGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(sizeTwo, GridUnitType.Star) });
                    break;
                case false:
                    navigationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(sizeOne, GridUnitType.Star) });
                    navigationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(sizeTwo, GridUnitType.Star) });
                    break;
            }
        }
        #endregion

        #region Setup Hideable Grid
        private void InitializeHideableGrid()
        {
            hideableGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
            };

            SetupHideableGridFromLocation();
        }
        private void SetupHideableGridFromLocation()
        {
            switch (location)
            {
                case NavigationLocation.Top:
                    SetupHideableGrid(false);
                    break;
                case NavigationLocation.Right:
                    SetupHideableGrid(true);
                    break;
                case NavigationLocation.Bottom:
                    SetupHideableGrid(false);
                    break;
                case NavigationLocation.Left:
                    SetupHideableGrid(true);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupHideableGridFromLocation)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists");
            }
        }      
        private void SetupHideableGrid(bool isVertical)
        {
            ClearDefinitions(isVertical, hideableGrid);

            switch (isVertical)
            {
                case true:
                    hideableGrid.RowDefinitions.AddAmount(orderedNavigations.Count());
                    break;
                case false:
                    hideableGrid.ColumnDefinitions.AddAmount(orderedNavigations.Count());
                    break;
            }
        }
        #endregion

        private void ClearDefinitions(bool isVertical, Grid grid)
        {
            if (isVertical)
                grid.RowDefinitions.Clear();
            else
                grid.ColumnDefinitions.Clear();
        }
    }
}
