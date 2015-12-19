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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Model.Initialize();

            if (BandTrackerClient.Instance.LoginTokenRestore() || LoginDialog.Run())
            {
                Model.Pages.Add(ViewModelBands.Id);
                Model.Pages.Add(ViewModelTasks.Id);
            }
        }

        // properties
        protected ViewModelMainWindow Model { get { return (ViewModelMainWindow) Resources["ViewModel"]; } } 
    }

    public class PageDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (item is string)
            {
                return element.FindResource("TemplatePage" + (item as string)) as DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
