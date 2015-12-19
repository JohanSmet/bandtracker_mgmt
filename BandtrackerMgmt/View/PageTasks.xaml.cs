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

namespace BandtrackerMgmt
{
    /// <summary>
    /// Interaction logic for PageTasks.xaml
    /// </summary>
    public partial class PageTasks : UserControl
    {
        public PageTasks()
        {
            InitializeComponent();
            Model.Initialize();
        }

        // properties
        protected ViewModelTasks Model { get { return (ViewModelTasks)Resources["ViewModel"]; } } 
    }

    public class StringArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is List<string>)
            {
                var f_src = value as List<string>;
                var f_dst = new StringBuilder();

                for (var f_idx = 0; f_idx < f_src.Count && f_idx < 2; ++f_idx)
                    f_dst.AppendLine(f_src[f_idx]);

                if (f_src.Count > 3)
                {
                    f_dst.AppendLine("...")
                         .AppendLine(f_src.Last());
                }
                else if (f_src.Count == 3)
                { 
                    f_dst.AppendLine(f_src.Last());
                }

                return f_dst.ToString().TrimEnd();
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is ServerTask.Status)
            {
                var status = (ServerTask.Status)value;

                if (status == ServerTask.Status.Running)
                    return Brushes.Yellow;
                if (status == ServerTask.Status.FinishedOk)
                    return Brushes.LightGreen;
                if (status == ServerTask.Status.Failed)
                    return Brushes.OrangeRed;
                else
                    return Brushes.White;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
