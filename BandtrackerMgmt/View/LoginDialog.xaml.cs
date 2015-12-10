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
using System.Windows.Shapes;

namespace BandtrackerMgmt
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        public static bool Run()
        {
            var f_dialog = new LoginDialog();
            return f_dialog.ShowDialog() ?? false;
        }

        public LoginDialog()
        {
            InitializeComponent();
        }

        // events

        private async void btnOK_click(object sender, RoutedEventArgs e)
        {
            if (await BandTrackerClient.Instance.LoginAsync(txtUsername.Text, txtPassword.Password))
                this.DialogResult = true;
            else
                lblError.Content = "Login failed...";
        }

        private void btnCancel_click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
