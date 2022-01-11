using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Logging;
using Wolf.Utility.Core.Wpf.Controls.Model;
using Wolf.Utility.Core.Wpf.Controls.Resources;
using Wolf.Utility.Core.Wpf.Core.Enums;
using Wolf.Utility.Core.Wpf.Extensions;

using Xceed.Wpf.Toolkit;

namespace Wolf.Utility.Core.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for NavigationPage.xaml
    /// </summary>
    public partial class NavigationPage : Page
    {
        public delegate void NavigateDelegate(Page page);
        public event NavigateDelegate Navigate;

        private IEnumerable<NavigationInfo>? navigations = new List<NavigationInfo>();
        private IEnumerable<NavigationInfo>? orderedNavigations = new List<NavigationInfo>();
        private readonly ILoggerManager? logger;
        private NavigationLocation location;
        private bool hidden = true;

        private Grid mainGrid = new Grid();
        private Grid navigationGrid = new Grid();
        private Grid hideableGrid = new Grid();
        private Frame pageViewer = new Frame();
        private IconButton burger = new IconButton();
        private IconButton back = new IconButton();

        private Stack<NavigationInfo> History = new Stack<NavigationInfo>();
        private Stack<Page> SubHistory = new Stack<Page>();
        public Stack<NavigationInfo> GetHistory => History;

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navigations">A List or similar containing NavigationInfo's used to setup navigation to different pages.</param>
        /// <param name="location">Where the navigation bar is located.</param>
        /// <param name="hidden">Wheather or not to start with the menu shown or not.</param>
        /// <param name="logger">The logger used to log during setup and when event happen. If non is supplied no logging happens.</param>
        /// <exception cref="NullReferenceException"></exception>
        public NavigationPage(IEnumerable<NavigationInfo> navigations,
                              ILoggerManager? logger = default,
                              NavigationLocation location = NavigationLocation.Left,
                              bool hidden = true) : base()
        {
            InitializeComponent();

            if (location == NavigationLocation.Null)
                throw new NullReferenceException($"{nameof(location)} was set to null location which is invalid.");
            this.navigations = navigations ?? throw new ArgumentNullException(nameof(navigations));
            this.logger = logger;
            this.location = location;
            this.hidden = hidden;

            this.logger?.SetCaller(nameof(NavigationPage));

            mainGrid = MainGrid;

            orderedNavigations = navigations.OrderBy(x => x.Desired);

            Navigate += NavigationPage_Navigate; 

            BuildWindow();
        }

        private void NavigationPage_Navigate(Page page)
        {
            SubHistory.Push(page);
            SetPageInFrame(page);
        }

        private void BuildWindow()
        {
            logger?.LogInfo($"Started buildind {nameof(NavigationPage)}.");
            SetupControls();
            SetControlPositions();
            logger?.LogInfo($"Finished building {nameof(NavigationPage)}.");
        }

        #region Initialize Elements
        private void SetupControls()
        {
            logger?.LogInfo($"Initializing Main Grid...");
            InitializeMainGrid();

            logger?.LogInfo($"Initializing Navigation Grid...");
            InitializeNavigationGrid();

            logger?.LogInfo($"Initializing Hideable Grid...");
            InitializeHideableGrid();

            pageViewer = new Frame()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Content = orderedNavigations.ToList().First().Content               
            };
            //pageViewer.Background = Brushes.Red;

            burger = new IconButton()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Icon = new Image()
                {
                    Source = ImageConverter.ByteToImageSource(Icons.burgerIcon)
                }
            };
            burger.Click += Burger_Click;

            back = new IconButton()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Icon = new Image()
                {
                    Source = ImageConverter.ByteToImageSource(Icons.arrowleft)
                }
            };
            back.Click += Back_Click; ;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (SubHistory.Count > 0)
            {
                SubHistory.Pop();
                if (SubHistory.Count > 0)
                {
                    var last = SubHistory.Peek();
                    logger?.LogInfo($"Returning to '{last.Title}'.");
                    SetPageInFrame(last);
                }
                else 
                {
                    var last = History.Peek();
                    logger?.LogInfo($"Returning to '{last.Title}'.");
                    SetPageInFrame(last);
                }
                
            }
            else if (History.Count > 1) 
            {
                History.Pop();
                var last = History.Peek();
                if (last.Content.Equals(pageViewer.Content)) return;
                logger?.LogInfo($"Returning to '{last.Title}'.");
                SetPageInFrame(last);
            }
                        
        }

        private void Burger_Click(object sender, RoutedEventArgs e)
        {
            hidden = !hidden;

            burger.Icon = new Image()
            {
                Source = ImageConverter.ByteToImageSource(hidden ? Icons.burgerIcon : Icons.arrowopen)
            };

            hideableGrid.Visibility = hideableGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
        #endregion

        #region Position Elements

        private void SetControlPositions()
        {
            switch (location)
            {
                case NavigationLocation.Top:
                    PlaceHorizontally(true);
                    break;
                case NavigationLocation.Right:
                    PlaceVertically(false);
                    break;
                case NavigationLocation.Bottom:
                    PlaceHorizontally(false);
                    break;
                case NavigationLocation.Left:
                    PlaceVertically(true);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetControlPositions)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists");
            }
        }

        private void PlaceHorizontally(bool isTopPositioned)
        {
            var orderedArray = orderedNavigations.ToArray();
            for (int i = 0; i < orderedNavigations.Count(); i++)
            {
                var nav = orderedArray[i];
                var button = CreateNavigationButton(nav);

                Grid.SetColumn(button, i);
                hideableGrid.Children.Add(button);
            }

            var first = orderedArray.First();
            logger?.LogInfo($"Setting the first shown page to '{first.Title}'");
            History.Push(first);
            SetPageInFrame(first);

            Grid.SetColumn(burger, 0);
            navigationGrid.Children.Add(burger);

            Grid.SetColumn(back, 1);
            navigationGrid.Children.Add(back);

            Grid.SetColumn(hideableGrid, 2);
            navigationGrid.Children.Add(hideableGrid);

            if(isTopPositioned)
            {
                Grid.SetRow(navigationGrid, 0);
                mainGrid.Children.Add(navigationGrid);

                Grid.SetRow(pageViewer, 1);
                mainGrid.Children.Add(pageViewer);
            }
            else
            {
                Grid.SetRow(pageViewer, 0);
                mainGrid.Children.Add(pageViewer);

                Grid.SetRow(navigationGrid, 1);
                mainGrid.Children.Add(navigationGrid);
            }
        }

        private void PlaceVertically(bool isLeftPositioned)
        {
            var orderedArray = orderedNavigations.ToArray();
            for (int i = 0; i < orderedNavigations.Count(); i++)
            {
                var nav = orderedArray[i];
                var button = CreateNavigationButton(nav);

                Grid.SetRow(button, i);
                hideableGrid.Children.Add(button);
            }

            var first = orderedArray.First();
            logger?.LogInfo($"Setting the first shown page to '{first.Title}'");
            History.Push(first);
            SetPageInFrame(first);

            Grid.SetRow(burger, 0);
            navigationGrid.Children.Add(burger);

            Grid.SetRow(back, 1);
            navigationGrid.Children.Add(back);

            Grid.SetRow(hideableGrid, 2);
            navigationGrid.Children.Add(hideableGrid);

            if (isLeftPositioned) 
            {
                Grid.SetColumn(navigationGrid, 0);
                mainGrid.Children.Add(navigationGrid);

                Grid.SetColumn(pageViewer, 1);
                mainGrid.Children.Add(pageViewer);
            }
            else
            {
                Grid.SetColumn(pageViewer, 0);
                mainGrid.Children.Add(pageViewer);

                Grid.SetColumn(navigationGrid, 1);
                mainGrid.Children.Add(navigationGrid);
            }

            
        }

        private IconButton CreateNavigationButton(NavigationInfo info)
        {
            var button = new IconButton()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Content = info.Title,
            };
            if (info.HasIcon) button.Icon = new Image() { Source = info.Icon };
            button.Click += (s, e) =>
            {
                SubHistory.Clear();
                History.Push(info);
                logger?.LogInfo($"Moving on to '{info.Title}'.");
                SetPageInFrame(info);
            };

            return button;
        }

        #endregion

        #region Main Grid Setup
        private void InitializeMainGrid()
        {
            //mainGrid = new Grid()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //};

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
                    switch (hidden)
                    {
                        case false:
                            CreateRowsOrColumnForMainGrid(false, 15, 85);
                            break;
                        case true:
                            CreateRowsOrColumnForMainGrid(false, 5, 95);
                            break;
                    }
                    break;
                case false:
                    switch (hidden)
                    {
                        case false:
                            CreateRowsOrColumnForMainGrid(false, 85, 15);
                            break;
                        case true:
                            CreateRowsOrColumnForMainGrid(false, 95, 5);
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
                    CreateRowsOrColumnForMainGrid(true, 95, 5);
                    break;
                case false:
                    CreateRowsOrColumnForMainGrid(true, 5, 95);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupHorizontalMainGrid)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists"); ;
            }
        }
        private void CreateRowsOrColumnForMainGrid(bool createRows, int sizeOne, int sizeTwo)
        {
            ClearDefinitions(createRows, mainGrid);

            switch (createRows)
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
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            SetupNavigationGridFromLocation();
        }
        private void SetupNavigationGridFromLocation()
        {
            switch (location)
            {
                case NavigationLocation.Top:
                    SetupNavigationGridFromPosition(isTopOrBottomNavigation: true);
                    break;
                case NavigationLocation.Right:
                    SetupNavigationGridFromPosition(isLeftNavigation: false);
                    break;
                case NavigationLocation.Bottom:
                    SetupNavigationGridFromPosition(isTopOrBottomNavigation: true);
                    break;
                case NavigationLocation.Left:
                    SetupNavigationGridFromPosition(isLeftNavigation: true);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupNavigationGridFromLocation)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists");
            }
        }

        private void SetupNavigationGridFromPosition(bool isTopOrBottomNavigation = false, bool isLeftNavigation = false) 
        {
            if (isTopOrBottomNavigation) CreateRowsOrColumnForNavigationGrid(false, 5, 95);
            else if (isLeftNavigation) CreateRowsOrColumnForNavigationGrid(true, 5, 95);              
            else CreateRowsOrColumnForNavigationGrid(true, 95, 5);
            
        }
        private void CreateRowsOrColumnForNavigationGrid(bool createRows, int sizeOne, int sizeTwo)
        {
            ClearDefinitions(createRows, navigationGrid);

            switch (createRows)
            {
                case true:
                    navigationGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(sizeOne, GridUnitType.Star) });
                    navigationGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(sizeOne, GridUnitType.Star) });
                    navigationGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(sizeTwo, GridUnitType.Star) });
                    break;
                case false:
                    navigationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(sizeOne, GridUnitType.Star) });
                    navigationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(sizeOne, GridUnitType.Star) });
                    navigationGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(sizeTwo, GridUnitType.Star) });
                    break;
            }
        }
        #endregion

        #region Hideable Grid Setup
        private void InitializeHideableGrid()
        {
            hideableGrid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            hideableGrid.Visibility = hideableGrid.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

            SetupHideableGridFromLocation();
        }
        private void SetupHideableGridFromLocation()
        {
            switch (location)
            {
                case NavigationLocation.Top:
                    CreateRowsOrColumnForHideableGrid(false);
                    break;
                case NavigationLocation.Right:
                    CreateRowsOrColumnForHideableGrid(true);
                    break;
                case NavigationLocation.Bottom:
                    CreateRowsOrColumnForHideableGrid(false);
                    break;
                case NavigationLocation.Left:
                    CreateRowsOrColumnForHideableGrid(true);
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(SetupHideableGridFromLocation)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists");
            }
        }
        private void CreateRowsOrColumnForHideableGrid(bool createRows)
        {
            ClearDefinitions(createRows, hideableGrid);

            if(createRows) hideableGrid.RowDefinitions.AddAmount(orderedNavigations.Count());
            else hideableGrid.ColumnDefinitions.AddAmount(orderedNavigations.Count());
        }
        #endregion

        private void ClearDefinitions(bool isForRows, Grid grid)
        {
            if (isForRows)
                grid.RowDefinitions.Clear();
            else
                grid.ColumnDefinitions.Clear();
        }

        private void SetPageInFrame(NavigationInfo info)
        {
            pageViewer.Content = info.Content;
        }

        private void SetPageInFrame(Page page)
        {
            pageViewer.Content = page;
        }
    }
}
