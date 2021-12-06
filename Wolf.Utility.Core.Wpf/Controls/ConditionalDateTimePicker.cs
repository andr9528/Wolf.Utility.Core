using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Wolf.Utility.Core.Wpf.Extensions;
using Wolf.Utility.Core.Exceptions;

using Xceed.Wpf.Toolkit;

namespace Wolf.Utility.Core.Wpf.Controls
{

    public class ConditionalDateTimePicker : Control
    {
        public delegate void DateChanged(EventArgs args);

        public event DateChanged? StartDateChanged;
        public event DateChanged? EndDateChanged;

        static readonly string[] defaultConditions = new string[] { "Both", "Start", "None" };
        static readonly string[] defaultTexts = new string[] { "Start Time", "End Time"};
        private readonly IEnumerable<string> conditions;
        private readonly IEnumerable<string> texts;
        private Grid mainGrid = new Grid();

        private DateTimePicker startPicker = new DateTimePicker();
        private DateTimePicker endPicker = new DateTimePicker();
        private Label startLabel = new Label();
        private Label endLabel = new Label();
        private ComboBox conditionBox = new ComboBox();

        public DateTimePicker StartPicker => startPicker;
        public DateTimePicker EndPicker => endPicker;
        public Label StartLabel => startLabel;
        public Label EndLabel => endLabel;
        public ComboBox ConditionBox => conditionBox;


        /// <summary>
        /// A DateTime Picker for chosing a Start time and a End Time. 
        /// Has the possiblity of enableing/disableing either Start or End Picker or both, depending on need at the moment of usage.
        /// </summary>
        /// <param name="conditions">Should contain 3 values - throws exceptions if incorrect count; 
        /// 1: text for when both DateTimePickers should be active, 
        /// 2: text for when only start DateTimePicker should be Active, 
        /// 3: text for when none should be active</param>
        /// <param name="texts">Should contain 2 values - throws exceptions if incorrect count;
        /// 1: text for describing the Start Time DateTimePicker
        /// 2: text for describing the End Time DateTimePicker</param>
        /// <exception cref="ArgumentNullException">Thrown when either of the inputs are null.</exception>
        /// <exception cref="ArgumentException">Thrown when either of the input deosn't match the required count. 
        /// While inputing the parameter, how many and what each value is for is specified by Intellisence</exception>
        public ConditionalDateTimePicker(IEnumerable<string> conditions, IEnumerable<string> texts)
        {
            if (conditions == null) throw new ArgumentNullException(nameof(conditions));
            if (texts == null) throw new ArgumentNullException(nameof(texts));
            if (conditions.Count() != 3) 
                throw new ArgumentException($"{nameof(conditions)} has to contain exactly 3 conditions names. " +
                    $"First being for Both, second for Start and third for None");
            if (texts.Count() != 2) throw new ArgumentException($"{nameof(texts)} has to contain exactly 2 text box texts. " +
                     $"First being for the Start Time and the second for End Time");

            this.conditions = conditions;
            this.texts = texts;

            SetupControls();
            
        }

        /// <summary>
        /// A DateTime Picker for chosing a Start time and a End Time. 
        /// Has the possiblity of enableing/disableing either Start or End Picker or both, depending on need at the moment of usage.
        /// Calls the main constructor with the values for conditions set to: Both, Start and None. 
        /// The default values for texts are set to: Start Time and End Time.
        /// </summary>
        public ConditionalDateTimePicker() : this(defaultConditions, defaultTexts)
        {
            
        }

        private void SetupControls()
        {
            mainGrid = new Grid() 
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
            };

            mainGrid.ColumnDefinitions.AddAmount(2);
            mainGrid.RowDefinitions.AddAmount(3);

            conditionBox = new ComboBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 10, 20, 10), 
                DataContext = conditions
            };
            conditionBox.SelectionChanged += ConditionBox_SelectionChanged;

            Grid.SetColumnSpan(conditionBox, 2);
            Grid.SetRow(conditionBox, 0);
            Grid.SetColumn(conditionBox, 0);
            mainGrid.Children.Add(conditionBox);

            SetupLabels();
        }

        private void SetupLabels()
        {
            startLabel = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 10, 20, 10),
                Content = texts.ToList()[0]
            };

            Grid.SetRow(startLabel, 1);
            Grid.SetColumn(startLabel, 0);
            mainGrid.Children.Add(startLabel);

            endLabel = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 10, 20, 10),
                Content = texts.ToList()[1]
            };

            Grid.SetRow(endLabel, 2);
            Grid.SetColumn(endLabel, 0);
            mainGrid.Children.Add(endLabel);

            SetupDateTimePickers();
        }

        private void SetupDateTimePickers()
        {
            startPicker = new DateTimePicker()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 10, 20, 10)
            };
            startPicker.ValueChanged += StartPicker_ValueChanged;

            Grid.SetRow(startPicker, 1);
            Grid.SetColumn(startPicker, 1);
            mainGrid.Children.Add(startPicker);

            endPicker = new DateTimePicker()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20, 10, 20, 10)
            };
            endPicker.ValueChanged += EndPicker_ValueChanged;

            Grid.SetRow(startPicker, 2);
            Grid.SetColumn(startPicker, 1);
            mainGrid.Children.Add(startPicker);
        }

        private void StartPicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            StartDateChanged?.Invoke(e);
        }

        private void EndPicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            EndDateChanged?.Invoke(e);
        }

        private void ConditionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (conditionBox.SelectedItem) 
            {
                case 0:
                    startPicker.IsEnabled = true;
                    EndPicker.IsEnabled = true;
                    break;
                case 1:
                    startPicker.IsEnabled = true;
                    EndPicker.IsEnabled = false;
                    break;
                case 2:
                    startPicker.IsEnabled = false;
                    EndPicker.IsEnabled = false;
                    break;
                default:
                    throw new ImpossibleException($"Code reached default section of switch in {nameof(ConditionBox_SelectionChanged)}. " +
                        $"This should be Impossible, as no more than the listed cases should ever be able to exists");
            }

        }
    }
}
