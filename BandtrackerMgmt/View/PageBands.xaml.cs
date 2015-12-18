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
    /// Interaction logic for PageBands.xaml
    /// </summary>
    public partial class PageBands : UserControl
    {
        public PageBands()
        {
            InitializeComponent();
            Model.Initialize();
        }

        // properties
        protected ViewModelBands Model { get { return (ViewModelBands) Resources["ViewModel"]; } } 
    }

    public class HtmlFontConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                return "<span style=\"font-size:10pt;\">" + (value as string) + "</span>";
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
