using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Labit.Controls
{
    /// <summary>
    /// Interaction logic for DateRangeControl.xaml
    /// </summary>
    public partial class DateRangeControl : UserControl
    {
        public DateRangeControl()
        {
            InitializeComponent();
            this.button.Click += Button_Click;
        }

        public static string ToDateLessOrEqaulFromDate = string.Format("'From Date' must be less than 'To Date'{0}otherwise you will get empty result.", Environment.NewLine);

        public static readonly DependencyProperty FromDateFieldVisibilityProperty =
            DependencyProperty.Register("FromDateFieldVisibility", typeof(Visibility), typeof(DateRangeControl), new UIPropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ToDateProperty =
            DependencyProperty.Register("ToDate", typeof(DateTime), typeof(DateRangeControl), new UIPropertyMetadata(DateTime.Today, PropertyChangedCallback));
        public static readonly DependencyProperty FromDateProperty =
            DependencyProperty.Register("FromDate", typeof(DateTime), typeof(DateRangeControl), new UIPropertyMetadata(DateTime.Today.AddDays(-1), PropertyChangedCallback));
        public static readonly DependencyProperty IsOpacueProperty =
            DependencyProperty.Register("IsOpacue", typeof(bool), typeof(DateRangeControl), new UIPropertyMetadata(false));
        public static readonly DependencyProperty ErrorCodeProperty =
            DependencyProperty.Register("ErrorCode", typeof(string), typeof(DateRangeControl), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty ErrorCodeVisibilityProperty =
            DependencyProperty.Register("ErrorCodeVisibility", typeof(Visibility), typeof(DateRangeControl), new UIPropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty SelectionDescriptionProperty =
            DependencyProperty.Register("SelectionDescription", typeof(string), typeof(DateRangeControl), new UIPropertyMetadata(string.Empty));
        public static readonly DependencyProperty IsPopupVisibleProperty =
            DependencyProperty.Register("IsPopupVisible", typeof(bool), typeof(DateRangeControl), new UIPropertyMetadata(false));
        public static readonly DependencyProperty RelativeDateProperty =
            DependencyProperty.Register("RelativeDate", typeof(string), typeof(DateRangeControl), new UIPropertyMetadata("-1 0", RelativeDatePropertyChangedCallback, CoerceValueCallback));
        public static readonly DependencyProperty DateRangeChangedProperty =
            DependencyProperty.Register("DateRangeChanged", typeof(ICommand), typeof(DateRangeControl), new UIPropertyMetadata(null));


        public Visibility FromDateFieldVisibility
        {
            get
            {
                return (Visibility)GetValue(FromDateFieldVisibilityProperty);
            }
            set
            {
                SetValue(FromDateFieldVisibilityProperty, value);
            }
        }

        public ICommand DateRangeChanged
        {
            get
            {
                return GetValue(DateRangeChangedProperty) as ICommand;
            }
            set
            {
                SetValue(DateRangeChangedProperty, value);
            }
        }

        public DateTime FromDate
        {
            get
            {
                return (DateTime)GetValue(FromDateProperty);
            }
            set
            {
                SetValue(FromDateProperty, value);
            }
        }

        public DateTime ToDate
        {
            get
            {
                return (DateTime)GetValue(ToDateProperty);
            }
            set
            {
                SetValue(ToDateProperty, value);
            }
        }

        public string RelativeDates
        {
            get
            {
                return GetValue(RelativeDateProperty) as string;
            }
            set
            {
                SetValue(RelativeDateProperty, value);
            }
        }

        public bool IsOpacue
        {
            get
            {
                return (bool)GetValue(IsOpacueProperty);
            }
            set
            {
                SetValue(IsOpacueProperty, value);
            }
        }

        public string ErrorCode
        {
            get
            {
                return (string)GetValue(ErrorCodeProperty);
            }
            set
            {
                SetValue(ErrorCodeProperty, value);
            }
        }

        public string SelectionDescription
        {
            get
            {
                return (string)GetValue(SelectionDescriptionProperty);
            }
            set
            {
                SetValue(SelectionDescriptionProperty, value);
            }
        }

        public Visibility ErrorCodeVisibility
        {
            get
            {
                return (Visibility)GetValue(ErrorCodeVisibilityProperty);
            }
            set
            {
                SetValue(ErrorCodeVisibilityProperty, value);
            }
        }

        public bool IsPopupVisible
        {
            get
            {
                return (bool)GetValue(IsPopupVisibleProperty);
            }
            set
            {
                SetValue(IsPopupVisibleProperty, value);
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.IsPopupVisible = true;
        }

        private static void RelativeDatePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var relativeDates = e.NewValue as string;
            var dateControl = d as DateRangeControl;
            int value = 0;
            if (!string.IsNullOrEmpty(relativeDates))
            {
                var values = relativeDates.Split(new char[] { ' ' }, StringSplitOptions.None);
                switch (values.Length)
                {
                    case 0:
                        break;
                    case 1:
                        if (Int32.TryParse(values[0], out value))
                            dateControl.FromDate = DateTime.Today.AddDays(value);
                        break;
                    default:
                        if (Int32.TryParse(values[1], out value))
                            dateControl.ToDate = DateTime.Today.AddDays(value);
                        if (Int32.TryParse(values[0], out value))
                            dateControl.FromDate = DateTime.Today.AddDays(value);
                        break;
                }
            }
        }

        private static object CoerceValueCallback(DependencyObject d, object baseValue)
        {
            var relativeDates = baseValue as string;
            var dateControl = d as DateRangeControl;
            if (!string.IsNullOrEmpty(relativeDates))
            {
                var values = relativeDates.Split(new char[] { ' ' }, StringSplitOptions.None);
                if (values.Length > 2)
                {
                    int d1 = 0;
                    int d0 = 0;
                    if (Int32.TryParse(values[0], out d1) && Int32.TryParse(values[1], out d0))
                    {
                        baseValue = string.Format("{0} {1}", d1, d0);
                    }
                }
            }
            return baseValue;
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Mouse.Capture(null);
            var today = DateTime.Today;
            var ctrl = d as DateRangeControl;
            if (ctrl.ToDate < ctrl.FromDate)
            {
                ctrl.ErrorCode = ToDateLessOrEqaulFromDate;
                ctrl.ErrorCodeVisibility = Visibility.Visible;
                ctrl.SelectionDescription = ctrl.ErrorCode;
            }
            else if (ctrl.FromDate > today || today > ctrl.ToDate)
            {
                ctrl.IsOpacue = true;
                ctrl.SelectionDescription = "Todays data will not be seen";
            }
            else
            {
                ctrl.ErrorCode = string.Empty;
                ctrl.ErrorCodeVisibility = Visibility.Collapsed;
                ctrl.SelectionDescription = ctrl.ErrorCode;
            }
            ctrl.IsOpacue = ctrl.ToDate < today;

            ctrl.FromDateFieldVisibility = ctrl.ToDate.Day != ctrl.FromDate.Day ? Visibility.Visible : Visibility.Collapsed;

            int d1 = (int)(ctrl.FromDate - today).TotalDays;
            int d0 = (int)(ctrl.ToDate - today).TotalDays;
            ctrl.RelativeDates = string.Format("{0} {1}", d1, d0);
            ctrl.calTo.DisplayDate = ctrl.ToDate;
            ctrl.calFrom.DisplayDate = ctrl.FromDate;
        }

        private void TodayButtonClick(object sender, RoutedEventArgs e)
        {
            this.ToDate = DateTime.Today;
            this.FromDate = ToDate;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            this.IsPopupVisible = false;
            if (DateRangeChanged != null)
                DateRangeChanged.Execute(null);
        }

        private void PreviewButtonClick(object sender, MouseButtonEventArgs e)
        {
            Button button = null;
            var source = e.Source as FrameworkElement;
            while (source != null)
            {
                if (source is Button)
                {
                    button = source as Button;
                    break;
                }
                else
                    source = LogicalTreeHelper.GetParent(source) as FrameworkElement;
            }

            if (button != null)
            {
                var p = e.GetPosition(button);
                var r = new Rect(0, 0, todayButton.ActualWidth, todayButton.ActualHeight);
                if (r.Contains(p))
                {
                    if (button.Name == "todayButton")
                    {
                        TodayButtonClick(sender, null);
                        e.Handled = true;
                    }
                    else if (button.Name == "okButton")
                    {
                        OkButtonClick(sender, null);
                        e.Handled = true;
                    }
                }
            }
        }
    }
}